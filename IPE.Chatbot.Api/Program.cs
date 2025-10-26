using IPE.Chatbot.Api.Services;
using IPE.Chatbot.Application.Interfaces;
using IPE.Chatbot.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ChatbotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IPE.Chatbot.Application.Features.Derechohabientes.Commands.CreateDerechohabienteCommand).Assembly));

// Configure Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ChatBot_";
});

// Configure SignalR
builder.Services.AddSignalR();

// Register notification service
builder.Services.AddScoped<IChatbotNotificationService, ChatbotNotificationService>();

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

// Map SignalR hub
app.MapHub<IPE.Chatbot.Api.Hubs.ChatbotHub>("/chatbotHub");

app.Run();