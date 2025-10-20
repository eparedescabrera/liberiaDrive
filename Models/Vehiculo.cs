using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models
{
    public partial class Vehiculo
    {
        public int IdVehiculo { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "La marca solo puede contener letras.")]
        public string Marca { get; set; } = null!;

        [Required(ErrorMessage = "El modelo es obligatorio.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Debe tener entre 1 y 50 caracteres.")]
        public string Modelo { get; set; } = null!;

        [Required(ErrorMessage = "La transmisión es obligatoria.")]
        [RegularExpression(@"^(Manual|Automática|Automatica)$", ErrorMessage = "Debe ser 'Manual' o 'Automática'.")]
        public string Transmision { get; set; } = null!;

        // ✅ Cambiado de bool? a bool para evitar errores en Razor (checkbox)
        public bool Estado { get; set; } = true;

        // Relaciones
        public virtual ICollection<MantenimientoVehiculo> MantenimientoVehiculo { get; set; } = new List<MantenimientoVehiculo>();
        public virtual ICollection<SesionPractica> SesionPractica { get; set; } = new List<SesionPractica>();
    }
}
