FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Microservice/Microservice.csproj", "Microservice/"]
RUN dotnet restore "Microservice/Microservice.csproj"
COPY . .
WORKDIR "/src/Microservice"
RUN dotnet build "Microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Microservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservice.dll"]
