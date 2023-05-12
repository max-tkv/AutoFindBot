# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["AutoFindBot.Web/AutoFindBot.Web.csproj", "AutoFindBot.Web/"]
COPY ["AutoFindBot.Controllers/AutoFindBot.Controllers.csproj", "AutoFindBot.Controllers/"]
COPY ["AutoFindBot/AutoFindBot.csproj", "AutoFindBot/"]
COPY ["AutoFindBot.Utils/AutoFindBot.Utils.csproj", "AutoFindBot.Utils/"]
COPY ["AutoFindBot.Integration.Avito/AutoFindBot.Integration.Avito.csproj", "AutoFindBot.Integration.Avito/"]
COPY ["AutoFindBot.Integration.KeyAutoProbeg/AutoFindBot.Integration.KeyAutoProbeg.csproj", "AutoFindBot.Integration.KeyAutoProbeg/"]
COPY ["AutoFindBot.Integration.TradeDealer/AutoFindBot.Integration.TradeDealer.csproj", "AutoFindBot.Integration.TradeDealer/"]
COPY ["AutoFindBot.Storage/AutoFindBot.Storage.csproj", "AutoFindBot.Storage/"]

RUN dotnet restore "AutoFindBot.Web/AutoFindBot.Web.csproj"

COPY . .
WORKDIR "/src/AutoFindBot.Web"
RUN dotnet build "AutoFindBot.Web.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "AutoFindBot.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install Chrome and dependencies
RUN apt-get update && apt-get install -y curl gnupg2 libgconf-2-4
RUN curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list
RUN apt-get update && apt-get install -y google-chrome-stable

# Copy ChromeDriver
COPY AutoFindBot/Resources/WebDrivers/Linux/chromedriver /usr/local/bin/chromedriver
RUN chmod +x /usr/local/bin/chromedriver
    
# Set up Chrome for headless mode
ENV DISPLAY=:99
RUN apt-get install -y xvfb
RUN Xvfb :99 -screen 0 1024x768x16 &

RUN sudo echo "Europe/Moscow" > /etc/timezone
RUN sudo dpkg-reconfigure -f noninteractive tzdata

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AutoFindBot.Web.dll", "--urls", "http://0.0.0.0:80"]