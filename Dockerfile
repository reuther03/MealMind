ARG DOTNET_VERSION=10.0
ARG BUILD_CONFIGURATION=Release

# ---- Build stage (has SDK) ----
FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION AS build
WORKDIR /src
COPY src/ ./
WORKDIR /src/Bootstrapper/MealMind.Bootstrapper
RUN dotnet publish MealMind.Bootstrapper.csproj -c $"BUILD_CONFIGURATION" -o /app/publish

# ---- Runtime stage (no SDK) ----
FROM mcr.microsoft.com/dotnet/aspnet:$DOTNET_VERSION AS runtime
WORKDIR /app
EXPOSE 8080
# Optional: expose and configure port
# ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MealMind.Bootstrapper.dll"]