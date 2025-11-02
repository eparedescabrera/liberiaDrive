using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LiberiaDriveMVC.Models
{
    public partial class ResultadoExamen
    {
        [Key]
        public int IdResultado { get; set; }

        // ==============================================
        // ✅ CLIENTE
        // ==============================================
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Alumno")]
        public int IdCliente { get; set; }

        // ==============================================
        // ✅ TIPO DE EXAMEN (Teórico o Práctico)
        // ==============================================
        [Required(ErrorMessage = "Debe indicar el tipo de examen.")]
        [StringLength(50, ErrorMessage = "El tipo de examen no debe superar los 50 caracteres.")]
        [Display(Name = "Tipo de examen")]
        public string TipoExamen { get; set; } = null!;

        // ==============================================
        // ✅ FECHA DEL EXAMEN
        // ==============================================
        [Required(ErrorMessage = "Debe indicar la fecha del examen.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del examen")]
        public DateOnly FechaExamen { get; set; }

        // ==============================================
        // ✅ RESULTADO (Aprobado / Reprobado)
        // ==============================================
        [Display(Name = "Aprobado")]
        public bool Aprobado { get; set; }

        // ==============================================
        // ✅ INSTRUCTOR (solo para exámenes prácticos)
        // ==============================================
        [Display(Name = "Instructor responsable")]
        public int? IdInstructor { get; set; }

        // ==============================================
        // 🔗 RELACIONES DE NAVEGACIÓN (para compatibilidad futura)
        // ==============================================
        [ValidateNever]
        public virtual Cliente? IdClienteNavigation { get; set; }

        [ValidateNever]
        public virtual Instructor? IdInstructorNavigation { get; set; }
    }
}
