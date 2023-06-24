using Service.Models;
using Service.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(
builder.Configuration.GetSection("MythicEmpire"));
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<CardService>();
builder.Services.AddSingleton<GameSessionService>();

// Register Lambda to replace Kestrel as the web server for the ASP.NET Core application.
// If the application is not running in Lambda then this method will do nothing. 
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// End todo

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
