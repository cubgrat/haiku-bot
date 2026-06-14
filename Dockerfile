# --- сборка ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Сначала только csproj — для кэширования restore.
COPY src/HaikuBot.Domain/*.csproj src/HaikuBot.Domain/
COPY src/HaikuBot.Application/*.csproj src/HaikuBot.Application/
COPY src/HaikuBot.Infrastructure/*.csproj src/HaikuBot.Infrastructure/
COPY src/HaikuBot.Telegram/*.csproj src/HaikuBot.Telegram/
COPY src/HaikuBot.Host/*.csproj src/HaikuBot.Host/
RUN dotnet restore src/HaikuBot.Host/HaikuBot.Host.csproj

# Затем весь исходный код и публикация.
COPY src/ src/
RUN dotnet publish src/HaikuBot.Host/HaikuBot.Host.csproj -c Release -o /app --no-restore

# --- рантайм ---
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "HaikuBot.Host.dll"]
