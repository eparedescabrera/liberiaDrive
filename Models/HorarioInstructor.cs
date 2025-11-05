using System;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public class HorarioInstructor
    {
        public int IdHorario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un instructor.")]
        public int IdInstructor { get; set; }

        [Required(ErrorMessage = "Debe ingresar el día de la semana.")]
        [MaxLength(15)]
        public string DiaSemana { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe especificar la hora de inicio.")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "Debe especificar la hora de fin.")]
        public TimeSpan HoraFin { get; set; }

        public bool Disponible { get; set; } = true;

        public virtual Instructor? IdInstructorNavigation { get; set; }
    }
}
