using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiberiaDriveMVC.Models
{
    public partial class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una inscripción.")]
        [ForeignKey(nameof(IdInscripcionNavigation))]
        public int IdInscripcion { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
        public DateOnly FechaPago { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [StringLength(50)]
        public string? TipoPago { get; set; }

        [StringLength(20)]
        [RegularExpression("^(Completado|Pendiente)$", 
            ErrorMessage = "El estado debe ser 'Completado' o 'Pendiente'.")]
        public string? Estado { get; set; }

        // 🔗 Relación con InscripcionCurso
        public virtual InscripcionCurso IdInscripcionNavigation { get; set; } = null!;
    }
}
