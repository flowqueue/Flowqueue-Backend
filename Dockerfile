FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Flowqueue-Backend/Flowqueue-Backend.csproj", "Flowqueue-Backend/"]
RUN dotnet restore "Flowqueue-Backend/Flowqueue-Backend.csproj"

COPY . .
WORKDIR "/src/Flowqueue-Backend"
RUN dotnet publish "Flowqueue-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Flowqueue-Backend.dll"]