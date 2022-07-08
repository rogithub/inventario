using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;


public class AjusteModel
{
    public AjusteModel()
    {
        this.ProductoId = Guid.NewGuid();
    }
    
    public Guid ProductoId { get; set; }
    public Ajuste Tipo { get; set; }
    public decimal Cantidad { get; set; }
    public string Motivo { get; set; }
}