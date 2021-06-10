#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["hvmbot/hvmbot.csproj", "hvmbot/"]
RUN dotnet restore "hvmbot/hvmbot.csproj"
COPY . .
WORKDIR "/src/hvmbot"
RUN dotnet build "hvmbot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "hvmbot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "hvmbot.dll"]