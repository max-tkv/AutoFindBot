#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AutoFindBot.Web/AutoFindBot.Web.csproj", "AutoFindBot.Web/"]
COPY ["AutoFindBot.Controllers/AutoFindBot.Controllers.csproj", "AutoFindBot.Controllers/"]
COPY ["AutoFindBot/AutoFindBot.csproj", "AutoFindBot/"]
COPY ["AutoFindBot.Utils/AutoFindBot.Utils.csproj", "AutoFindBot.Utils/"]
COPY ["AutoFindBot.Integration/AutoFindBot.Integration.csproj", "AutoFindBot.Integration/"]
COPY ["AutoFindBot.Storage/AutoFindBot.Storage.csproj", "AutoFindBot.Storage/"]
RUN dotnet restore "AutoFindBot.Web/AutoFindBot.Web.csproj"
COPY . .
WORKDIR "/src/AutoFindBot.Web"
RUN dotnet build "AutoFindBot.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoFindBot.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoFindBot.Web.dll"]