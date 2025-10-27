using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models;

public partial class Licencia
{
    public int IdLicencia { get; set; }

        [Required(ErrorMessage = "El tipo de licencia es obligatorio.")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
        public string TipoLicencia { get; set; } = null!;
    public virtual ICollection<InstructorLicencia> InstructorLicencias { get; set; } = new List<InstructorLicencia>();


    public virtual ICollection<Instructor> IdInstructor { get; set; } = new List<Instructor>();
}
