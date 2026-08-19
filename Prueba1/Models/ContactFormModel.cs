using System.ComponentModel.DataAnnotations;

namespace CommitSystemsMvc.Models;

public class ContactFormModel
{
    [Required(ErrorMessage = "Ingresa tu nombre.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu correo.")]
    [EmailAddress(ErrorMessage = "Ese correo no parece válido.")]
    [Display(Name = "Correo")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Tipo de proyecto")]
    public string ProjectType { get; set; } = "Desarrollo web";

    [Required(ErrorMessage = "Cuéntanos algo de tu proyecto.")]
    [Display(Name = "Mensaje")]
    public string Message { get; set; } = string.Empty;

    public static readonly string[] ProjectTypes =
    {
        "Desarrollo web",
        "Desarrollo de app",
        "Consultoría técnica",
        "Aún no lo sé",
    };
}
