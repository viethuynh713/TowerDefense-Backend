using Service.Models;
using Service.Services;


var builder = WebApplication.CreateBuilder(args);

// Todo
builder.Services.Configure<UserProfileDatabaseSettings>(
    builder.Configuration.GetSection("OnlineUserProfile"));
builder.Services.AddSingleton<UserProfileService>();




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
