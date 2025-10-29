using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiberiaDriveMVC.Models
{
    public class Cita
    {
        [Key]
        public int IdCita { get; set; }

        [Required]
        [ForeignKey("Cliente")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El tipo de examen es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Tipo de Examen")]
        public string TipoExamen { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de cita es obligatoria")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaCita { get; set; }


        [StringLength(50)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; }

        // 🔗 Propiedad de navegación hacia Cliente
        public virtual Cliente? IdClienteNavigation { get; set; }
    }
}
