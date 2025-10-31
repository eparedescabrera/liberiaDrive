using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LiberiaDriveMVC.Models
{
    public partial class InscripcionCurso
    {
        [Key]
        public int IdInscripcion { get; set; }

        // ==============================================
        // ✅ CLIENTE
        // ==============================================
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        // ==============================================
        // ✅ CURSO
        // ==============================================
        [Required(ErrorMessage = "Debe seleccionar un curso.")]
        [Display(Name = "Curso")]
        public int IdCurso { get; set; }

        // ==============================================
        // ✅ FECHA INSCRIPCIÓN
        // ==============================================
        [Required(ErrorMessage = "Debe indicar la fecha de inscripción.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inscripción")]
        public DateOnly FechaInscripcion { get; set; }

        // ==============================================
        // 🔗 RELACIONES DE NAVEGACIÓN (IGNORADAS EN VALIDACIÓN)
        // ==============================================
        [ValidateNever]
        public virtual Cliente IdClienteNavigation { get; set; } = null!;

        [ValidateNever]
        public virtual Curso IdCursoNavigation { get; set; } = null!;

        [ValidateNever]
        public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
    }
}
