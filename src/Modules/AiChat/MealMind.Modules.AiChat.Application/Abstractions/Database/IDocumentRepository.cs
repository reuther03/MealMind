using System.Reflection.Metadata;
using MealMind.Shared.Abstractions.Kernel.Database;
using Document = MealMind.Modules.AiChat.Domain.Rag.Document;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IDocumentRepository : IRepository<Document>
{

}