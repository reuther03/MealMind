using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;

namespace MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand;

public record GetChatResponseCommand : ICommand<string>
{
    internal sealed class Handler : ICommandHandler<GetChatResponseCommand, string>
    {
        public Task<Result<string>> Handle(GetChatResponseCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}