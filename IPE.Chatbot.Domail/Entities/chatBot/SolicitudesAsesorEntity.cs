using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPE.Chatbot.Domain.Entities.chatBot
{
    public class SolicitudesAsesorEntity
    {
        public int Id { get; set; }
        public int DerechohabienteId { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        
        // Navigation property
        public DerechohabienteEntity Derechohabiente { get; set; }
    }
}
