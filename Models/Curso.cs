using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class Curso
{
    public int IdCurso { get; set; }

    public string TipoCurso { get; set; } = null!;

    public int Duracion { get; set; }

    public decimal Costo { get; set; }

    public virtual ICollection<FeedbackCliente> FeedbackCliente { get; set; } = new List<FeedbackCliente>();

    public virtual ICollection<InscripcionCurso> InscripcionCurso { get; set; } = new List<InscripcionCurso>();
}
