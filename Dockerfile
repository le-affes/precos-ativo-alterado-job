FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY publish/ .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["sh", "-c", "test -n \"$APP_DLL\" || (echo 'APP_DLL não informado' && exit 1); dotnet \"$APP_DLL\""]
