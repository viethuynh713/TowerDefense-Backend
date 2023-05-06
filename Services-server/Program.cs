using Service.Models;
using Service.Services;


var builder = WebApplication.CreateBuilder(args);

// Todo
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MythicEmpire"));
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<CardService>();
builder.Services.AddSingleton<GameSessionService>();



// End todo

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
