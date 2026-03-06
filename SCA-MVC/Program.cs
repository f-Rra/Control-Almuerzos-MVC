using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Helpers;
using SCA_MVC.Models;
using SCA_MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── ASP.NET Identity ────────────────────────────────────────────────────────
// AddIdentity registra todos los servicios: UserManager, RoleManager,
// SignInManager, IPasswordHasher, IUserValidator, etc.
// Vincula ApplicationUser como entidad de usuario y IdentityRole como rol.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Política de contraseñas
    options.Password.RequiredLength         = 6;
    options.Password.RequireDigit           = true;
    options.Password.RequireLowercase       = true;
    options.Password.RequireUppercase       = false;  // Relajado para facilidad de uso
    options.Password.RequireNonAlphanumeric = false;

    // Lockout: bloqueo tras intentos fallidos
    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers      = true;

    // Usuario: email único, no requerir confirmación para simplificar
    options.User.RequireUniqueEmail         = true;
    options.SignIn.RequireConfirmedAccount  = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<SpanishIdentityErrorDescriber>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppUserClaimsPrincipalFactory>();

// Cookie de autenticación: redirige a /Account/Login si no está autenticado
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.LogoutPath       = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan   = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<IEmpresaNegocio, EmpresaNegocio>();
builder.Services.AddScoped<IEmpleadoNegocio, EmpleadoNegocio>();
builder.Services.AddScoped<IServicioNegocio, ServicioNegocio>();
builder.Services.AddScoped<IRegistroNegocio, RegistroNegocio>();
builder.Services.AddScoped<ILugarNegocio, LugarNegocio>();
builder.Services.AddScoped<IReporteNegocio, ReporteNegocio>();
builder.Services.AddScoped<IEstadisticasNegocio, EstadisticasNegocio>();

// Configuración de QuestPDF para uso comunitario
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// UseAuthentication debe ir ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
