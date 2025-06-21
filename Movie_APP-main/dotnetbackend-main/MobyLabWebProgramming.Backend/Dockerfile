FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MobyLabWebProgramming.Backend.csproj", "."]
RUN dotnet restore "MobyLabWebProgramming.Backend.csproj"
COPY . .
RUN dotnet build "MobyLabWebProgramming.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MobyLabWebProgramming.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MobyLabWebProgramming.Backend.dll"]
