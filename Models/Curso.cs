using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiberiaDriveMVC.Models
{
    public class Curso
    {
        [Key]
        public int IdCurso { get; set; }

        [Required(ErrorMessage = "El tipo de curso es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Tipo de Curso")]
        public string TipoCurso { get; set; } = string.Empty;

        [Display(Name = "Duración (horas)")]
        [Range(1, 2000, ErrorMessage = "La duración debe ser mayor a 0")]
        public int Duracion { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 9999999, ErrorMessage = "El costo debe ser un valor positivo")]
        [Display(Name = "Costo (₡)")]
        public decimal Costo { get; set; }
    }
}
