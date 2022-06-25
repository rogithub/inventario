BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Categorias" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "Nombre"	  TEXT NOT NULL,
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "UnidadesMedida" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "Nombre"	          TEXT NOT NULL,
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "Productos" (
       "Id"  	    	   	       TEXT NOT NULL UNIQUE,
       "Nombre"	              TEXT NOT NULL,
       "UnidadMedidaId"	       TEXT,
       "CodigoBarrasItem"	       TEXT, -- Si el item tiene codigo de barras por ejemplo codigo barras en un lapiz
       "CodigoBarrasCaja"	       TEXT, -- Si la caja tiene un codigo de barras por ejemplo caja lapices
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "PreciosProductos" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "ProductoId"    TEXT NOT NULL,
       "FechaCreado"	  TEXT NOT NULL, -- el ultimo creado es el bueno       
       "PrecioVenta"	  NUMERIC,
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS "CategoriasProductos" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "CategoriaId"	  TEXT NOT NULL,
       "ProductoId"    TEXT NOT NULL,       
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
       FOREIGN KEY("CategoriaId") REFERENCES "Categorias"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Compras" (
       "Id"  	    	   TEXT NOT NULL UNIQUE,
       "Notas"	   TEXT,
       "FechaFactura"   TEXT NOT NULL,       
       "FechaCreado"    TEXT NOT NULL,
       "CostoPaqueteria"   NUMERIC, -- decimal
       "TotalFactura"      NUMERIC, -- decimal
       "PorcentajeFacturaIVA" NUMERIC, -- decimal
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "ComprasProductos" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "ProductoId"	  TEXT NOT NULL,
       "CompraId"	  TEXT NOT NULL,
       "Cantidad"      NUMERIC, -- decimal
       "PrecioCompra"  NUMERIC, -- el precio al que yo lo compré
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
       FOREIGN KEY("CompraId") REFERENCES "Compras"("Id") ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS "Ventas" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "Notas"	  TEXT,
       "FechaVenta"    TEXT NOT NULL,
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "VentasProductos" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "ProductoId"	  TEXT NOT NULL,
       "VentaId"	  TEXT NOT NULL,
       "Cantidad"      NUMERIC, -- decimal
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
       FOREIGN KEY("VentaId") REFERENCES "Ventas"("Id") ON DELETE CASCADE

);


COMMIT;




