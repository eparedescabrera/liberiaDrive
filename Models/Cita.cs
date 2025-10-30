using System;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public partial class Cita
    {
        [Key]
        public int IdCita { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Debe indicar el tipo de examen.")]
        public string TipoExamen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe indicar la fecha de la cita.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCita { get; set; }

        [Required]
        public string Estado { get; set; } = "Pendiente";

        // ⚙️ Mantener navegación para EF Core, aunque no la uses en SP
        public virtual Cliente IdClienteNavigation { get; set; } = null!;
    }
}
