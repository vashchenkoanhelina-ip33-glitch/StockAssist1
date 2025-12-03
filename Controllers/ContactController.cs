using Microsoft.AspNetCore.Mvc;
using StockAssist.Web.Data;
using StockAssist.Web.Models;
using StockAssist.Web.Services;

namespace StockAssist.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _mail;

        public ContactController(ApplicationDbContext db, IEmailService mail)
        {
            _db = db;
            _mail = mail;
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name, string email, string subject, string body)
        {
            if (!ModelState.IsValid) return View();

            var msg = new Contact
            {
                Name = name,
                Email = email,
                Subject = subject,
                Message = body,
                CreatedAt = DateTime.UtcNow
            };

            _db.Contacts.Add(msg);
            await _db.SaveChangesAsync();

            await _mail.SendAsync("support@example.com", $"Повідомлення: {subject}",
                $"<p><b>Від:</b> {name} ({email})</p><p>{body}</p>");

            ViewBag.Success = "Ваше повідомлення успішно надіслано!";
            return View();
        }
    }
}