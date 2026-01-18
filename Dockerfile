FROM node:latest AS build
    WORKDIR /source

    RUN npm install -g npm@latest

    # Copy only angular build files to source folder on image
    COPY ./CanvasGridAPI/CanvasGridAPI/CanvasGridUI/package.json /source/package.json
    RUN npm install

    COPY ./CanvasGridAPI/CanvasGridAPI/CanvasGridUI/. /source/
    RUN npm run-script compile

    # RUN find -type d -exec chmod +w {} +

# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env

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
    COPY --from=build /source/. ./CanvasGridUI/
    RUN ls

ENV ASPNETCORE_URLS=http://+:80/
EXPOSE 80
ENTRYPOINT ["dotnet", "CanvasGrid.dll"]
