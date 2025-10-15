using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class Cita
{
    public int IdCita { get; set; }

    public int IdCliente { get; set; }

    public string TipoExamen { get; set; } = null!;

    public DateTime FechaCita { get; set; }

    public string? Estado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
