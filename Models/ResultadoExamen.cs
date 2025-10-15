using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class ResultadoExamen
{
    public int IdResultado { get; set; }

    public int IdCliente { get; set; }

    public string TipoExamen { get; set; } = null!;

    public DateOnly FechaExamen { get; set; }

    public bool Aprobado { get; set; }

    public int? IdInstructor { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Instructor? IdInstructorNavigation { get; set; }
}
