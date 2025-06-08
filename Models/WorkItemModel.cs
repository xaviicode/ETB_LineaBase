using System;

namespace LineaBaseETB_V2.Models
{
    public class WorkItemModel
    {
        // Campos básicos de Azure DevOps
        public int Id { get; set; }
        public string Title { get; set; }               // System.Title
        public string State { get; set; }               // System.State (SOLO UNA VEZ)
        //public string Id { get; set; }                // System.WorkItemType (para filtros)
        public string AssignedTo { get; set; }          // System.AssignedTo (para filtros)

        // Campos para EntregaFuentes (Carrito)
        public string NumeroIniciativa { get; set; }
        public string Sistema1 { get; set; }
        public string Sistema2 { get; set; }
        public string Sistema3 { get; set; }
        public string Sistema4 { get; set; }
        public string Sistema5 { get; set; }
        public string Sistema6 { get; set; }
        public string Sistema7 { get; set; }
        public string Sistema8 { get; set; }
        public string Sistema9 { get; set; }
        public string Sistema10 { get; set; }
        public string Sistema11 { get; set; }
        public string Sistema12 { get; set; }

        // Campos para GestiónPasoProducción (Avión)
        public string SistemasAfectados { get; set; }
        public DateTime? FechaPnP { get; set; }
        public string Type { get; internal set; }
    }
}
