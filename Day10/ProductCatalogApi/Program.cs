using ProductCatalogApi.Data;
using ProductCatalogApi.Mapping;
using ProductCatalogApi.DTOs;
using FluentValidation;
using FluentValidation.AspNetCore;
using ProductCatalogApi.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProductCatalogContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();
builder.Services.AddScoped<IValidator<SellProductDto>, SellProductDtoValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Catalog API",
        Version = "v1",
        Description = "API quản lý danh mục sản phẩm với đầy đủ chức năng CRUD, lọc, phân trang và quản lý tồn kho",
        Contact = new OpenApiContact
        {
            Name = "Product Catalog Team",
            Email = "support@productcatalog.com"
        }
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Product Catalog API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
    });
}

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.MapControllers();

// Redirect root to Swagger
app.MapGet("/", context => {
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
