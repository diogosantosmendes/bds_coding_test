using Application.Auctions;
using Application.Inventory;
using Infrastructure.Auctions;
using Infrastructure.Inventory;
using Infrastructure.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
}); ;

builder.Services.AddInMemoryDb();

builder.Services.AddInventoryInfrastructure();
builder.Services.AddAuctionsInfrastructure();

builder.Services.AddAuctionsServices();
builder.Services.AddInventoryServices();

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

public partial class Program { }