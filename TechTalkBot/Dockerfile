FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TechTalkBot.csproj", "TechTalkBot/"]
RUN dotnet restore "TechTalkBot/TechTalkBot.csproj"
COPY . TechTalkBot
WORKDIR "/src/TechTalkBot"
RUN dotnet build "TechTalkBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechTalkBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechTalkBot.dll"]
