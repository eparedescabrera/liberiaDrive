using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class Instructor
{
    public int IdInstructor { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public bool? Estado { get; set; }

    public virtual ICollection<HorarioInstructor> HorarioInstructor { get; set; } = new List<HorarioInstructor>();

    public virtual ICollection<ResultadoExamen> ResultadoExamen { get; set; } = new List<ResultadoExamen>();

    public virtual ICollection<SesionPractica> SesionPractica { get; set; } = new List<SesionPractica>();

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();

    public virtual ICollection<Licencia> IdLicencia { get; set; } = new List<Licencia>();
}
