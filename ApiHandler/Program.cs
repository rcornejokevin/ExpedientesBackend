using Microsoft.EntityFrameworkCore;
using DBHandler.Context;
using Support.Items;
using DBHandler.Service.Catalog;
using DBHandler.Service.Cases;
using DBHandler.Service.Security;
using BusinessLogic.Services;
var builder = WebApplication.CreateBuilder(args);
//Cors
var localCors = "MyCors";

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(localCors, b =>
    {
        b.WithOrigins("https://preprod-sideju-fe.marn.gob.gt",
                      "http://localhost:5173")
         .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
         .WithHeaders("Content-Type", "Authorization", "X-Requested-With", "X-CSRF-Token")
         .AllowCredentials();
    });
});

// Agrega servicios
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Permite que el controlador maneje ModelState.IsValid
        options.SuppressModelStateInvalidFilter = true;

    });
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DBHandlerContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDB"))
    .UseLazyLoadingProxies());

builder.Services.AddDbContext<LoginDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDBUser")));

builder.Services.AddScoped<Jwt>();
builder.Services.AddScoped<CampoService>();
builder.Services.AddScoped<FlujoService>();
builder.Services.AddScoped<EtapaService>();
builder.Services.AddScoped<EtapaDetalleService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RemitenteService>();
builder.Services.AddScoped<CampoLogic>();
builder.Services.AddScoped<FileLogic>();
builder.Services.AddScoped<CasesLogic>();
builder.Services.AddScoped<CasesService>();
builder.Services.AddScoped<CasesDetailService>();
builder.Services.AddScoped<CasesNoteService>();
var app = builder.Build();
app.UsePathBase("/app");
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error");
}
app.UseHttpsRedirection();

app.UseCors(localCors);

app.UseAuthorization();

app.MapControllers();

app.Map("/error", (HttpContext context) =>
{
    return Results.Problem("Ocurrió un error inesperado.");
});

app.Run();
