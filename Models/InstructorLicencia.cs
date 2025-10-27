using System.ComponentModel.DataAnnotations.Schema;

namespace LiberiaDriveMVC.Models
{
    public partial class InstructorLicencia
    {
        [ForeignKey("Instructor")]
        public int IdInstructor { get; set; }

        [ForeignKey("Licencia")]
        public int IdLicencia { get; set; }

        public virtual Instructor Instructor { get; set; } = null!;
        public virtual Licencia Licencia { get; set; } = null!;
    }
}
