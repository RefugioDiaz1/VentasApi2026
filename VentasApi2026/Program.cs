using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using VentasApi2026.Data;
using VentasApi2026.Filter;
using VentasApi2026.Helpers;
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


//JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["key"]!)),

            ClockSkew = TimeSpan.Zero // ✅ elimina los 5 minutos extra
        };
    });

// En Program.cs — hace que Swagger muestre "Pending", "Completed", "Cancelled"
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });


// Carga los permisos directo de la BD al iniciar la app
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var permissions = context.Permissions.Select(p => p.Name).ToList();

    builder.Services.AddAuthorization(options =>
    {

        ////Order
        //options.AddPolicy("CanViewOrder", policy => policy.RequireClaim("permission", "CanViewOrder"));
        //options.AddPolicy("CanMyOrdersAsync", policy => policy.RequireClaim("permission", "CanMyOrdersAsync"));
        //options.AddPolicy("CanViewAllOrder", policy => policy.RequireClaim("permission", "CanViewAllOrder"));
        //options.AddPolicy("CanCreateOrder", policy => policy.RequireClaim("permission", "CanCreateOrder"));
        //options.AddPolicy("CanCancelOrder", policy => policy.RequireClaim("permission", "CanCancelOrder"));
        //options.AddPolicy("CanCompleteOrder", policy => policy.RequireClaim("permission", "CanCompleteOrder"));

        ////Products
        //options.AddPolicy("CanDeleteProduct", policy => policy.RequireClaim("permission", "CanDeleteProduct"));
        //options.AddPolicy("CanUpdateProducts", policy => policy.RequireClaim("permission", "CanUpdateProducts"));
        //options.AddPolicy("CanCreateProducts", policy => policy.RequireClaim("permission", "CanCreateProducts"));

        //// Usuarios
        //options.AddPolicy("CanManageUsers", p => p.RequireClaim("permission", "CanManageUsers"));

        foreach (var permission in permissions)
            options.AddPolicy(permission, p => p.RequireClaim("permission", permission));


    });

}

    


// ✅ Swagger clásico
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa tu token JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderRepository,OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor(); // ✅ agrégalo
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();

builder.Services.AddScoped<TokenVersionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<TokenVersionFilter>(); // ✅ aplica a todos los endpoints
});



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

//jwt
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
