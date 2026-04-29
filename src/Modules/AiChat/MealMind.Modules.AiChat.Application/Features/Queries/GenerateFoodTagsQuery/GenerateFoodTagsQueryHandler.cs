using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.AiChat;
using MealMind.Shared.Contracts.Result;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.AiChat.Application.Features.Queries.GenerateFoodTagsQuery;

public class GenerateFoodTagsQueryHandler : IQueryHandler<Shared.Abstractions.Messaging.AiChat.GenerateFoodTagsQuery, FoodTagsResult>
{
    private readonly IAiChatService _aiChatService;
    private readonly ILogger<GenerateFoodTagsQueryHandler> _logger;

    public GenerateFoodTagsQueryHandler(IAiChatService aiChatService, ILogger<GenerateFoodTagsQueryHandler> logger)
    {
        _aiChatService = aiChatService;
        _logger = logger;
    }

    public async Task<Result<FoodTagsResult>> Handle(Shared.Abstractions.Messaging.AiChat.GenerateFoodTagsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _aiChatService.GenerateFoodTagsAsync(query.ProductName, query.Brand, cancellationToken);

            if (response.Categories.Count == 0 && response.DietaryTags.Count == 0)
                return Result<FoodTagsResult>.NotFound("No tags found for the given food item.");

            return Result<FoodTagsResult>.Ok(response);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Food tag generation failed for {FoodName}.", query.ProductName);
            return Result<FoodTagsResult>.BadRequest("Automatic tagging is currently unavailable.");
        }
    }
}