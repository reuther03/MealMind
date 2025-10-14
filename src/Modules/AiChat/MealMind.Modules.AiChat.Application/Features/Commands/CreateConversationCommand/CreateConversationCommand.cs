using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.AI;

namespace MealMind.Modules.AiChat.Application.Features.Commands.CreateConversationCommand;

public record CreateConversationCommand(string Prompt) : ICommand<string>
{
    public sealed class Handler : ICommandHandler<CreateConversationCommand, string>
    {
        private readonly IChatClient _chatClient;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IChatClient chatClient, IConversationRepository conversationRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _chatClient = chatClient;
            _conversationRepository = conversationRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(CreateConversationCommand command, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<string>.BadRequest("User is not authenticated");

            var conversation = Conversation.Create(_userService.UserId!.Value, "New Conversation");
            await _conversationRepository.AddAsync(conversation, cancellationToken);

            var chatMessages = new List<ChatMessage>();

            // creats initial message of type ChatMessage which is type used by the IChatClient to get a response from the AI model
            var systemInstruction = new ChatMessage(ChatRole.System,
                """
                Your are a helpful assistant that helps users to plan their meals and nutrition. Provide recipes, nutritional information,
                and meal suggestions based on user preferences and dietary needs.
                """);
            // creates initial message of type AiChatMessage which is type used to store messages in the database
            var aiChatSystemMessage = AiChatMessage.Create(conversation.Id, AiChatRole.System, systemInstruction.Text, Guid.Empty);


            // user message - the prompt provided by the user to IChatClient
            var userMessage = new ChatMessage(ChatRole.User, command.Prompt);
            // user message - the prompt provided by the user to be stored in the database
            var aiChatUserMessage = AiChatMessage.Create(conversation.Id, AiChatRole.User, command.Prompt, aiChatSystemMessage.Id);

            chatMessages.Add(systemInstruction);
            chatMessages.Add(userMessage);

            conversation.AddMessage(aiChatSystemMessage);
            conversation.AddMessage(aiChatUserMessage);

            var response = await _chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);
            var assistantMessage = AiChatMessage.Create(conversation.Id, AiChatRole.Assistant, response.Text, aiChatUserMessage.Id);
            conversation.AddMessage(assistantMessage);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(response.Text);
        }
    }
}