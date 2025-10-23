using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand;

public record GetChatResponseCommand(Guid ConversationId, string Prompt) : ICommand<StructuredResponse>
{
    public sealed class Handler : ICommandHandler<GetChatResponseCommand, StructuredResponse>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserService _userService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IResponseManager _responseManager;
        private readonly IUnitOfWork _unitOfWork;


        public Handler(IConversationRepository conversationRepository, IDocumentRepository documentRepository, IUserService userService,
            IEmbeddingService embeddingService, IResponseManager responseManager, IUnitOfWork unitOfWork)
        {
            _conversationRepository = conversationRepository;
            _documentRepository = documentRepository;
            _userService = userService;
            _embeddingService = embeddingService;
            _responseManager = responseManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StructuredResponse>> Handle(GetChatResponseCommand request, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<StructuredResponse>.BadRequest("User is not authenticated");

            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            NullValidator.ValidateNotNull(conversation);

            var chatMessages = conversation.ChatMessages
                .Where(x => x.Role != AiChatRole.System)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new ChatMessage(new ChatRole(x.Role.ToString()), x.Content))
                .ToList();

            var userInputEmbeddings = await _embeddingService.GenerateEmbeddingAsync(request.Prompt, cancellationToken);
            var relevantDocuments = await _documentRepository.GetRelevantDocumentsAsync(userInputEmbeddings.ToArray(), cancellationToken);

            if (!relevantDocuments.Any())
                return Result<StructuredResponse>.BadRequest("No relevant documents found.");

            var documentsText = string.Join("\n\n", relevantDocuments.Select(x => $"Title: {x.Title}\nContent: {x.Content}"));
            var documentTitles = relevantDocuments.Select(x => x.Title).ToList();

            //should aichatmessage be created before or after response manager call?

            var response = await _responseManager.GenerateStructuredResponseAsync(request.Prompt, documentsText, documentTitles, chatMessages,
                cancellationToken);
            var responseString = System.Text.Json.JsonSerializer.Serialize(response);

            //keep it here or move to response manager?
            var aiChatMessage = AiChatMessage.Create(conversation.Id, AiChatRole.User, request.Prompt, conversation.GetRecentMessage().Id);
            conversation.AddMessage(aiChatMessage);

            var assistantMessage = AiChatMessage.Create(conversation.Id, AiChatRole.Assistant, responseString, aiChatMessage.Id);
            conversation.AddMessage(assistantMessage);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(response);
        }
    }
}