FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim

RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get clean 

WORKDIR /app
RUN useradd -c 'Application User' -U -d /app -s /bin/bash appuser && \
    chown -R appuser:appuser /app
COPY --chown=appuser:appuser .build .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
USER appuser

# TODO: Change the dll name
CMD ["dotnet", "SE.Sustainability.Template.API.dll"]
