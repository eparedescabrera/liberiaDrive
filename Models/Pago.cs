using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdInscripcion { get; set; }

    public DateOnly FechaPago { get; set; }

    public decimal Monto { get; set; }

    public string? TipoPago { get; set; }

    public string? Estado { get; set; }

    public virtual InscripcionCurso IdInscripcionNavigation { get; set; } = null!;
}
