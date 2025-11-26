using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.ImageAnalyze;

public class ImageAnalyze : AggregateRoot<Guid>
{
    public UserId UserId { get; private set; }
    public string FoodName { get; private set; }
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
    public decimal ConfidenceScore { get; private init; }
    public double TotalQuantityInGrams { get; private init; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SavedAt { get; private set; }

    private ImageAnalyze()
    {
    }

    private ImageAnalyze(
        Guid id,
        UserId userId,
        string foodName,
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
        decimal confidenceScore,
        double totalQuantityInGrams,
        DateTime? savedAt = null
    ) : base(id)
    {
        if (imageUrl is null && imageBytes is null)
            throw new DomainException("Either imageUrl or imageBytes must be provided.");

        UserId = userId;
        Prompt = prompt;
        FoodName = foodName;
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
        ConfidenceScore = confidenceScore;
        TotalQuantityInGrams = totalQuantityInGrams;
        CreatedAt = DateTime.UtcNow;
        SavedAt = savedAt;
    }

    public static ImageAnalyze Create(
        UserId userId,
        string foodName,
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
        decimal confidenceScore,
        double totalQuantityInGrams,
        DateTime? savedAt = null
    )
    {
        return new ImageAnalyze(
            Guid.NewGuid(),
            userId,
            foodName,
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
            confidenceScore,
            totalQuantityInGrams,
            savedAt
        );
    }
}