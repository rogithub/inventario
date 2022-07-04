BEGIN TRANSACTION;

-- Reporte de Ventas
DROP VIEW IF EXISTS rpt_ventas;
CREATE VIEW rpt_ventas
AS
WITH Precios AS (
     SELECT cp.ProductoId, cp.PrecioCompra FROM ComprasProductos cp JOIN Compras c ON c.Id = cp.CompraId ORDER BY DateTime(c.FechaCreado) DESC
  )
SELECT 
	DATE(a.FechaAjuste) FechaVenta, 
	SUM(ap.Cantidad) as ProductosVendidos,
	(SELECT SUM (PrecioCompra) FROM Precios WHERE ProductoId = ap.ProductoId) as Inversion,
	SUM(CAST(a.Pago AS FLOAT))-SUM(CAST(a.Cambio AS FLOAT)) as TotalVenta
FROM 
	Ajustes a JOIN AjustesProductos ap ON a.Id = ap.AjusteId 	
WHERE TipoAjuste = 0 
GROUP BY Date(a.FechaAjuste)
	
COMMIT;




