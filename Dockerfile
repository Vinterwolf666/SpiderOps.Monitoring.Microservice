#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Customer.Monitoring.Microservice.API/Customer.Monitoring.Microservice.API.csproj", "Customer.Monitoring.Microservice.API/"]
COPY ["Customer.Monitoring.Microservice.App/Customer.Monitoring.Microservice.App.csproj", "Customer.Monitoring.Microservice.App/"]
COPY ["Customer.Microservice/Customer.Monitoring.Microservice.Domain.csproj", "Customer.Microservice/"]
COPY ["Customer.Monitoring.Microservice.Infrastructure/Customer.Monitoring.Microservice.Infrastructure.csproj", "Customer.Monitoring.Microservice.Infrastructure/"]
COPY ["Customer.Monitoring.Microservice.Services/Customer.Monitoring.Microservice.Services.csproj", "Customer.Monitoring.Microservice.Services/"]
RUN dotnet restore "Customer.Monitoring.Microservice.API/Customer.Monitoring.Microservice.API.csproj"
COPY . .
WORKDIR "/src/Customer.Monitoring.Microservice.API"
RUN dotnet build "Customer.Monitoring.Microservice.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Customer.Monitoring.Microservice.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.Monitoring.Microservice.API.dll"]