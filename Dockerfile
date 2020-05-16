FROM mcr.microsoft.com/dotnet/core/sdk:latest AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://*:5000

FROM base AS builder
ARG Configuration=Release
WORKDIR /src
COPY *.sln ./
COPY ./Authentication/*.csproj Authentication/
COPY ./AuthenticationDataAccess/*.csproj AuthenticationDataAccess/
COPY ./AuthenticationDataAccessModel/*.csproj AuthenticationDataAccessModel/
COPY ./AuthenticationDataTransferModel/*.csproj AuthenticationDataTransferModel/
COPY ./AuthenticationErrorHandling/*.csproj  AuthenticationErrorHandling/
COPY ./AuthenticationManager/*.csproj AuthenticationManager/
RUN dotnet restore --verbosity detailed
COPY . .
WORKDIR /src/Authentication
RUN dotnet build -c $Configuration -o /app

FROM builder AS publish
ARG Configuration=Release
RUN dotnet publish -c $Configuration -o /app

FROM base as final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Authentication.dll"]

EXPOSE 5000
