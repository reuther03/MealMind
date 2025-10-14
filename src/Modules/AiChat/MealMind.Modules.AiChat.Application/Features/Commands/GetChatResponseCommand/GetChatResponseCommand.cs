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
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;


        public Handler(IChatClient chatClient, IConversationRepository conversationRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _chatClient = chatClient;
            _conversationRepository = conversationRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(GetChatResponseCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            NullValidator.ValidateNotNull(conversation);

            var chatMessages = conversation.ChatMessages
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ChatMessage(new ChatRole(x.Role.ToString()), x.Content))
                .ToList();

            NullValidator.ValidateNotNull(chatMessages);

            var userMessage = new ChatMessage(new ChatRole("user"), request.Prompt);
            var aiChatMessage = AiChatMessage.Create(conversation.Id, AiChatRole.User, request.Prompt, conversation.GetRecentMessage().Id);

            chatMessages.Add(userMessage);
            conversation.AddMessage(aiChatMessage);

            var response = await _chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);

            var assistantMessage = AiChatMessage.Create(conversation.Id, AiChatRole.Assistant, response.Text, aiChatMessage.Id);
            conversation.AddMessage(assistantMessage);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(response.Text);
        }
    }
}