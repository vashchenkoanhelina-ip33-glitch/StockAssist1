# StockAssist (ASP.NET Core MVC, .NET 8)

Скелет веб‑застосунку для підтримки роботи складу згідно з ТЗ. 
Технології: ASP.NET Core MVC, Entity Framework Core (SQLite), Razor Views.

## Швидкий старт
1. Встанови .NET 8 SDK.
2. `cd StockAssist.Web`
3. `dotnet restore`
4. Додай міграцію: `dotnet tool install --global dotnet-ef` і `dotnet ef migrations add Init`.
5. `dotnet ef database update`
6. `dotnet run` і відкрий https://localhost:5001

## Основні можливості
- Реєстрація/логін, ролі (User/Operator/Admin).
- Створення замовлення (зберігання), оплата (симуляція), історія.
- Кабінет оператора (черга замовлень), кабінет адміністратора (користувачі, замовлення).
- Контактна форма.

## Примітки
- Це базовий каркас. Додай реальний платіжний шлюз, валідацію та міграції.
- Можна замінити SQLite на MSSQL у `appsettings.json` і `Program.cs`.
