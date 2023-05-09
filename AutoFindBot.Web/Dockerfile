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

# Install dependencies
RUN apt-get update && apt-get install -y curl gnupg2 xvfb unzip

# Install Chrome
RUN curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list
RUN apt-get update && apt-get install -y google-chrome-stable

# Install ChromeDriver
ARG CHROME_DRIVER_VERSION=113.0.5672.63
RUN curl -sSL https://chromedriver.storage.googleapis.com/${CHROME_DRIVER_VERSION}/chromedriver_linux64.zip -o /tmp/chromedriver.zip && \
    unzip /tmp/chromedriver.zip -d /usr/local/bin && \
    chmod +x /usr/local/bin/chromedriver && \
    rm /tmp/chromedriver.zip

# Set up Xvfb
RUN Xvfb :99 -ac -screen 0 1280x1024x16 +extension RANDR &

# Set environment variables
ENV DISPLAY=:99

# Copy the published application
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AutoFindBot.Web.dll"]