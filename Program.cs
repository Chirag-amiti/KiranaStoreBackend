using Microsoft.EntityFrameworkCore;
using KiranaStore.Data;
using KiranaStore.Services.Interfaces;
using KiranaStore.Services.Implementations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() 
    .WriteTo.Console() 
    .WriteTo.File("Logs/app_log.txt", rollingInterval: RollingInterval.Day) // store daily log files
    .CreateLogger();

// Here I remove the default logging with serilog
builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddDbContext<KiranaContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Register services (DI)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductJsonService, ProductJsonService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kirana Store API V1");
        c.RoutePrefix = string.Empty; // Swagger at root: http://localhost:5000/
    });
}

app.MapControllers();

app.Run();

// Here logs are close after shutdown
Log.CloseAndFlush();
























// using Microsoft.EntityFrameworkCore;
// using KiranaStore.Data;
// using KiranaStore.Services.Interfaces;
// using KiranaStore.Services.Implementations;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();

// builder.Services.AddDbContext<KiranaContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//     ));

// builder.Services.AddScoped<IProductService, ProductService>();
// builder.Services.AddScoped<IProductJsonService, ProductJsonService>();

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kirana Store API V1");
//         c.RoutePrefix = string.Empty; // Swagger at root: http://localhost:5000/
//     });
// }

// app.MapControllers();
// app.Run();














// using Microsoft.EntityFrameworkCore;
// using KiranaStore.Data;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();

// // Register DbContext for MySQL
// builder.Services.AddDbContext<KiranaContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//     ));

// var app = builder.Build();

// // if (app.Environment.IsDevelopment())
// // {
// //     app.UseSwagger();
// //     app.UseSwaggerUI();
// // }

// app.MapControllers();

// app.Run();


// -----------------------------------------------------------------------------

// using Microsoft.EntityFrameworkCore;
// using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
// using KiranaStore.Models; 
// using KiranaStore.Data;  

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();

// builder.Services.AddDbContext<KiranaContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//     ));

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseAuthorization();

// app.MapControllers();

// app.Run();
