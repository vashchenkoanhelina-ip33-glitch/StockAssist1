using Microsoft.AspNetCore.Mvc;

namespace StockAssist.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Contacts() => RedirectToAction("Create", "Contact");

        public IActionResult History()
        {
            throw new NotImplementedException();
        }
    }
}
