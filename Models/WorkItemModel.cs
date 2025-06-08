using System;

public class WorkItemModel
{
    // Campos comunes
    public string Title { get; set; }               // System.Title
    public string State { get; set; }               // System.State

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
    public int Id { get; internal set; }
}

