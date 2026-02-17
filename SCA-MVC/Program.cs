using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AccesoDatos>();
builder.Services.AddScoped<IEmpresaNegocio, EmpresaNegocio>();
builder.Services.AddScoped<IEmpleadoNegocio, EmpleadoNegocio>();
builder.Services.AddScoped<IServicioNegocio, ServicioNegocio>();
builder.Services.AddScoped<IRegistroNegocio, RegistroNegocio>();
builder.Services.AddScoped<ILugarNegocio, LugarNegocio>();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
