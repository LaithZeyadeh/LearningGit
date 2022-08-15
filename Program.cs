using Microsoft.EntityFrameworkCore;
using AllServices;
using DataModels.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IDatabase, DatabaseService>(); // To connect interface with Service then with context.
// Connect to PostgreSQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Mycontext>(options => options.UseNpgsql(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Authentication and auothrization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
