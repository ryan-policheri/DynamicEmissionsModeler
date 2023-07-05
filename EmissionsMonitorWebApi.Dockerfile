# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /source
# copy all solution files
COPY ./ .

# restore as distinct layers
RUN dotnet restore ./EmissionsMonitorWebApi/EmissionsMonitorWebApi.csproj --use-current-runtime  

# build app
RUN dotnet publish ./EmissionsMonitorWebApi/EmissionsMonitorWebApi.csproj --use-current-runtime --self-contained false --no-restore -o /app


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .

# setup kestrel service to host app
RUN sudo systemctl start ServiceSpec.service
RUN sudo systemctl status ServiceSpec.service
RUN sudo systemctl enable /home/ryanpoli/dev_ops/ServiceSpec.service


#
RUN apt-get update
RUN apt-get install apache2 -y
RUN apt-get install apache2-utils -y
RUN apt-get clean


ENTRYPOINT ["dotnet", "EmissionsMonitorWebApi.dll"]


RUN apt-get update
RUN apt-get install apache2 -y
RUN apt-get install apache2-utils -y
RUN apt-get clean

CMD ["-D", "FOREGROUND"]
ENTRYPOINT ["/usr/sbin/apache2"]
EXPOSE 80