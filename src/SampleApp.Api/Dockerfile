#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SampleApp.Api/SampleApp.Api.csproj", "SampleApp.Api/"]
COPY ["SampleApp.Core/SampleApp.Core.csproj", "SampleApp.Core/"]
RUN dotnet restore "SampleApp.Api/SampleApp.Api.csproj"
COPY . .
WORKDIR "/src/SampleApp.Api"
RUN dotnet build "SampleApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampleApp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleApp.Api.dll"]