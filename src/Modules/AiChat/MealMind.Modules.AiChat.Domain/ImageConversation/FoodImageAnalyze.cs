using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.ImageConversation;

public class FoodImageAnalyze : AggregateRoot<Guid>
{
    public UserId UserId { get; private set; }
    public string? Prompt { get; private set; }
    public string? ImageUrl { get; private set; }
    public byte[]? ImageBytes { get; private set; }
    public decimal CaloriesMin { get; private init; }
    public decimal CaloriesMax { get; private init; }
    public decimal ProteinMin { get; private init; }
    public decimal ProteinMax { get; private init; }
    public decimal CarbsMin { get; private init; }
    public decimal CarbsMax { get; private init; }
    public decimal FatMin { get; private init; }
    public decimal FatMax { get; private init; }
    public decimal? TotalSugars { get; private set; }
    public decimal? TotalSaturatedFats { get; private set; }
    public decimal? TotalFiber { get; private set; }
    public decimal? TotalSodium { get; private set; }
    public decimal? TotalSalt { get; private set; }
    public decimal? TotalCholesterol { get; private set; }
    public string Response { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SavedAt { get; private set; }

    private FoodImageAnalyze()
    {
    }

    private FoodImageAnalyze(
        Guid id,
        UserId userId,
        string? prompt,
        string? imageUrl,
        byte[]? imageBytes,
        decimal caloriesMin,
        decimal caloriesMax,
        decimal proteinMin,
        decimal proteinMax,
        decimal carbsMin,
        decimal carbsMax,
        decimal fatMin,
        decimal fatMax,
        decimal? totalSugars,
        decimal? totalSaturatedFats,
        decimal? totalFiber,
        decimal? totalSodium,
        decimal? totalSalt,
        decimal? totalCholesterol,
        string response) : base(id)
    {
        if (imageUrl is null && imageBytes is null)
            throw new DomainException("Either imageUrl or imageBytes must be provided.");

        UserId = userId;
        Prompt = prompt;
        ImageUrl = imageUrl;
        ImageBytes = imageBytes;
        CaloriesMin = caloriesMin;
        CaloriesMax = caloriesMax;
        ProteinMin = proteinMin;
        ProteinMax = proteinMax;
        CarbsMin = carbsMin;
        CarbsMax = carbsMax;
        FatMin = fatMin;
        FatMax = fatMax;
        TotalSugars = totalSugars;
        TotalSaturatedFats = totalSaturatedFats;
        TotalFiber = totalFiber;
        TotalSodium = totalSodium;
        TotalSalt = totalSalt;
        TotalCholesterol = totalCholesterol;
        Response = response;
        CreatedAt = DateTime.UtcNow;
    }

    public static FoodImageAnalyze Create(
        UserId userId,
        string? prompt,
        string? imageUrl,
        byte[]? imageBytes,
        decimal caloriesMin,
        decimal caloriesMax,
        decimal proteinMin,
        decimal proteinMax,
        decimal carbsMin,
        decimal carbsMax,
        decimal fatMin,
        decimal fatMax,
        decimal? totalSugars,
        decimal? totalSaturatedFats,
        decimal? totalFiber,
        decimal? totalSodium,
        decimal? totalSalt,
        decimal? totalCholesterol,
        string response)
    {
        return new FoodImageAnalyze(
            Guid.NewGuid(),
            userId,
            prompt,
            imageUrl,
            imageBytes,
            caloriesMin,
            caloriesMax,
            proteinMin,
            proteinMax,
            carbsMin,
            carbsMax,
            fatMin,
            fatMax,
            totalSugars,
            totalSaturatedFats,
            totalFiber,
            totalSodium,
            totalSalt,
            totalCholesterol,
            response);
    }
}