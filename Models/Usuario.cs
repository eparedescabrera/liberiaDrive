using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public bool? Estado { get; set; }

    public int IdRol { get; set; }

    public int? IdCliente { get; set; }

    public int? IdInstructor { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Instructor? IdInstructorNavigation { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;
}
