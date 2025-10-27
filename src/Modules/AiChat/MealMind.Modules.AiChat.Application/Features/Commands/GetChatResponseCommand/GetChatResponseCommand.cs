using System.Text.Json;
using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Dtos;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.Extensions.AI;
using SharpToken;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand;

public record GetChatResponseCommand(Guid ConversationId, string Prompt) : ICommand<StructuredResponse>
{
    public sealed class Handler : ICommandHandler<GetChatResponseCommand, StructuredResponse>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IAiChatUserRepository _aiChatUserRepository;
        private readonly IUserService _userService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IResponseManager _responseManager;
        private readonly IUnitOfWork _unitOfWork;


        public Handler(IConversationRepository conversationRepository, IDocumentRepository documentRepository, IAiChatUserRepository aiChatUserRepository,
            IUserService userService,
            IEmbeddingService embeddingService, IResponseManager responseManager, IUnitOfWork unitOfWork)
        {
            _conversationRepository = conversationRepository;
            _documentRepository = documentRepository;
            _aiChatUserRepository = aiChatUserRepository;
            _userService = userService;
            _embeddingService = embeddingService;
            _responseManager = responseManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StructuredResponse>> Handle(GetChatResponseCommand request, CancellationToken cancellationToken)
        {
            var aiUser = await _aiChatUserRepository.GetByUserIdAsync(_userService.UserId!, cancellationToken);
            NullValidator.ValidateNotNull(aiUser);

            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            NullValidator.ValidateNotNull(conversation);

            if (aiUser.DailyPromptsLimit != -1 &&
                conversation.ChatMessages.Count(x => x.CreatedAt >= DateTime.UtcNow.Date) >= aiUser.DailyPromptsLimit)
                return Result<StructuredResponse>.BadRequest("Daily prompts limit exceeded.");

            var chatMessages = conversation.ChatMessages
                .Where(x => x.Role != AiChatRole.System)
                .OrderBy(x => x.CreatedAt)
                .Where(x => x.CreatedAt >= DateTime.UtcNow.AddDays(-aiUser.ConversationsMessagesHistoryDaysLimit)) // check if this is correct
                .Select(x => new ChatMessage(new ChatRole(x.Role.ToString()), x.Content))
                .ToList();

            var encoding = GptEncoding.GetEncoding("o200k_base");

            if (encoding.CountTokens(request.Prompt) > aiUser.PromptTokensLimit)
                return Result<StructuredResponse>.BadRequest("Prompt exceeds the token limit.");

            var userInputEmbeddings = await _embeddingService.GenerateEmbeddingAsync(request.Prompt, cancellationToken);
            var relevantDocuments = await _documentRepository.GetRelevantDocumentsAsync(userInputEmbeddings.ToArray(), cancellationToken);

            if (!relevantDocuments.Any())
                return Result<StructuredResponse>.BadRequest("No relevant documents found.");

            var documentsText = string.Join("\n\n",
                relevantDocuments.Select(x => $"Title: {x.Title}\nContent: {x.Content}"));

            var documentTitles = relevantDocuments.Select(x => x.Title).ToList();

            //should aichatmessage be created before or after response manager call?

            var response = await _responseManager.GenerateStructuredResponseAsync(
                request.Prompt,
                documentsText,
                documentTitles,
                chatMessages,
                aiUser.ResponseTokensLimit,
                cancellationToken
            );
            var responseString = JsonSerializer.Serialize(response);

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