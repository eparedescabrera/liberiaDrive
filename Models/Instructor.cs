using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public partial class Instructor
    {
        [Key]
        public int IdInstructor { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El nombre solo puede contener letras.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "Los apellidos solo pueden contener letras.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 100 caracteres.")]
        public string Apellidos { get; set; } = null!;

        public bool? Estado { get; set; } = true;

        // 🔹 Relaciones (navegación con otras tablas)
        public virtual ICollection<InstructorLicencia> InstructorLicencias { get; set; } = new List<InstructorLicencia>();

        public virtual ICollection<HorarioInstructor> HorarioInstructor { get; set; } = new List<HorarioInstructor>();

        public virtual ICollection<ResultadoExamen> ResultadoExamen { get; set; } = new List<ResultadoExamen>();

        public virtual ICollection<SesionPractica> SesionPractica { get; set; } = new List<SesionPractica>();

        public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();

        public virtual ICollection<Licencia> IdLicencia { get; set; } = new List<Licencia>();
    }
}
