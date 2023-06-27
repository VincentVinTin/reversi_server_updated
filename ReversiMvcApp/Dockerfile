#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update -y && apt-get install -y libc6-dev libgdiplus libx11-dev && apt-get clean && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ReversiMvcApp/ReversiMvcApp.csproj", "ReversiMvcApp/"]
RUN dotnet restore "ReversiMvcApp/ReversiMvcApp.csproj"
COPY . .
WORKDIR "/src/ReversiMvcApp"
RUN dotnet build "ReversiMvcApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReversiMvcApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReversiMvcApp.dll"]