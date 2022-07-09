BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "Settings" (
       "Key"  	       TEXT NOT NULL UNIQUE,
       "Value"         TEXT NULL,
       PRIMARY KEY("Key")
);

INSERT INTO Settings (Key, Value) VALUES ("IVA", "0.16");

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
       "nid"            INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
       "Id"  	    	   TEXT NOT NULL UNIQUE,
       "Nombre"         TEXT NOT NULL,
       "UnidadMedidaId" TEXT,
       "CodigoBarrasItem" TEXT, -- Si el item tiene codigo de barras por ejemplo codigo barras en un lapiz
       "CodigoBarrasCaja" TEXT       
);

CREATE VIRTUAL TABLE Productos_fst USING fts5 (
    Nombre,
    content=Productos,
    content_rowid='nid' 
);

CREATE TRIGGER Productos_fst_insert AFTER INSERT ON Productos
BEGIN
    INSERT INTO Productos_fst (rowid, nombre) VALUES (new.nid, new.nombre);
END;

CREATE TRIGGER Productos_fst_delete AFTER DELETE ON Productos
BEGIN
    INSERT INTO Productos_fst (Productos_fst, rowid, nombre) VALUES ('delete', old.nid, old.nombre);
END;

CREATE TRIGGER Productos_fst_update AFTER UPDATE ON Productos
BEGIN
    INSERT INTO Productos_fst (Productos_fst, rowid, nombre) VALUES ('delete', old.nid, old.nombre);
    INSERT INTO Productos_fst (rowid, nombre) VALUES (new.nid, new.nombre);
END;

CREATE TABLE IF NOT EXISTS "PreciosProductos" (
       "Id"  	    	  TEXT NOT NULL UNIQUE,
       "ProductoId"       TEXT NOT NULL,
       "FechaCreado"	  TEXT NOT NULL, -- el ultimo creado es el bueno       
       "PrecioVenta"	  NUMERIC,       -- Precio unitario de venta al publico incluyendo impuestos aplicables
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "CategoriasProductos" (
       "Id"  	    	    TEXT NOT NULL UNIQUE,
       "CategoriaId"	    TEXT NOT NULL,
       "ProductoId"         TEXT NOT NULL,       
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
       FOREIGN KEY("CategoriaId") REFERENCES "Categorias"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Compras" (
       "Id"  	    	   TEXT NOT NULL UNIQUE,
       "Proveedor"	   TEXT,
       "FechaFactura"   TEXT NOT NULL,       
       "FechaCreado"    TEXT NOT NULL,
       "CostoPaqueteria"   NUMERIC, -- decimal
       "TotalFactura"      NUMERIC, -- decimal
       "PorcentajeFacturaIVA" NUMERIC, -- decimal
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "ComprasProductos" (
       "Id"  	        TEXT NOT NULL UNIQUE,
       "ProductoId"	    TEXT NOT NULL,
       "CompraId"	    TEXT NOT NULL,
       "Cantidad"       NUMERIC, -- decimal
       "PrecioCompra"   NUMERIC, -- el precio al que yo lo compré
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
       FOREIGN KEY("CompraId") REFERENCES "Compras"("Id") ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS "Ajustes" (
       "Id"  	      TEXT NOT NULL UNIQUE,
       "Pago"         NUMERIC, -- cantidad que pagó el cliente
       "Cambio"       NUMERIC, -- cambio que le dimos
       "FechaAjuste"  TEXT NOT NULL,
       "TipoAjuste"   INTEGER DEFAULT 0, -- 0 es Venta       
       "IvaVenta"     NUMERIC, --IVA solo ventas
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "AjustesProductos" (
       "Id"  	                TEXT NOT NULL UNIQUE,
       "ProductoId"             TEXT NOT NULL,
       "AjusteId"               TEXT NOT NULL,
       "Cantidad"               NUMERIC, -- decimal
       "PrecioUnitarioVenta"    NUMERIC, -- Precio Unitario solo ventas
       "Notas"                  TEXT,
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
       FOREIGN KEY("AjusteId") REFERENCES "Ajustes"("Id") ON DELETE CASCADE

);

CREATE TABLE IF NOT EXISTS "DevolucionesProductos" (
       "Id"  	       TEXT NOT NULL UNIQUE,
       "AjusteId"      TEXT NOT NULL,
       "ProductoId"    TEXT NOT NULL,
       "Cantidad"      NUMERIC, -- decimal,
       "FechaCreado"   TEXT NOT NULL,
       "Motivo"	       TEXT,            -- Productos en buen estado vuelven al stock, por el hecho de reducir su cantidad en
                                        -- la venta origina. Para productos en mal estado, se reduce la cantidad en la
                                        -- venta y se captura una merma. 
       "ProductoEnBuenEstado"  INTEGER, -- 1 si está en buen estado, 0 no está en buen estado
       PRIMARY KEY("Id"),
       FOREIGN KEY("ProductoId") REFERENCES "Productos"("Id") ON DELETE CASCADE
       FOREIGN KEY("AjusteId") REFERENCES "Ajustes"("Id") ON DELETE CASCADE
);

COMMIT;
