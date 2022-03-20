FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["PortEval.Application/PortEval.Application.csproj", "PortEval.Application/"]
COPY ["PortEval.Infrastructure/PortEval.Infrastructure.csproj", "PortEval.Infrastructure/"]
COPY ["PortEval.Domain/PortEval.Domain.csproj", "PortEval.Domain/"]
COPY ["PortEval.Application.Services/PortEval.Application.Services.csproj", "PortEval.Application.Services/"]
COPY ["PortEval.Application.Models/PortEval.Application.Models.csproj", "PortEval.Application.Models/"]
COPY ["PortEval.BackgroundJobs/PortEval.BackgroundJobs.csproj", "PortEval.BackgroundJobs/"]
COPY ["PortEval.FinancialDataFetcher/PortEval.FinancialDataFetcher.csproj", "PortEval.FinancialDataFetcher/"]

RUN dotnet restore "PortEval.Application/PortEval.Application.csproj"
COPY . .
RUN chmod +x ./wait-for-it.sh
WORKDIR "/app/PortEval.Application"
RUN dotnet build "PortEval.Application.csproj" -c Debug -o /app/build
RUN mkdir /app/files

FROM build AS publish
RUN dotnet publish "PortEval.Application.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/wait-for-it.sh .
ENTRYPOINT ["./wait-for-it.sh", "porteval-db:1433", "--timeout=120", "--", "dotnet", "PortEval.Application.dll"]
