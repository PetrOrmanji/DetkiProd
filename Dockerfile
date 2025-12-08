# Используем официальный образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проектов для восстановления зависимостей
COPY ["DetkiProd/DetkiProd.sln", "DetkiProd/"]
COPY ["DetkiProd/DetkiProd/DetkiProd.csproj", "DetkiProd/DetkiProd/"]
COPY ["DetkiProd/DetkiProd.Domain/DetkiProd.Domain.csproj", "DetkiProd/DetkiProd.Domain/"]
COPY ["DetkiProd/DetkiProd.Application/DetkiProd.Application.csproj", "DetkiProd/DetkiProd.Application/"]
COPY ["DetkiProd/DetkiProd.Infrastructure/DetkiProd.Infrastructure.csproj", "DetkiProd/DetkiProd.Infrastructure/"]

# Восстанавливаем пакеты
RUN dotnet restore "DetkiProd/DetkiProd/DetkiProd.csproj"

# Копируем все остальные файлы
COPY DetkiProd/ DetkiProd/

# Собираем проект
WORKDIR /src/DetkiProd/DetkiProd
RUN dotnet build "DetkiProd.csproj" -c Release -o /app/build

# Публикуем проект
FROM build AS publish
RUN dotnet publish "DetkiProd.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Используем runtime образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Копируем опубликованные файлы (теперь путь правильный)
COPY --from=publish /app/publish .

# Точка входа
ENTRYPOINT ["dotnet", "DetkiProd.dll"]