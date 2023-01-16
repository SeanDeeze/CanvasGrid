# escape=` 

FROM node:12.7-alpine AS build

    WORKDIR /source

    # Prevent 'Warning: apt-key output should not be parsed (stdout is not a terminal)'
    ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=1
    
    #RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
    #RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
    #RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
    #RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

    # install NodeJS 16.x
    # see https://github.com/nodesource/distributions/blob/master/README.md#deb
    RUN apt-get update -yq 
    RUN apt-get install curl gnupg -yq 
    RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
    RUN apt-get install -y nodejs
    
    # update npm
    RUN npm install -g npm@9.3.0

    # Copy only angular build files to source folder on image
    COPY ./CanvasGridAPI/CanvasGridAPI/CanvasGridUI/package.json /source/package.json
    RUN npm install --force

    COPY ./CanvasGridAPI/CanvasGridAPI/CanvasGridUI/. /source/
    RUN npm run-script compile

    RUN find -type d -exec chmod +w {} +

# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

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

ENV ASPNETCORE_URLS=http://+:80/
EXPOSE 80
ENTRYPOINT ["dotnet", "CanvasGrid.dll"]
