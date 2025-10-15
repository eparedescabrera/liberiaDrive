using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class SesionPractica
{
    public int IdSesionPractica { get; set; }

    public int IdCliente { get; set; }

    public int IdInstructor { get; set; }

    public int IdVehiculo { get; set; }

    public DateOnly FechaSesion { get; set; }

    public string? Estado { get; set; }

    public decimal? Calificacion { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Instructor IdInstructorNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
