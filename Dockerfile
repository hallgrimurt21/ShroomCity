# Use the .NET Core SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:7.0.404 AS build

# Set the working directory
WORKDIR /app

# Copy csproj and restore dependencies
COPY ShroomCity.API/*.csproj ./ShroomCity.API/
COPY ShroomCity.Repositories/*.csproj ./ShroomCity.Repositories/
COPY ShroomCity.Services/*.csproj ./ShroomCity.Services/
COPY ShroomCity.Models/*.csproj ./ShroomCity.Models/
COPY ShroomCity.Utilities/*.csproj ./ShroomCity.Utilities/
RUN dotnet restore ./ShroomCity.API/ShroomCity.API.csproj

# Copy the rest of the files and build the app
COPY ShroomCity.API/ ./ShroomCity.API/
COPY ShroomCity.Repositories/ ./ShroomCity.Repositories/
COPY ShroomCity.Services/ ./ShroomCity.Services/
COPY ShroomCity.Models/ ./ShroomCity.Models/
COPY ShroomCity.Utilities/ ./ShroomCity.Utilities/
WORKDIR /app/ShroomCity.API
RUN dotnet publish -c Release -o out

# Use the .NET Core runtime for running the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0.14 AS runtime

# Set the working directory
WORKDIR /app

# Copy the build output from the build image
COPY --from=build /app/ShroomCity.API/out ./

# Set the entry point for the Docker container
ENTRYPOINT ["dotnet", "ShroomCity.API.dll"]
