using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class Vehiculo
{
    public int IdVehiculo { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Transmision { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<MantenimientoVehiculo> MantenimientoVehiculo { get; set; } = new List<MantenimientoVehiculo>();

    public virtual ICollection<SesionPractica> SesionPractica { get; set; } = new List<SesionPractica>();
}
