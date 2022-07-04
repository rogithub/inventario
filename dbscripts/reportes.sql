BEGIN TRANSACTION;


-- Vista Productos
DROP VIEW IF EXISTS v_productos;
CREATE VIEW v_productos
AS
SELECT 
	p.nid, p.Id, p.Nombre, c.Nombre as Categoria, um.Nombre as UnidadMedida,
	(SELECT PrecioVenta FROM PreciosProductos pp WHERE p.Id = pp.ProductoId ORDER BY datetime(pp.FechaCreado) DESC LIMIT 1) as PrecioVenta,
       (SELECT PrecioCompra FROM Compras c JOIN ComprasProductos cp ON c.Id = cp.CompraId WHERE cp.ProductoId = p.Id ORDER BY datetime(c.FechaCreado) DESC LIMIT 1) as PrecioCompra,
	p.CodigoBarrasItem,
	p.CodigoBarrasCaja

FROM Productos p 
JOIN CategoriasProductos cp ON p.Id = cp.ProductoId
JOIN Categorias c ON c.Id = cp.CategoriaId
JOIN UnidadesMedida um ON p.UnidadMedidaId = um.Id;

-- Vista Inventario
DROP VIEW IF EXISTS v_inventario;
CREATE VIEW v_inventario
AS
SELECT 
	p.*,	
	((SELECT ifnull(SUM(cp.Cantidad), 0) FROM ComprasProductos cp WHERE cp.ProductoId = p.Id) 
	- (SELECT ifnull(SUM(ap.Cantidad), 0) FROM AjustesProductos ap WHERE ap.ProductoId = p.Id)) AS Stock
FROM 
	v_productos p;


-- Reporte de Ventas
DROP VIEW IF EXISTS rpt_ventas;
CREATE VIEW rpt_ventas
AS
SELECT 
	DATE(a.FechaAjuste) FechaVenta, 
	SUM(ap.Cantidad) as ProductosVendidos,
	SUM(CAST(p.PrecioCompra AS FLOAT)) as Inversion,
	SUM(CAST(a.Pago AS FLOAT))-SUM(CAST(a.Cambio AS FLOAT)) as TotalVenta
FROM 
	Ajustes a JOIN AjustesProductos ap ON a.Id = ap.AjusteId JOIN v_productos p on ap.ProductoId = p.id
WHERE TipoAjuste = 0 
GROUP BY Date(a.FechaAjuste)
	
COMMIT;




