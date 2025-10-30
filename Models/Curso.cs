using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
   public partial class Curso
    {
        [Key]
        public int IdCurso { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de curso.")]
        public int IdCursoTipo { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria.")]
        [Range(1, 200, ErrorMessage = "La duración debe ser entre 1 y 200 horas.")]
        public int Duracion { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Range(1, 1000000, ErrorMessage = "El costo debe ser mayor a 0.")]
        public decimal Costo { get; set; }
        // ✅ Relación con tabla CursoTipo
         public virtual CursoTipo CursoTipo { get; set; } = null!;

        // ✅ Relaciones con otras tablas
        public virtual ICollection<FeedbackCliente> FeedbackCliente { get; set; } = new List<FeedbackCliente>();
        public virtual ICollection<InscripcionCurso> InscripcionCurso { get; set; } = new List<InscripcionCurso>();
    }
}
