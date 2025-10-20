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
    public class SolicitudesSimulacionConfiguracion : IEntityTypeConfiguration<SolicitudesSimulacionEntity>
    {
        public void Configure(EntityTypeBuilder<SolicitudesSimulacionEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DerechohabienteId).IsRequired();
            builder.Property(x => x.TipoSimulacion);
            builder.Property(x => x.FechaSolicitud);
            builder.Property(x => x.Estado);
            
            // Configure the relationship
            builder.HasOne(x => x.Derechohabiente)
                   .WithMany(d => d.SolicitudesSimulacion)
                   .HasForeignKey(x => x.DerechohabienteId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
