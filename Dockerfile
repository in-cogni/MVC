FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "Academy/Academy.csproj"

WORKDIR "/src/Academy"
RUN dotnet build "Academy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Academy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Academy.dll"]