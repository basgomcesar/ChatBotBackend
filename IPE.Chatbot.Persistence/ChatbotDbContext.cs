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

        public DbSet<DerechohabienteEntity> Derechohabientes { get; set; }
        public DbSet<SolicitudesSimulacionEntity> SolicitudesSimulacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DerechohabienteConfiguracion());
            modelBuilder.ApplyConfiguration(new SolicitudesSimulacionConfiguracion());
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}