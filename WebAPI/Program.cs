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

// התיקון המעודכן: מניעת לולאה אינסופית
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PizzaStoreContext>();

    // פקודה זו תמחק את כל מסד הנתונים הישן והמבולגן לחלוטין!
    //context.Database.EnsureDeleted();

    // פקודה זו תייצר מסד נתונים חדש ונקי בהתאם לקוד המעודכן שלך
    context.Database.EnsureCreated();

    // פקודה זו תמלא את המסד הנקי ב-10 לקוחות ו-30 הזמנות חדשות
    DbSeeder.SeedData(context);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();