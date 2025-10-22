using Microsoft.AspNetCore.Authentication.Cookies;
using LiberiaDriveMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Agrega servicios MVC
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// ✅ Registrar servicios
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<EmailService>();

// ✅ Configurar autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// 🔒 ESTE BLOQUE DEBE IR AQUÍ
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    await next();
});

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");
    

app.Run();
