using MealMind.Modules.Training.Application.Features.Queries;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Get;

public class GetExercisesEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/exercises",
                async (string? name, ExerciseType? exerciseType, MuscleGroup? muscleGroup,
                    bool? isCustom, int? pageNumber, int? pageSize,
                    ISender sender) =>
                {
                    var result = await sender.Send(new GetExercisesByFilterParametersQuery(
                        name, exerciseType, muscleGroup, isCustom,
                        pageNumber ?? 1, pageSize ?? 10));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Get Exercises",
                "Searches the exercise catalog with optional filters. All parameters are optional - combine as needed. Supports pagination.",
                """
                Query parameters:
                - name: "bench" (partial match)
                - exerciseType: 0 (Strength) or 1 (Cardio)
                - muscleGroup: 0 (Chest), 1 (Back), 2 (Legs), etc.
                - isCustom: true/false
                - pageNumber: 1 (default)
                - pageSize: 10 (default)
                """,
                """
                {
                  "value": {
                    "pageNumber": 1,
                    "pageSize": 10,
                    "totalCount": 25,
                    "items": [
                      {
                        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "name": "Bench Press",
                        "imageUrl": null,
                        "type": "Strength",
                        "muscleGroup": "Chest",
                        "isCustom": false
                      }
                    ]
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
