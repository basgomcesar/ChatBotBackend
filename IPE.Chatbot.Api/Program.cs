using IPE.Chatbot.Api.Services;
using IPE.Chatbot.Application.Interfaces;
using IPE.Chatbot.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// CORS global (permite cualquier origen y soporta SignalR)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)  // Permite cualquier dominio
            .AllowAnyMethod()               // Permite cualquier método
            .AllowAnyHeader()               // Permite cualquier header
            .AllowCredentials();            // Necesario si usas SignalR
    });
});

builder.Services.AddDbContext<ChatbotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(IPE.Chatbot.Application.Features.Derechohabientes.Commands.CreateDerechohabienteCommand).Assembly));

// Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ChatBot_";
});

// SignalR
builder.Services.AddSignalR();

// Register services
builder.Services.AddScoped<IChatbotNotificationService, ChatbotNotificationService>();
builder.Services.AddScoped<IOcrService, OcrService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS BEFORE controllers and hubs
app.UseCors("AllowAll");

//app.UseHttpsRedirection();
//app.UseAuthorization();

app.MapControllers();

// SignalR Hub
app.MapHub<IPE.Chatbot.Api.Hubs.ChatbotHub>("/chatbotHub");

app.Run();
