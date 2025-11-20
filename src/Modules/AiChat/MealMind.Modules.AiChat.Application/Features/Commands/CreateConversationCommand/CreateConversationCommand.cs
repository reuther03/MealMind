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
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SharpToken;

namespace MealMind.Modules.AiChat.Application.Features.Commands.CreateConversationCommand;

public record CreateConversationCommand(string Prompt) : ICommand<StructuredResponse>
{
    public sealed class Handler : ICommandHandler<CreateConversationCommand, StructuredResponse>
    {
        private readonly IAiChatService _aiChatService;
        private readonly IConversationRepository _conversationRepository;
        private readonly IAiChatUserRepository _aiChatUserRepository;
        private readonly IUserService _userService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IEmbeddingService _embeddingService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IConversationRepository conversationRepository, IAiChatUserRepository aiChatUserRepository,
            IUserService userService,
            IDocumentRepository documentRepository,
            IEmbeddingService embeddingService,
            IUnitOfWork unitOfWork, IAiChatService aiChatService)
        {
            _conversationRepository = conversationRepository;
            _aiChatUserRepository = aiChatUserRepository;
            _userService = userService;
            _documentRepository = documentRepository;
            _embeddingService = embeddingService;

            _unitOfWork = unitOfWork;
            _aiChatService = aiChatService;
        }

        public async Task<Result<StructuredResponse>> Handle(CreateConversationCommand command, CancellationToken cancellationToken)
        {
            var aiUser = await _aiChatUserRepository.GetByUserIdAsync(_userService.UserId, cancellationToken);
            Validator.ValidateNotNull(aiUser);

            var userDailyPromptsCount = await _conversationRepository.GetUserDailyConversationPromptsCountAsync(aiUser.Id, cancellationToken);

            if (aiUser.DailyPromptsLimit != -1 &&
                userDailyPromptsCount >= aiUser.DailyPromptsLimit)
            {
                return Result<StructuredResponse>.BadRequest("Daily prompts limit exceeded.");
            }

            var conversation = Conversation.Create(aiUser.Id);

            var systemInstruction = new ChatMessageContent(AuthorRole.System,
                """
                Your are a helpful assistant that helps users to plan their meals and nutrition. Provide recipes, nutritional information,
                and meal suggestions based on user preferences and dietary needs.
                """);

            var aiChatSystemMessage = AiChatMessage.Create(conversation.Id, AiChatRole.System, systemInstruction.Content!, Guid.Empty);
            conversation.AddMessage(aiChatSystemMessage);

            var encoding = GptEncoding.GetEncoding("o200k_base");

            if (encoding.CountTokens(command.Prompt) > aiUser.PromptTokensLimit)
                return Result<StructuredResponse>.BadRequest("Prompt exceeds the token limit.");

            var userInputEmbeddings = await _embeddingService.GenerateEmbeddingAsync(command.Prompt, cancellationToken);
            var relevantDocuments = await _documentRepository.GetRelevantDocumentsAsync(userInputEmbeddings.ToArray(), cancellationToken);

            if (!relevantDocuments.Any())
                return Result<StructuredResponse>.BadRequest("No relevant documents found.");

            var documentsText = string.Join("\n\n",
                relevantDocuments.Select(x => $"Content: {x.Content}"));

            var userMessage = new ChatMessageContent(AuthorRole.User, command.Prompt);
            var aiChatUserMessage = AiChatMessage.Create(conversation.Id, AiChatRole.User, command.Prompt, aiChatSystemMessage.Id);

            conversation.AddMessage(aiChatUserMessage);

            var messages = new List<ChatMessageContent>
            {
                systemInstruction,
                userMessage
            };

            var response = await _aiChatService.GenerateStructuredResponseAsync(
                command.Prompt,
                documentsText,
                messages,
                aiUser.ResponseTokensLimit,
                cancellationToken
            );

            var responseString = JsonSerializer.Serialize(response);

            var assistantMessage = AiChatMessage.Create(conversation.Id, AiChatRole.Assistant, responseString, aiChatUserMessage.Id);
            conversation.AddMessage(assistantMessage);

            conversation.SetTitle(response.Title);

            await _conversationRepository.AddAsync(conversation, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(response);
        }
    }
}