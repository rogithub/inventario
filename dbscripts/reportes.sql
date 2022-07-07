BEGIN TRANSACTION;


-- Vista Productos --PRECIOS CON FECHAS MAS RECIENTES
DROP VIEW IF EXISTS v_productos;
CREATE VIEW v_productos
AS
SELECT 
	p.nid, p.Id, p.Nombre, c.Nombre as Categoria, um.Nombre as UnidadMedida,
	(SELECT PrecioVenta FROM PreciosProductos pp WHERE p.Id = pp.ProductoId ORDER BY datetime(pp.FechaCreado) DESC LIMIT 1) as UltimoPrecioVenta,
    (SELECT AVG (PrecioCompra) FROM Compras c JOIN ComprasProductos cp ON c.Id = cp.CompraId WHERE cp.ProductoId = p.Id) as PrecioCompraPromedio,
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
	- (SELECT ifnull(SUM(ap.Cantidad), 0) FROM AjustesProductos ap JOIN Ajustes a on a.Id = ap.AjusteId WHERE a.TipoAjuste=0 AND ap.ProductoId = p.Id)) AS Stock
FROM 
	v_productos p;


-- REVISAR NO REGRESA TODOS LOS PRODUCTOS (solo los que se han vendido)
DROP VIEW IF EXISTS v_ventas_productos;
CREATE VIEW v_ventas_productos
AS
SELECT 
	ap.Id as AjusteProductoId,
	ap.ProductoId,
	ap.AjusteId,
	ap.Cantidad,
	cast(pp.PrecioVenta AS FLOAT) as UltimoPrecioVenta,
	cast(cp.PrecioCompra AS FLOAT) as UltimoPrecioCompra	
FROM 
	Ajustes a JOIN AjustesProductos ap ON a.Id = ap.AjusteId JOIN productos p on ap.ProductoId = p.id
	JOIN PreciosProductos pp ON pp.Id = (SELECT Id FROM PreciosProductos WHERE  datetime( FechaCreado ) <= datetime( a.FechaAjuste ) AND ProductoId = p.Id ORDER BY datetime(FechaCreado) DESC LIMIT 1)
	JOIN ComprasProductos cp ON cp.Id = (SELECT cp.Id FROM ComprasProductos cp JOIN Compras c on cp.CompraId = c.Id WHERE datetime( c.FechaFactura ) <= datetime( a.FechaAjuste ) AND pp.ProductoId = p.Id ORDER BY datetime(c.FechaFactura) DESC LIMIT 1)
WHERE TipoAjuste = 0 

-- REVISAR PRECIOS
--SELECT 
--	a.Id as AjusteId,
--	p.Id as ProductoId,
--	p.Nombre,	
--	a.FechaAjuste, 
--	CAST(ap.Cantidad AS FLOAT) as Cantidad,
--	ifnull((SELECT PrecioCompra FROM Compras c JOIN ComprasProductos cp ON c.Id = cp.CompraId WHERE cp.ProductoId = p.Id AND datetime(c.FechaCreado) <= datetime(a.FechaAjuste) ORDER BY datetime(c.FechaCreado) DESC LIMIT 1),p.PrecioCompra) as PrecioCompra,
--	ifnull((SELECT PrecioVenta FROM PreciosProductos pp WHERE p.Id = pp.ProductoId AND datetime(pp.FechaCreado) <= datetime(a.FechaAjuste) ORDER BY datetime(pp.FechaCreado) DESC LIMIT 1),p.PrecioVenta) as PrecioVenta    
--FROM 
--	AjustesProductos ap JOIN Ajustes a ON a.Id = ap.AjusteId JOIN v_productos p on ap.ProductoId = p.id
--WHERE a.TipoAjuste = 0 
--ORDER BY Date(a.FechaAjuste), a.Id
	
COMMIT;




