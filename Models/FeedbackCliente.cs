using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class FeedbackCliente
{
    public int IdFeedback { get; set; }

    public int IdCliente { get; set; }

    public int IdCurso { get; set; }

    public int? Puntuacion { get; set; }

    public string? Comentario { get; set; }

    public DateOnly? FechaFeedback { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Curso IdCursoNavigation { get; set; } = null!;
}
