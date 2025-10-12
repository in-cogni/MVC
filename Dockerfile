FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ContosoUniversityHW/ContosoUniversityHW.csproj", "ContosoUniversityHW/"]
COPY ["ContosoUniversity/ContosoUniversity.csproj", "ContosoUniversity/"]

RUN dotnet restore "ContosoUniversityHW/ContosoUniversityHW.csproj"

COPY . .

WORKDIR "/src/ContosoUniversityHW"
RUN dotnet build "ContosoUniversityHW.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContosoUniversityHW.csproj" -c Release -o /app/publish

RUN dotnet tool install --global dotnet-ef
RUN dotnet ef migrations bundle --self-contained -r linux-x64 -o /app/efbundle

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/efbundle .

RUN chmod +x ./efbundle

CMD ["sh", "-c", "./efbundle --connection \"$ConnectionStrings__DatabaseConnection1\" && dotnet ContosoUniversityHW.dll"]