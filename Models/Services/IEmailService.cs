using System.Threading.Tasks;

namespace StockAssist.Web.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}