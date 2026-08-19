using System.ComponentModel.DataAnnotations;
using System.Net;
using CommitSystemsMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Resend;

namespace CommitSystemsMvc.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _config;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IConfiguration config,
        ILogger<HomeController> logger)
    {
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(BuildViewModel(new ContactFormModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(HomeViewModel model)
    {
        var form = model.ContactForm;

        if (form == null)
        {
            TempData["ContactStatus"] = "error";
            TempData["ContactMessage"] =
                "No se recibieron los datos del formulario.";

            return RedirectToAction(
                nameof(Index),
                "Home",
                null,
                "contacto"
            );
        }

        var validationContext = new ValidationContext(form);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            form,
            validationContext,
            validationResults,
            true
        );

        if (!isValid)
        {
            foreach (var validationResult in validationResults)
            {
                _logger.LogWarning(
                    "Error de validación: {Error}",
                    validationResult.ErrorMessage
                );
            }

            TempData["ContactStatus"] = "error";
            TempData["ContactMessage"] =
                "Por favor, completa correctamente todos los campos.";

            return RedirectToAction(
                nameof(Index),
                "Home",
                null,
                "contacto"
            );
        }

        try
        {
            await SendContactEmailAsync(form);

            TempData["ContactStatus"] = "success";
            TempData["ContactMessage"] =
                "Tu mensaje fue enviado correctamente. Te responderemos dentro de 48 horas hábiles.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "ERROR REAL DE RESEND"
            );

            TempData["ContactStatus"] = "error";
            TempData["ContactMessage"] =
                $"Error Resend: {ex.Message}";
        }

        return RedirectToAction(
            nameof(Index),
            "Home",
            null,
            "contacto"
        );
    }

    private async Task SendContactEmailAsync(ContactFormModel form)
    {
        var settings = _config.GetSection("EmailSettings");

        var apiKey = settings["ResendApiKey"];
        var fromAddress = settings["FromAddress"];
        var toAddress = settings["ToAddress"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new Exception(
                "No está configurado EmailSettings:ResendApiKey."
            );
        }

        if (string.IsNullOrWhiteSpace(fromAddress))
        {
            throw new Exception(
                "No está configurado EmailSettings:FromAddress."
            );
        }

        if (string.IsNullOrWhiteSpace(toAddress))
        {
            throw new Exception(
                "No está configurado EmailSettings:ToAddress."
            );
        }

        _logger.LogInformation(
            "Configuración Resend encontrada. From: {From}, To: {To}",
            fromAddress,
            toAddress
        );

        IResend resend = ResendClient.Create(apiKey);

        var name = WebUtility.HtmlEncode(form.Name);
        var email = WebUtility.HtmlEncode(form.Email);
        var projectType = WebUtility.HtmlEncode(form.ProjectType);
        var message = WebUtility.HtmlEncode(form.Message);

        message = message
            .Replace("\r\n", "<br/>")
            .Replace("\n", "<br/>");

        var htmlBody = $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <title>Nuevo contacto - Commit Systems</title>
        </head>
        <body style="
            margin:0;
            padding:0;
            background:#f4f4f4;
            font-family:Arial,Helvetica,sans-serif;
        ">
            <div style="
                max-width:650px;
                margin:40px auto;
                background:#ffffff;
                border-radius:10px;
                overflow:hidden;
                box-shadow:0 2px 10px rgba(0,0,0,.08);
            ">
                <div style="
                    background:#111111;
                    color:white;
                    padding:25px;
                ">
                    <h1 style="
                        margin:0;
                        font-size:24px;
                    ">
                        Nuevo mensaje de contacto
                    </h1>

                    <p style="
                        margin:8px 0 0;
                        color:#cccccc;
                    ">
                        Commit Systems
                    </p>
                </div>

                <div style="
                    padding:30px;
                ">
                    <h2>
                        Datos del contacto
                    </h2>

                    <p>
                        <strong>Nombre:</strong><br>
                        {name}
                    </p>

                    <p>
                        <strong>Correo:</strong><br>
                        {email}
                    </p>

                    <p>
                        <strong>Tipo de proyecto:</strong><br>
                        {projectType}
                    </p>

                    <hr style="
                        border:none;
                        border-top:1px solid #eeeeee;
                        margin:25px 0;
                    ">

                    <h2>
                        Mensaje
                    </h2>

                    <p style="
                        line-height:1.6;
                        color:#333333;
                    ">
                        {message}
                    </p>
                </div>

                <div style="
                    background:#f8f8f8;
                    padding:20px 30px;
                    color:#777777;
                    font-size:13px;
                ">
                    Este correo fue enviado automáticamente
                    desde el formulario de contacto
                    de Commit Systems.
                </div>
            </div>
        </body>
        </html>
        """;

        var emailMessage = new EmailMessage
        {
            From = fromAddress,
            To = toAddress,
            Subject = $"Nuevo contacto — {form.ProjectType}",
            HtmlBody = htmlBody
        };

        emailMessage.ReplyTo = form.Email;

        _logger.LogInformation(
            "Enviando correo mediante Resend..."
        );

        await resend.EmailSendAsync(emailMessage);

        _logger.LogInformation(
            "Correo enviado correctamente mediante Resend."
        );
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true
    )]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            }
        );
    }

    private static HomeViewModel BuildViewModel(
        ContactFormModel form)
    {
        return new HomeViewModel
        {
            HeroEyebrow =
                "Desarrollador y consultores de software — San Felipe, Chile",

            HeroLead =
                "Somos un equipo de desarrollo web y de aplicaciones con base en San Felipe, Valparaíso. " +
                "Trabajamos con pequeñas y medianas empresas que necesitan algo que funcione de verdad — " +
                "no una demo bonita que nadie mantiene.",

            Services = new List<ServiceItem>
            {
                new(
                    "01",
                    "Desarrollo Web",
                    "Sitios y sistemas web a medida: landing pages, paneles de administración, plataformas internas y e-commerce. Rápidos, responsivos y fáciles de mantener.",
                    new[]
                    {
                        "React",
                        "Next.js",
                        "Astro",
                        "ASP.NET"
                    }
                ),

                new(
                    "02",
                    "Desarrollo de Apps",
                    "Aplicaciones móviles y de escritorio conectadas a tu backend, pensadas para uso real: menos pantallas, más flujo. Del prototipo al lanzamiento.",
                    new[]
                    {
                        "C#",
                        "Java",
                        "TypeScript"
                    }
                ),

                new(
                    "03",
                    "Consultoría técnica",
                    "Revisión de arquitectura, elección de stack, migración de datos o auditoría de un sistema existente. Ideal si ya tienes algo y no sabes por qué falla.",
                    new[]
                    {
                        "SQL Server",
                        "Supabase",
                        "Oracle"
                    }
                )
            },

            Process = new List<ProcessStep>
            {
                new(
                    "01",
                    "Levantamiento",
                    "Conversamos sobre el problema real, no solo la solución que imaginas. Definimos alcance, plazos y presupuesto en una reunión sin costo."
                ),

                new(
                    "02",
                    "Arquitectura",
                    "Proponemos el stack y la estructura del sistema, con foco en que sea mantenible por ustedes o por otro equipo el día de mañana."
                ),

                new(
                    "03",
                    "Desarrollo",
                    "Entregas parciales cada semana para que veas avances reales, no un informe de estado. Ajustamos sobre la marcha."
                ),

                new(
                    "04",
                    "Entrega y soporte",
                    "Despliegue, documentación y un período de soporte post-lanzamiento incluido para resolver lo que aparezca en uso real."
                )
            },

            Stack = new List<StackGroup>
            {
                new(
                    "Lenguajes",
                    new[]
                    {
                        "C#",
                        "Java",
                        "JavaScript",
                        "TypeScript",
                        "Python"
                    }
                ),

                new(
                    "Frontend",
                    new[]
                    {
                        "React.js",
                        "Next.js",
                        "Astro",
                        "Vite.js"
                    }
                ),

                new(
                    "Backend",
                    new[]
                    {
                        "ASP.NET Core MVC",
                        "Django"
                    }
                ),

                new(
                    "Datos",
                    new[]
                    {
                        "Supabase",
                        "MySQL",
                        "SQL Server",
                        "Oracle SQL"
                    }
                )
            },

            Why = new List<WhyItem>
            {
                new(
                    "Un solo interlocutor",
                    "Hablas directo con el equipo desde la primera reunión hasta el despliegue final. Nada se pierde entre traspasos."
                ),

                new(
                    "Código que puedes heredar",
                    "Documentación clara y decisiones justificadas, para que cualquier equipo futuro pueda seguir el proyecto sin partir de cero."
                ),

                new(
                    "Experiencia en terreno",
                    "Antes de dedicarnos al desarrollo trabajamos en operaciones de planta, así que entendemos procesos reales, no solo diagramas."
                ),

                new(
                    "Presupuesto cerrado",
                    "Cotización clara antes de partir. Si algo cambia de alcance, lo conversamos antes — no aparece en la factura."
                )
            },

            ContactLines = new List<ContactLine>
            {
                new(
                    "Correo",
                    "gonzalovr2001@gmail.com"
                ),

                new(
                    "Ubicación",
                    "San Felipe, Valparaíso, Chile"
                ),

                new(
                    "Modalidad",
                    "Remoto / presencial"
                ),

                new(
                    "Respuesta",
                    "< 48 horas hábiles"
                )
            },

            ContactForm = form
        };
    }
}