using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Configuración de la base de datos
builder.Services.AddDbContext<AppFianzasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();



// Agregar servicios para la caché distribuida en memoria y la sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de expirar la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Obligatorio según GDPR
});
// Habilitar Razor Pages y Controllers con recompilación en tiempo de ejecución
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddScoped<AutenticacionService>();
builder.Services.AddScoped<ListaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<DocumentosService>();
builder.Services.AddScoped<SolicitudService>();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Agregar esta línea ANTES de UseAuthorization
app.UseSession();

app.UseAuthorization();


// Configuración de endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Autenticacion}/{action=Inicio_sesion}/{id?}");
});
app.Run();
