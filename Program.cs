using Microsoft.EntityFrameworkCore;
using SmartLogApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add Database Context to the application container
builder.Services.AddDbContext<LogContext>(options =>
    options.UseSqlite("Data Source=smartlogs.db"));


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();

