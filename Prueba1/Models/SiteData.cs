namespace CommitSystemsMvc.Models;

public record ServiceItem(string Num, string Title, string Desc, string[] Tags);

public record ProcessStep(string Num, string Title, string Desc);

public record StackGroup(string Group, string[] Items);

public record WhyItem(string Title, string Desc);

public record ContactLine(string Label, string Value);

// ViewModel principal de la página de inicio: agrupa todo el contenido
// editable del sitio, en el mismo espíritu que siteData.js en la
// versión React — para cambiar textos, solo se edita HomeController.
public class HomeViewModel
{
    public string HeroEyebrow { get; init; } = string.Empty;
    public string HeroLead { get; init; } = string.Empty;

    public List<ServiceItem> Services { get; init; } = new();
    public List<ProcessStep> Process { get; init; } = new();
    public List<StackGroup> Stack { get; init; } = new();
    public List<WhyItem> Why { get; init; } = new();
    public List<ContactLine> ContactLines { get; init; } = new();

    public ContactFormModel ContactForm { get; init; } = new();
}
