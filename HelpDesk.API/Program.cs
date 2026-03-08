using HelpDesk.API.Data;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Repositories.Implementations;
using HelpDesk.API.Services.Interfaces;
using HelpDesk.API.Services.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Add Swagger (API testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Database Connection
builder.Services.AddDbContext<HelpDeskDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// Dependency Injection (Service Layer)
builder.Services.AddScoped<ITicketService, TicketService>();

// Dependency Injection (Repository Layer)
builder.Services.AddScoped<ITicketRepository, TicketRepository>();


var app = builder.Build();


// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();

app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();