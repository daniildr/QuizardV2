# Quizard

[Запуск Игры](GAME_START.md)

[Формирование Сценария](SCENARIO.md)

[Интеграция с SignalR](SIGNALR.md)

[Схема переходов состояний](FSM.md)

[Логирование](LOGGING.md)

[Чек-лист соответствия ТЗ](CHECKLIST.md)

## Установка и настройка
### Предварительные требования
#### .NET Runtime 9.0 
Приложение собрано как standalone пакет, но на всякий случай необходимо установить runtime.
Скачать и установить runtime:
https://dotnet.microsoft.com/en-us/download/dotnet/9.0
---

#### PostgreSQL
Самый простой способ развернуть докер контейнер:
``` bash
docker run --name quizard-postgres -e POSTGRES_DB=postgres -e POSTGRES_USER=quizard -e POSTGRES_PASSWORD='iGMgTs9K%fZP' -p 5432:5432 -v "$(pwd)/migration.sql:/docker-entrypoint-initdb.d/migration.sql:ro" -d postgres
```
↑ ↑ ↑ При запуске контейнера сразу будет выполнена миграция БД. ↑ ↑ ↑

↑ ↑ ↑ Выполнять команду необходимо из директории установки. ↑ ↑ ↑

Для установки в ручную или подключения другой базы необходимо сохранить конфигурацию БД и накатить миграции

**Конфигурация и креды БД:**
   - Host - {local ip|localhost|url и тд}:5432
   - User - quizard
   - DB - postgres
   - Password - iGMgTs9K%fZP

Для управления БД можно использовать любой удобный клиент.
Самый простой способ развернуть pgAdmin (креды admin@admin.com/admin): 
``` bash 
docker run --name pgadmin -e PGADMIN_DEFAULT_EMAIL=admin@admin.com -e PGADMIN_DEFAULT_PASSWORD=admin -p 8080:80 -d dpage/pgadmin4
```
---

#### Конфигурация сети
Для доступа к приложению необходимо открыть порт 5009 для внешнего подключения на хост машине:
1. Запустите «Брандмауэр Windows с расширенной безопасностью».  
2. Создайте правило входящего подключения:     
   - Тип: порт
   - Протокол: TCP
   - Локальный порт: 5009
   - Применить к профилям (Domain/Private и тд)
3. Для корректной работы рекомендуется зарезервировать ip адрес хоста приложения.
---

### Установка
Запустить установочный файл

---