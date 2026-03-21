using MealMind.Modules.Training.Application.Features.Commands.UpdateCardioDetails;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class UpdateCardioDetailsEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPut("/training-plans/{planId:guid}/sessions/{sessionId:guid}/exercises/{exerciseId:guid}/cardio",
                async (Guid planId, Guid sessionId, Guid exerciseId, CardioDetails cardioDetails,
                    ISender sender) =>
                {
                    var result = await sender.Send(new UpdateCardioDetailsCommand(planId, sessionId, exerciseId, cardioDetails));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Update Cardio Details",
                "Replaces the cardio details (duration, distance, calories, heart rate) for a specific exercise in a training session.",
                """
                {
                  "durationInMinutes": 45,
                  "distanceInKm": 5.3,
                  "caloriesBurned": 420,
                  "averageHeartRate": 145,
                  "averageSpeed": 7.1,
                  "notes": "Steady pace, felt good",
                  "caloriesEstimated": null
                }
                """,
                """
                {
                  "value": true,
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
