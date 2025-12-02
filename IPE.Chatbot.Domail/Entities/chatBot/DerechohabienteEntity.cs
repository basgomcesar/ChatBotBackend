using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPE.Chatbot.Domain.Entities.chatBot
{
    public class DerechohabienteEntity
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Tipo { get; set; }
        public string? Flujo { get; set; }
        public string? Paso { get; set; }
        public string? Afiliacion { get; set; }  
        public string? Folio { get; set; }
        public DateTime? UltimaInteraccion { get; set; } 
        public DateTime FechaCreacion { get; set; }

        public ICollection<SolicitudesSimulacionEntity> SolicitudesSimulacion { get; set; } = new List<SolicitudesSimulacionEntity>();
    }
}
