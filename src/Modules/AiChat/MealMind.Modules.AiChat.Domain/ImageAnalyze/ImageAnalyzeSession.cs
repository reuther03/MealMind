using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Domain.ImageAnalyze;

public class ImageAnalyzeSession : AggregateRoot<Guid>
{
    private readonly IList<ImageAnalyze> _corrections = [];

    public UserId UserId { get; private set; }
    public ImageAnalyze ImageAnalyze { get; private set; }
    public Guid ImageAnalyzeId { get; private set; }
    public IReadOnlyList<ImageAnalyze> Corrections => _corrections.AsReadOnly();
    public DateTime CreatedAt { get; private set; }

    private ImageAnalyzeSession()
    {
    }

    private ImageAnalyzeSession(Guid id, UserId userId, ImageAnalyze imageAnalyze) : base(id)
    {
        UserId = userId;
        ImageAnalyze = imageAnalyze;
        CreatedAt = DateTime.UtcNow;
    }

    public static ImageAnalyzeSession Create(UserId userId, ImageAnalyze imageAnalyze)
        => new(Guid.NewGuid(), userId, imageAnalyze);

    public void AddCorrection(ImageAnalyze correction)
        => _corrections.Add(correction);
}