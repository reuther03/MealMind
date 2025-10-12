using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IConversationRepository : IRepository<Conversation>
{

}