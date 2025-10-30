using System;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public partial class Cita
    {
        [Key]
        public int IdCita { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public int IdCliente { get; set; }  // 👈 sigue existiendo para inserts/updates

        [Display(Name = "Cliente")]
        public string? NombreCliente { get; set; }  // 👈 se llena al listar o editar

        [Required(ErrorMessage = "Debe indicar el tipo de examen.")]
        [StringLength(50)]
        public string TipoExamen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe indicar la fecha de la cita.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de la cita")]
        public DateTime FechaCita { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        public virtual Cliente? IdClienteNavigation { get; set; }
    }
}
