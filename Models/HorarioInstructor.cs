using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class HorarioInstructor
{
    public int IdHorario { get; set; }

    public int IdInstructor { get; set; }

    public string DiaSemana { get; set; } = null!;

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public bool? Disponible { get; set; }

    public virtual Instructor IdInstructorNavigation { get; set; } = null!;
}
