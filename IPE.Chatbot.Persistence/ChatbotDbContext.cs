using IPE.Chatbot.Domain.Entities.chatBot;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Persistence
{
    public partial class ChatbotDbContext : DbContext
    {
        public DbSet<DerechohabienteEntity> Derechohabientes { get; set; }
        public ChatbotDbContext()
        {
        }

        public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}