using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public partial class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El nombre solo puede contener letras.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "Los apellidos solo pueden contener letras.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 100 caracteres.")]
        public string Apellidos { get; set; } = null!;

        [Required(ErrorMessage = "El contacto es obligatorio.")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "Debe contener exactamente 8 dígitos numéricos.")]
        public string? Contacto { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria.")]
        public DateOnly FechaRegistro { get; set; }

        public bool Estado { get; set; }

        // =====================================================
        // 🔗 RELACIONES (NAVEGACIÓN)
        // =====================================================

        public virtual ICollection<Cita> Cita { get; set; } = new HashSet<Cita>();

       // public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

        public virtual ICollection<FeedbackCliente> FeedbackCliente { get; set; } = new List<FeedbackCliente>();

        public virtual ICollection<InscripcionCurso> InscripcionCurso { get; set; } = new List<InscripcionCurso>();

        public virtual ICollection<ResultadoExamen> ResultadoExamen { get; set; } = new List<ResultadoExamen>();

        public virtual ICollection<SesionPractica> SesionPractica { get; set; } = new List<SesionPractica>();

        // Si tu sistema tiene usuarios asociados al cliente (como antes)
        public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
    }
}
