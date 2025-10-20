using System;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public class TokenRecuperacion
    {
        [Key]
        public int IdToken { get; set; }

        [Required, StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime FechaExpira { get; set; }

        public bool Usado { get; set; } = false;
    }
}
