using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand;

public record GetChatResponseCommand(Guid ConversationId, string Prompt) : ICommand<string>
{
    public sealed class Handler : ICommandHandler<GetChatResponseCommand, string>
    {
        private readonly IChatClient _chatClient;
        private readonly IConversationRepository _conversationRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserService _userService;
        private readonly IEmbeddingService _embeddingService;
        private readonly IUnitOfWork _unitOfWork;


        public Handler(IChatClient chatClient, IConversationRepository conversationRepository, IDocumentRepository documentRepository, IUserService userService,
            IEmbeddingService embeddingService,
            IUnitOfWork unitOfWork)
        {
            _chatClient = chatClient;
            _conversationRepository = conversationRepository;
            _documentRepository = documentRepository;
            _userService = userService;
            _embeddingService = embeddingService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(GetChatResponseCommand request, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<string>.BadRequest("User is not authenticated");

            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            NullValidator.ValidateNotNull(conversation);

            var chatMessages = conversation.ChatMessages
                .OrderBy(x => x.CreatedAt)
                .Select(x => new ChatMessage(new ChatRole(x.Role.ToString()), x.Content))
                .ToList();

            var userInputEmbeddings = await _embeddingService.GenerateEmbeddingAsync(request.Prompt, cancellationToken);

            var relevantDocuments = await _documentRepository.GetRelevantDocumentsAsync(userInputEmbeddings.ToArray(), cancellationToken);

            if (!relevantDocuments.Any())
                return Result<string>.BadRequest("No relevant documents found.");

            var documentsText = string.Join("\n", relevantDocuments.Select(d => new { d.Content }));

            var systemPrompt =
                $"Use the following documents to answer the user's question:\n {documentsText}, response should be short but informative.";

            var systemMessage = new ChatMessage(ChatRole.System, systemPrompt);
            chatMessages.Add(systemMessage);

            var userMessage = new ChatMessage(ChatRole.User, request.Prompt);
            var aiChatMessage = AiChatMessage.Create(conversation.Id, AiChatRole.User, request.Prompt, conversation.GetRecentMessage().Id);

            chatMessages.Add(userMessage);
            conversation.AddMessage(aiChatMessage);

            NullValidator.ValidateNotNull(chatMessages);

            var response = await _chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);

            var assistantMessage = AiChatMessage.Create(conversation.Id, AiChatRole.Assistant, response.Text, aiChatMessage.Id);
            conversation.AddMessage(assistantMessage);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(response.Text);
        }
    }
}