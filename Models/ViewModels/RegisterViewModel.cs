using System.ComponentModel.DataAnnotations;

namespace LiberiaDriveMVC.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Nombre de usuario")]
    [StringLength(100, ErrorMessage = "El nombre de usuario no puede exceder los {1} caracteres.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.")]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmacionContrasena { get; set; } = string.Empty;
}
