using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VentasApi2026.Data;
using VentasApi2026.Mappings;
using VentasApi2026.Middleware;
using VentasApi2026.Repositories;
using VentasApi2026.Repositories.Interfaces;
using VentasApi2026.Services;
using VentasApi2026.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(Program));


//Aqui es cuando en el dto donde guarda el usuario, si agrega un campo que no existe en el dto mandar una validacion al usuario
builder.Services.AddControllers().AddJsonOptions(options =>
{

    options.JsonSerializerOptions.UnmappedMemberHandling =
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//Personalizar respuesta de validación
builder.Services.Configure<ApiBehaviorOptions>(options =>
{

    options.InvalidModelStateResponseFactory = context =>
    {

        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .Where(x=>x.Key != "data") //Quitar Ruido
            .ToDictionary(
                kvp => kvp.Key.Replace("$.",""),
                kvp => kvp.Value!.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray()
            );

        var response = new
        {

            success = false,
            message = "Validation failed",
            errors
        };
        return new BadRequestObjectResult(response);
    };

});

//Conexion por defecto
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        ));

// ✅ Swagger clásico
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderRepository,OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

//Area de Middleware

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
