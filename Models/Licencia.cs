using System;
using System.Collections.Generic;

namespace LiberiaDriveMVC.Models;

public partial class Licencia
{
    public int IdLicencia { get; set; }

    public string TipoLicencia { get; set; } = null!;

    public virtual ICollection<Instructor> IdInstructor { get; set; } = new List<Instructor>();
}
