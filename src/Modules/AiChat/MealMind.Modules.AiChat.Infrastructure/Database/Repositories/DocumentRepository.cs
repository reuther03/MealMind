using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Rag;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class DocumentRepository : Repository<Document, AiChatDbContext>, IDocumentRepository
{
    public DocumentRepository(AiChatDbContext dbContext) : base(dbContext)
    {
    }


}