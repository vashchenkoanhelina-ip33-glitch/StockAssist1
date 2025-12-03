using Microsoft.AspNetCore.Identity;

namespace StockAssist.Web.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Role { get; set; }
	}
}
