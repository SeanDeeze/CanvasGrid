# escape=` 

 FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base

    WORKDIR /source

    # Prevent 'Warning: apt-key output should not be parsed (stdout is not a terminal)'
    ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=1

    # install NodeJS 13.x
    # see https://github.com/nodesource/distributions/blob/master/README.md#deb
    RUN apt-get update -yq 
    RUN apt-get install curl gnupg -yq 
    RUN curl -sL https://deb.nodesource.com/setup_13.x | bash -
    RUN apt-get install -y nodejs

    COPY ./CanvasGridAPI/CanvasGridAPI/CanvasGridUI/package.json /source/package.json
    RUN npm install
    RUN npm i typescript@3.8
    #RUN npm install -g @angular/cli@7.3.9

    COPY ./CanvasGridAPI/CanvasGridAPI/CanvasGridUI/. /source/
    RUN npm run-script compile

    RUN find -type d -exec chmod +w {} +

# https://hub.docker.com/_/microsoft-mssql-server
FROM mcr.microsoft.com/mssql/server:2019-CU8-ubuntu-16.04 AS sql-server

    RUN -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=QJyDHjjatZ6T3jwtP9Yr6SYj' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu

# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

    LABEL maintainer='davidseandunn@gmail.com'

    WORKDIR /source

    # Copy csproj and restore as distinct layers
    COPY ./CanvasGridAPI/CanvasGridAPI/*.csproj ./
    RUN dotnet restore

    # Copy everything else and build
    COPY ./CanvasGridAPI/CanvasGridAPI/. ./
    RUN dotnet publish -c Release -o canvasgridapi

    #run mkdir /canvasgrid/
    RUN cp -r ./canvasgridapi/. /canvasgrid/

    WORKDIR /canvasgrid/
    COPY --from=base /source/. ./CanvasGridUI/
    RUN ls

    EXPOSE 80
    EXPOSE 443
    EXPOSE 5000-5001
    EXPOSE 1433

ENTRYPOINT ["dotnet", "CanvasGrid.dll"]
