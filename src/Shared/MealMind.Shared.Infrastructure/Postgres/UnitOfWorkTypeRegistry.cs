using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Shared.Infrastructure.Postgres;

internal class UnitOfWorkTypeRegistry
{
    private readonly Dictionary<string, Type> _types = new();

    public void Register<T>() where T : IBaseUnitOfWork
        => _types[GetKey<T>()] = typeof(T);

    public Type? Resolve<T>()
        => _types.GetValueOrDefault(GetKey<T>());

    private static string GetKey<T>()
        => $"{typeof(T).GetModuleName()}";
}