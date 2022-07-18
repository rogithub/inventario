using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Services;
using Ro.SQLite.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
        options.LogoutPath = "/Account/Logout";
    });

var dbPath = builder.Configuration.GetSection("DbPath").Value;
var interopPath = builder.Configuration.GetSection("InteropPath").Value;

builder.Services.AddTransient<IDbAsync>(svc =>
{
    var connString = string.Format("Data Source={0}; Version=3;", dbPath);
    return new Database(connString, new DbTasks(interopPath));
});
builder.Services.AddScoped<IComprasProductosRepo, ComprasProductosRepo>();
builder.Services.AddScoped<IComprasRepo, ComprasRepo>();
builder.Services.AddScoped<IPreciosProductosRepo, PreciosProductosRepo>();
builder.Services.AddScoped<IProductosRepo, ProductosRepo>();
builder.Services.AddScoped<IUnidadMedidaRepo, UnidadMedidaRepo>();
builder.Services.AddScoped<ICategoriasRepo, CategoriasRepo>();
builder.Services.AddScoped<ICategoriasProductosRepo, CategoriasProductosRepo>();
builder.Services.AddScoped<IComprasService, ComprasService>();
builder.Services.AddScoped<INuevosProductosValidatorService, NuevosProductosValidatorService>();
builder.Services.AddScoped<IBusquedaProductosRepo, BusquedaProductosRepo>();
builder.Services.AddScoped<IAjustesProductosRepo, AjustesProductosRepo>();
builder.Services.AddScoped<IAjustesRepo, AjustesRepo>();
builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<IVentasService, VentasService>();
builder.Services.AddScoped<ISettingsRepo, SettingsRepo>();
builder.Services.AddScoped<IDevolucionesProductosRepo, DevolucionesProductosRepo>();
builder.Services.AddScoped<IUsersRepo, UsersRepo>();
builder.Services.AddScoped<IChangePasswordRepo, ChangePasswordRepo>();
builder.Services.AddScoped<IRolesRepo, RolesRepo>();


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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
