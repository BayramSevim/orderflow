using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.ErrorHandling;
using OrderFlow.Application.Common;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Application.Dealers;
using OrderFlow.Application.Orders;
using OrderFlow.Application.Orders.Commands;
using OrderFlow.Application.Orders.Queries;
using OrderFlow.Application.Products;
using OrderFlow.Application.Users;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.Infrastructure.Persistence.Repositories;
using OrderFlow.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);


// Validator
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers();

// DBContext
builder.Services.AddDbContext<OrderFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderFlow")));

// Repository + Unit Of Work
builder.Services.AddScoped<IDealerRepository, DealerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderFlowDbContext>());

// Services
builder.Services.AddScoped<DealerService>();
builder.Services.AddScoped<ProductService>();

// ExceptionHandler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CQRS
builder.Services.Scan(scan => scan
    .FromAssemblyOf<CreateOrderHandler>()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
        .AsImplementedInterfaces().WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
        .AsImplementedInterfaces().WithScopedLifetime());


// Password Hash
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo("en");

var app = builder.Build();

app.UseExceptionHandler();

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
