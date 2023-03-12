FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/' /etc/ssl/openssl.cnf
RUN sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/' /etc/ssl/openssl.cnf


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY . .

#RUN head -c 5 /dev/random > random_bytes && dotnet tool install --global dotnet-sonarscanner --version 5.8.0
#RUN head -c 5 /dev/random > random_bytes && export PATH="$PATH:/root/.dotnet/tools" 
#RUN head -c 5 /dev/random > random_bytes && dotnet tool list -g
#RUN head -c 5 /dev/random > random_bytes && dotnet sonarscanner begin /k:"ConfigServer" /d:sonar.host.url="http://10.110.100.61:9000"  #/d:sonar.login="300d8454578143292c3be6aaf0349a30e0ab82e1"

RUN echo $(ls -1 /src)

#RUN dotnet restore -s "http://10.110.100.61:9090/v3/index.json"  -s "https://api.nuget.org/v3/index.json" "my_api.csproj"

#WORKDIR "/src"
#RUN dotnet build "my_api.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "my_api.csproj" -c Release -o /app/publish

#RUN head -c 5 /dev/random > random_bytes && dotnet sonarscanner end /d:sonar.login=\"300d8454578143292c3be6aaf0349a30e0ab82e1\"

RUN echo $(ls -1 /app)

FROM base AS final
WORKDIR /src/app
COPY --from=publish /app/publish .

#RUN apt-get update

#RUN apt-get install curl -y

ENTRYPOINT ["dotnet", "my_api.dll"]	

