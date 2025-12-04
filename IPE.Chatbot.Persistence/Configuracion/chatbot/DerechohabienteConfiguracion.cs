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
    public class DerechohabienteConfiguracion : IEntityTypeConfiguration<DerechohabienteEntity>
    {
        public void Configure(EntityTypeBuilder<DerechohabienteEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nombre);
            builder.Property(x => x.Flujo);
            builder.Property(x => x.Paso);
            builder.Property(x => x.Folio);
            builder.Property(x => x.Telefono);
            builder.Property(x => x.Tipo);
            builder.Property(x => x.UltimaInteraccion);
            builder.Property(x => x.FechaCreacion);
        }
    }
}
