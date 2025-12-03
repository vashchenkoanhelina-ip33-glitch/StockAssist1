using System;
using System.ComponentModel.DataAnnotations;

namespace StockAssist.Web.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        
        [Required, MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(2048)]
        public string MessageText { get; set; } = string.Empty;

        [Required, MaxLength(128)]
        public string Subject { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Processed { get; set; } = false;
    }
}