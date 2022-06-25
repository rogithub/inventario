namespace Ro.Inventario.Web.Entities;

public class Categoria
{    
    public Categoria()
    {
        Id = Guid.NewGuid();
        Nombre = string.Empty;
    }
    public Guid Id { get; set; }
    public string Nombre { get; set; }
}


public class CategoriaProducto
{    
    public CategoriaProducto()
    {
        Id = Guid.NewGuid();
        CategoriaId = Guid.NewGuid();
        ProductoId = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid ProductoId { get; set; }
}
