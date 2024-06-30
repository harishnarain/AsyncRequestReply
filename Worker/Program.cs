using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker.Repositories;
using Worker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IRecordRepository, InMemoryRecordRepository>();
builder.Services.AddSingleton<IServiceBusService, ServiceBusService>(sp =>
{
    var repository = sp.GetRequiredService<IRecordRepository>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ServiceBusService(repository, configuration);
});
builder.Services.AddHostedService(sp => (BackgroundService)sp.GetRequiredService<IServiceBusService>());
builder.Services.AddControllers();
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
