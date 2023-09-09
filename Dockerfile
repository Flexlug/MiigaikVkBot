#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["/src/MiigaikVkBot/MiigaikVkBot.csproj", "MiigaikVkBot/"]
RUN dotnet restore "MiigaikVkBot/MiigaikVkBot.csproj"
COPY ./src /src
WORKDIR "/src/MiigaikVkBot"
RUN dotnet build "MiigaikVkBot.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runner
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "MiigaikVkBot.dll"]