using IPE.Chatbot.Domain.Entities.chatBot;
using IPE.Chatbot.Persistence.Configuracion.chatbot;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Persistence
{
    public partial class ChatbotDbContext : DbContext
    {
        public ChatbotDbContext()
        {
        }

        public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DerechohabienteConfiguracion());
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}