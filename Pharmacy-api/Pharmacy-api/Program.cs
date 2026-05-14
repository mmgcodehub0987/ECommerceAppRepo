using Pharmacy_api.Repositories.Interfaces;
using Pharmacy_api.Repositories.Json;
using Pharmacy_api.Services.Interfaces;
using Pharmacy_api.Services;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// SERVICE REGISTRATION
// ======================================================

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------------------------
// CORS CONFIGURATION
// Allow Angular frontend running on http://localhost:4200
// ------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ------------------------------------------------------
// REPOSITORY REGISTRATION
// ------------------------------------------------------
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

// ------------------------------------------------------
// SERVICE REGISTRATION
// ------------------------------------------------------
builder.Services.AddScoped<IMedicineService, MedicineService>();

var app = builder.Build();

// ======================================================
// HTTP REQUEST PIPELINE
// ======================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable CORS BEFORE authorization and endpoint mapping
app.UseCors("AllowAngular");

// Authorization middleware
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run application
app.Run();
