FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiază toate .csproj-urile separat
COPY ["MobyLabWebProgramming.Backend/MobyLabWebProgramming.Backend.csproj", "MobyLabWebProgramming.Backend/"]
COPY ["MobyLabWebProgramming.Infrastructure/MobyLabWebProgramming.Infrastructure.csproj", "MobyLabWebProgramming.Infrastructure/"]
COPY ["MobyLabWebProgramming.Core/MobyLabWebProgramming.Core.csproj", "MobyLabWebProgramming.Core/"]

# 2. Restore
RUN dotnet restore "MobyLabWebProgramming.Backend/MobyLabWebProgramming.Backend.csproj"

# 3. Copiază restul surselor
COPY . .

WORKDIR "/src/MobyLabWebProgramming.Backend"
RUN dotnet build "MobyLabWebProgramming.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MobyLabWebProgramming.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MobyLabWebProgramming.Backend.dll"]
