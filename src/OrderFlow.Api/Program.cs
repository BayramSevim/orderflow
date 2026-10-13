using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;
using OrderFlow.Application.Dealers;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DBContext
builder.Services.AddDbContext<OrderFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderFlow")));

// Repository + Unit Of Work
builder.Services.AddScoped<IDealerRepository, DealerRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderFlowDbContext>());

// Services
builder.Services.AddScoped<DealerService>();

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
