
StockAssist.Web — Models & DataContext Pack

1) Скопіюй папки `Models` і `Data` у корінь свого проекту StockAssist.Web.
2) Переконайся, що у Program.cs підключено Identity + DbContext (SQLite).
3) Виконай міграції:
   dotnet ef migrations add InitOrdersPayments
   dotnet ef database update
4) Запусти: dotnet run
