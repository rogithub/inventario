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
/*

public enum TipoAjuste
{
    Venta = 0,
    Merma = 1,
    IngresoSinCompra = 2
}
*/
DROP VIEW IF EXISTS v_inventario;
CREATE VIEW v_inventario
AS
SELECT 
	p.*,	
	(
		(
			(SELECT ifnull(SUM(cp.Cantidad), 0) FROM ComprasProductos cp WHERE cp.ProductoId = p.Id) 
			+
			(SELECT ifnull(SUM(ap.Cantidad), 0) FROM AjustesProductos ap JOIN Ajustes a on a.Id = ap.AjusteId WHERE a.TipoAjuste IN (2) AND ap.ProductoId = p.Id)
		)
		- (SELECT ifnull(SUM(ap.Cantidad), 0) FROM AjustesProductos ap JOIN Ajustes a on a.Id = ap.AjusteId WHERE a.TipoAjuste IN (0, 1) AND ap.ProductoId = p.Id)
	) AS Stock
FROM 
	v_productos p;


-- v_ventas_productos (count debe ser AjustesProductos count)
DROP VIEW IF EXISTS v_ventas_productos;
CREATE VIEW v_ventas_productos
AS
SELECT 
	ap.Id as AjusteProductoId,
	ap.ProductoId,
	ap.AjusteId,
	date( a.FechaAjuste ) as FechaAjuste,
	cast(a.Pago AS FLOAT) as Pago,
	cast(a.Cambio AS FLOAT) as Cambio,
	cast(ap.Cantidad AS FLOAT) as Cantidad
	, cast(ap.PrecioUnitarioVenta AS FLOAT) as PrecioUnitarioVenta
	, cast(cp.PrecioCompra AS FLOAT) as UltimoPrecioCompra	
FROM 
	AjustesProductos ap JOIN Ajustes a ON a.Id = ap.AjusteId JOIN productos p on ap.ProductoId = p.id	
	JOIN ComprasProductos cp ON cp.Id = 
		(
			SELECT cp.Id FROM ComprasProductos cp JOIN Compras c on cp.CompraId = c.Id 
			WHERE 
				cp.ProductoId = p.Id
				AND  datetime(c.FechaFactura) <= datetime(a.FechaAjuste)
			ORDER BY datetime(c.FechaFactura) DESC LIMIT 1)
WHERE TipoAjuste = 0;

-- rpt_estimado_ventas Estimado de ventas basado en precios estimados
DROP VIEW IF EXISTS rpt_estimado_ventas;
CREATE VIEW rpt_estimado_ventas
AS
SELECT 
	FechaAjuste,
	COUNT(DISTINCT(AjusteId)) as NumeroVentas,
	SUM(UltimoPrecioCompra * Cantidad) as Inversion,
	SUM(PrecioUnitarioVenta * Cantidad) as Venta,	
	SUM(PrecioUnitarioVenta * Cantidad) - SUM(UltimoPrecioCompra * Cantidad) as Ganancia
FROM v_ventas_productos
GROUP BY FechaAjuste
ORDER BY FechaAjuste;


DROP VIEW IF EXISTS rpt_estimado_ventas_mensual;
CREATE VIEW rpt_estimado_ventas_mensual
AS
SELECT 
       strftime("%m-%Y", FechaAjuste) as 'Mes',
	   SUM(NumeroVentas) as NumeroVentas,
	   SUM(Inversion) as Inversion,
	   SUM(Venta) as Venta,
	   SUM(Ganancia) as Ganancia
       FROM rpt_estimado_ventas GROUP BY strftime("%m-%Y", FechaAjuste);


-- Rendimiento
DROP VIEW IF EXISTS rpt_rendimiento_productos;
CREATE VIEW rpt_rendimiento_productos
AS
SELECT 
	p.Nombre, 
	v.PrecioUnitarioVenta - v.UltimoPrecioCompra as Rendimiento, 
	SUM(v.Cantidad) as VecesVendido,
	p.Stock
FROM v_inventario p JOIN v_ventas_productos v on p.Id = v.ProductoId 
GROUP BY p.Id
ORDER BY FechaAjuste DESC;

COMMIT;




