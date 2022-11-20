#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.Payment.WandaPay/Tgs.Payment.WandaPay.csproj", "Tgs.Payment.WandaPay/"]
COPY ["WandaPay.Core/WandaPay.Core.csproj", "WandaPay.Core/"]
COPY ["WandaPay.Client/WandaPay.Client.csproj", "WandaPay.Client/"]
RUN dotnet restore "Tgs.Payment.WandaPay/Tgs.Payment.WandaPay.csproj"
COPY . .
WORKDIR "/src/Tgs.Payment.WandaPay"
RUN dotnet build "Tgs.Payment.WandaPay.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Payment.WandaPay.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Payment.WandaPay.dll"]