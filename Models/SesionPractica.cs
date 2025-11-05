using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LiberiaDriveMVC.Models
{
    public partial class SesionPractica
    {
        [Key]
        public int IdSesionPractica { get; set; }

        // =======================
        // 🔹 CLAVE FORÁNEA: Cliente
        // =======================
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        // =======================
        // 🔹 CLAVE FORÁNEA: Instructor
        // =======================
        [Required(ErrorMessage = "Debe seleccionar un instructor.")]
        [Display(Name = "Instructor")]
        public int IdInstructor { get; set; }

        // =======================
        // 🔹 CLAVE FORÁNEA: Vehículo
        // =======================
        [Required(ErrorMessage = "Debe seleccionar un vehículo.")]
        [Display(Name = "Vehículo")]
        public int IdVehiculo { get; set; }

        // =======================
        // 🔹 Fecha de la sesión
        // =======================
        [Required(ErrorMessage = "Debe indicar la fecha de la sesión.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de sesión")]
        public DateOnly FechaSesion { get; set; }

        // =======================
        // 🔹 Estado (Programada / Cancelada)
        // Nota: tu SP ya usa 'Programada' como valor por defecto.
        // =======================
        [Required(ErrorMessage = "Debe indicar el estado.")]
        [StringLength(30)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Programada";

        // =======================
        // 🔗 Navegaciones (no validar en el binder)
        // =======================
        [ValidateNever]
        public virtual Cliente IdClienteNavigation { get; set; } = null!;

        [ValidateNever]
        public virtual Instructor IdInstructorNavigation { get; set; } = null!;

        [ValidateNever]
        public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
    }
}
