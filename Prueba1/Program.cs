using Resend;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// MVC
// =========================================================

builder.Services.AddControllersWithViews();

// =========================================================
// RESEND
// =========================================================

builder.Services.AddOptions();

builder.Services.AddHttpClient<ResendClient>();

// =========================================================
// BUILD
// =========================================================

var app = builder.Build();

// =========================================================
// CONFIGURACIÓN DEL PIPELINE
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// =========================================================
// RUTA MVC
// =========================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// =========================================================
// RUN
// =========================================================

app.Run();