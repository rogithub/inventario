using System;
using System.ComponentModel.DataAnnotations;
namespace Ro.Inventario.Web.Models;

public class CompraNuevosProductos
{
    public CompraNuevosProductos()
    {
        this.Id = Guid.NewGuid();
        this.FechaCreado = DateTime.Now;
        this.FechaFactura = DateTime.Now;
        this.Notas = string.Empty;          
    }
    public Guid Id { get; set; }
    public string Notas { get; set; }
    public DateTime FechaCreado { get; set; }

    [Display(Name = "Fecha de factura")]
    [Required(ErrorMessage = "Fecha de factura es requerido")]
    public DateTime FechaFactura { get; set; }
    
    [Display(Name = "Costo de paquetería")]
    [Required(ErrorMessage = "Costo de paquetería es requerido")]
    public decimal CostoPaqueteria { get; set; }
    [Display(Name = "Total de la factura")]
    [Required(ErrorMessage = "Total de la factura es requerido")]
    public decimal TotalFactura { get; set; }    

    [Display(Name = "Archivo")]
    [Required(ErrorMessage = "Se requiere que suba el archivo")]
    public IFormFile? Archivo {get; set;}
}
