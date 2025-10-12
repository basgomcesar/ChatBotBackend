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
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public int Tipo { get; set; }
        public string  Flujo { get; set; }
        public string Paso { get; set; }
        public string Folio { get; set; }
    }
}
