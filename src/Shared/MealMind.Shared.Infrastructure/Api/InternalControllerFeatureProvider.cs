using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MealMind.Shared.Infrastructure.Api;

internal class InternalControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!typeInfo.IsClass)
        {
            return false;
        }

        if (typeInfo.IsAbstract)
        {
            return false;
        }

        if (typeInfo.ContainsGenericParameters)
        {
            return false;
        }

        if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
        {
            return false;
        }

        return typeInfo.Name.EndsWith("Controllers", StringComparison.OrdinalIgnoreCase) ||
            typeInfo.IsDefined(typeof(ControllerAttribute));
    }
}