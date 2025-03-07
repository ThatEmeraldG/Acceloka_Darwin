using Acceloka.Entities;
using Acceloka.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Acceloka.Application.Behaviors;
using Acceloka.Application.Commands.Users;
using Acceloka.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// PostgreSQL Connection
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<AccelokaContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection"));
});

builder.Services.AddTransient<TicketService>();
builder.Services.AddTransient<BookTicketService>();
builder.Services.AddTransient<BookedTicketDetailsService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<TransactionService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Acceloka.Application.AssemblyReference.Assembly);
});
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
builder.Services.AddValidatorsFromAssembly(Acceloka.Application.AssemblyReference.Assembly, includeInternalTypes: true);
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
