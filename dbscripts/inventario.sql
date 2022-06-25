BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Categorias" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "Nombre"	          TEXT NOT NULL,
       PRIMARY KEY("Id")
);

-- Init data
INSERT INTO Categorias (id, Nombre) VALUES ('3f75daaa-04c6-4d04-aaa1-e243a00ef2c6', 'General');
INSERT INTO Categorias (id, Nombre) VALUES ('e6e15392-7c43-49a6-bb5c-4ed6a6794c62', 'SHEIN');
INSERT INTO Categorias (id, Nombre) VALUES ('84debfc5-a23b-44c6-9945-248dfc4e4239', 'Monografias');
INSERT INTO Categorias (id, Nombre) VALUES ('20e61d99-6d05-4b8b-a9b3-bf5273558f9d', 'Biografias');
INSERT INTO Categorias (id, Nombre) VALUES ('c046b206-5a37-4463-925c-c195e267cc74', 'Kawaii');


CREATE TABLE IF NOT EXISTS "UnidadesMedida" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "Nombre"	          TEXT NOT NULL,
       PRIMARY KEY("Id")
);

INSERT INTO Categorias  (Id, Nombre) VALUES ('d0ce9c76-58d2-4462-a41f-1849b413ffbe', 'Pieza');
INSERT INTO Categorias  (Id, Nombre) VALUES ('20161328-7f62-4631-86c7-95be7f094f29', 'Metro');
INSERT INTO Categorias  (Id, Nombre) VALUES ('710ad3c8-b75e-49b3-a3cd-43507ac959c9', 'Caja');

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




