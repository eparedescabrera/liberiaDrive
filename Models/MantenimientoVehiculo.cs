using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class MantenimientoVehiculo
{
    public int IdMantenimiento { get; set; }

    public int IdVehiculo { get; set; }

    public DateOnly FechaMantenimiento { get; set; }

    public string? TipoMantenimiento { get; set; }

    public decimal? Costo { get; set; }

    public string? Descripcion { get; set; }

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
