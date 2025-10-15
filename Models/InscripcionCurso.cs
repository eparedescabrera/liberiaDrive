using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class InscripcionCurso
{
    public int IdInscripcion { get; set; }

    public int IdCliente { get; set; }

    public int IdCurso { get; set; }

    public DateOnly FechaInscripcion { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Curso IdCursoNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
