using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPE.Chatbot.Domain.Entities.chatBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IPE.Chatbot.Persistence.Configuracion.chatbot
{
    public class SolicitudesAsesorConfiguracion : IEntityTypeConfiguration<SolicitudesAsesorEntity>
    {
        public void Configure(EntityTypeBuilder<SolicitudesAsesorEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DerechohabienteId).IsRequired();
            builder.Property(x => x.Estado).IsRequired();
            builder.Property(x => x.FechaSolicitud).IsRequired();
            
            // Configure the relationship
            builder.HasOne(x => x.Derechohabiente)
                   .WithMany(d => d.SolicitudesAsesor)
                   .HasForeignKey(x => x.DerechohabienteId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
