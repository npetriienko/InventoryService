using InventoryService.Data;
using InventoryService.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("serilog.json", optional: false, reloadOnChange: true);
        builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration);
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<InventoryContext>(options => 
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddAutoMapper(typeof(Program));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRequestContextLogging();
        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}