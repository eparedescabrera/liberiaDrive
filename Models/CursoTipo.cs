using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public class CursoTipo
    {
        [Key]
        public int IdCursoTipo { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de curso es obligatorio.")]
        [StringLength(50, ErrorMessage = "Debe tener menos de 50 caracteres.")]
        public string NombreCursoTipo { get; set; } = null!;
        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();

    }
}
