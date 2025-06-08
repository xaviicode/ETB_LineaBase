// Models/SistemaAsociado.cs
using System;
using System.Collections.Generic;

namespace LineaBaseETB_V2.Models
{
    public class SistemaAsociado
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NumeroIniciativa { get; set; }
        public string LiderTecnico { get; set; }
        public string NumeroCRQ { get; set; }
        public bool ActualizaManualDespliegue { get; set; }
        public bool BaseDatos { get; set; }
        public bool EsDelta { get; set; }
        public bool Bug { get; set; }
        public string Fabrica { get; set; }
        public string TipoDelta { get; set; }
        public string DirectorProyecto { get; set; }
        public DateTime? FechaPnP { get; set; }
        public DateTime? FechaEstimadaPuestaProd { get; set; }
        public bool CumpleEstandares { get; set; }
        public string Estado { get; set; }
        public string Sistema { get; set; }
        public DateTime? FechaProgramacion { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public string ErrorMessagesString => string.Join("; ", ErrorMessages);
    }
}
