using MealMind.Modules.AiChat.Application.Abstractions;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand;

public record GetChatResponseCommand(Guid ConversationId, string Prompt) : ICommand<string>
{
    internal sealed class Handler : ICommandHandler<GetChatResponseCommand, string>
    {
        private readonly IAiChatService _aiChatService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IAiChatService aiChatService, IUnitOfWork unitOfWork)
        {
            _aiChatService = aiChatService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(GetChatResponseCommand request, CancellationToken cancellationToken)
        {
            var response = await _aiChatService.GetResponseAsync(request.Prompt, request.ConversationId, cancellationToken);
            return Result.Ok(response);
        }
    }
}