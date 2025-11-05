using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models;

public partial class MantenimientoVehiculo
{
    public int IdMantenimiento { get; set; }

    [Required]
    public int IdVehiculo { get; set; }

    [Required]
    public DateOnly FechaMantenimiento { get; set; }

    [Required, StringLength(100)]
    public string TipoMantenimiento { get; set; } = null!;

    [Required, Range(0, 1000000)]
    public decimal Costo { get; set; }

    public string? Descripcion { get; set; }

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
