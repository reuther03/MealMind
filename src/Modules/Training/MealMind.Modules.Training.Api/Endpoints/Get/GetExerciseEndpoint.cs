using MealMind.Modules.Training.Application.Features.Queries;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Get;

public class GetExerciseEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/exercises/{exerciseId:guid}",
                async (Guid exerciseId, ISender sender) =>
                {
                    var result = await sender.Send(new GetExerciseDetailsQuery(exerciseId));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Get Exercise",
                "Returns full details of a single exercise from the catalog (name, description, media, type, muscle group, custom flag).",
                "",
                """
                {
                  "value": {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "name": "Bench Press",
                    "description": "Compound chest exercise performed on a flat bench.",
                    "imageUrl": null,
                    "videoUrl": null,
                    "type": "Strength",
                    "muscleGroup": "Chest",
                    "isCustom": false
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
