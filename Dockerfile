FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /app

# copy csproj and restore as distinct layers
# the reason for that is to chache the dependencies as layers, so later on if we change the code and rebuilt the image, we don't have to restore the dependencies again as they will be cached in layers
COPY "GatherSpot.API.sln" .
COPY "GatherSpot.API/GatherSpot.API.csproj" "GatherSpot.API/"
COPY "Application/Application.csproj" "Application/"
COPY "Infrastructure/Infrastructure.csproj" "Infrastructure/"
COPY "Persistence/Persistence.csproj" "Persistence/"
COPY "Domain/Domain.csproj" "Domain/"

RUN dotnet restore "GatherSpot.API.sln"

# copy everything else and build app
COPY . .
RUN dotnet publish -c Release -o out

# build runtime image
# this is the final image that will be used to run the application
# this image is smaller than the sdk image
# the sdk image is used to build the application
# the aspnet image is used to run the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "GatherSpot.API.dll" ]