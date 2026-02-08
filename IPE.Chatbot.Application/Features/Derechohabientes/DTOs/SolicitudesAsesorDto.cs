using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPE.Chatbot.Application.Features.Derechohabientes.DTOs
{
    public class SolicitudesAsesorDto
    {
        public int Id { get; set; }
        public string NombreDerechohabiente { get; set; }
        public int DerechohabienteId { get; set; }
        public string Estado { get; set; }
        public string FechaSolicitude { get; set; }
        public string NumeroTelefono  { get; set; }
    }
}
