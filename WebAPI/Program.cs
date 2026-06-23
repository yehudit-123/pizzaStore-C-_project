using Infrastructure;
using InfrastructureAPI;
using Microsoft.EntityFrameworkCore;
using Service;
using ServiceInterface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PizzaStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ofakimPizza")));

builder.Services.AddScoped<IPizzaInfrastructure, PizzaInfrastructure>();
builder.Services.AddScoped<IPizzaService, PizzaServices>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();