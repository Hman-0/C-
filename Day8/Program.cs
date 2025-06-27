
using Microsoft.EntityFrameworkCore;
using Day8.Data;
using Day8.Services;
using Day8.Mappings;
using Day8.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework with In-Memory Database
builder.Services.AddDbContext<FinancialDbContext>(options =>
    options.UseInMemoryDatabase("FinancialDB"));

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Financial Management API",
        Version = "v1",
        Description = "API quản lý dòng tiền và phân tích tài chính doanh nghiệp",
        Contact = new OpenApiContact
        {
            Name = "Financial Team",
            Email = "finance@company.com"
        }
    });
    
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS
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

// Configure the HTTP request pipeline
// Enable Swagger in all environments for demo purposes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financial Management API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FinancialDbContext>();
    context.Database.EnsureCreated();
    
    // Add sample transactions if database is empty
    if (!context.Transactions.Any())
    {
        var sampleTransactions = new[]
        {
            new Transaction
            {
                Date = DateTime.Now.AddDays(-30),
                Amount = 50000,
                Type = TransactionType.Income,
                Category = "Doanh thu bán hàng",
                Description = "Doanh thu tháng trước",
                DepartmentId = 1
            },
            new Transaction
            {
                Date = DateTime.Now.AddDays(-25),
                Amount = 15000,
                Type = TransactionType.Expense,
                Category = "Lương",
                Description = "Lương nhân viên IT",
                DepartmentId = 1
            },
            new Transaction
            {
                Date = DateTime.Now.AddDays(-20),
                Amount = 8000,
                Type = TransactionType.Expense,
                Category = "Marketing",
                Description = "Quảng cáo Facebook",
                DepartmentId = 2
            },
            new Transaction
            {
                Date = DateTime.Now.AddDays(-15),
                Amount = 12000,
                Type = TransactionType.Expense,
                Category = "Văn phòng phẩm",
                Description = "Mua thiết bị văn phòng",
                DepartmentId = 3
            }
        };
        
        context.Transactions.AddRange(sampleTransactions);
        context.SaveChanges();
    }
}

app.Run();
