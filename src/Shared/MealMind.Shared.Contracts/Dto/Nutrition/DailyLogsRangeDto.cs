namespace MealMind.Shared.Contracts.Dto.Nutrition;

public class DailyLogsRangeDto
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public int DaysLogged { get; init; }
    public int TotalDaysInRange { get; init; }
    public int DaysOnTarget { get; init; }
    public NutritionTotalsDto RangeTotals { get; init; } = null!;
    public NutritionAveragesDto DailyAverages { get; init; } = null!;
    public List<DailyLogDto> DailyLogs { get; init; } = [];
}
