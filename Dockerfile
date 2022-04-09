#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["StudBot-longpoll/StudBot-longpoll.csproj", "StudBot-longpoll/"]
RUN dotnet restore "StudBot-longpoll/StudBot-longpoll.csproj"
COPY . .
WORKDIR "/src/StudBot-longpoll"
RUN dotnet build "StudBot-longpoll.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudBot-longpoll.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudBot-longpoll.dll"]