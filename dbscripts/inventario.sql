BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Users" (
       "Id"  TEXT NOT NULL UNIQUE,
       "Email"	          TEXT NOT NULL UNIQUE,
       "IsActive"              INTEGER DEFAULT 0,
       "PasswordHash"	   BLOB NOT NULL,
       "PasswordSalt"	   BLOB NOT NULL,
       "DateCreated"	TEXT NOT NULL,
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "Roles" (
       "Id"	TEXT NOT NULL UNIQUE,
       "Role"	TEXT NOT NULL UNIQUE,
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "User_Roles" (
       "Id"	TEXT NOT NULL UNIQUE,
       "RoleId" TEXT NOT NULL,
       "UserId" TEXT NOT NULL,       
       PRIMARY KEY("Id"),
       FOREIGN KEY("RoleId") REFERENCES "Roles"("Id") ON DELETE CASCADE
       FOREIGN KEY("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Reset_Password" (
       "Id"                    TEXT NOT NULL UNIQUE,
       "UserId"                TEXT NOT NULL,       
       "UsedDate"	          TEXT NULL,
       "ExpiryDate"	          TEXT NOT NULL,
       PRIMARY KEY("Id"),
       FOREIGN KEY("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS "Generador_Cobros" (
       "Id"	                     TEXT NOT NULL UNIQUE,
       "IsActive"                  INTEGER DEFAULT 0,
       "Nombre"	              TEXT NOT NULL,
       "Recurrencia"               INTEGER,             -- UNICO,MENSUAL
       "RecurrenciaCada"           INTEGER DEFAULT 1,   -- CADAxMESES
       "Monto"	              REAL,
       "MontoRecargo"	       REAL,         -- Valor de recargo si paga vencido
       "FechaInicio"	              TEXT NOT NULL,
       "FechaFin"	              TEXT,         -- NULL is es por tiempo indefinido
       "FechaVencimiento"          TEXT NULL,    -- Fecha de vencimiento si pago único
       "DiaVencimiento"	       INTEGER NULL, -- Día de vencimiento si pago mensual (día del mes, valores 1-31)
       "DateCreated"	              TEXT NOT NULL,
       "DateModified"	       TEXT NOT NULL,
       PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "Cobros" (
       "Id"	                     TEXT NOT NULL UNIQUE,
       "GeneradorId"	              TEXT NOT NULL UNIQUE,
       "Nombre"	              TEXT NOT NULL,      
       "Monto"	              REAL,
       "MontoRecargo"	       REAL,
       "FechaVencimiento"          TEXT NULL,
       "DiaVencimiento"	       INTEGER NULL, 
       "DateCreated"	              TEXT NOT NULL,
       "Mes"                       INTEGER NULL, -- se llena en pagos mensuales únicamente
       "Anio"                      INTEGER NULL, -- se llena en pagos mensuales únicamente
       PRIMARY KEY("Id"),
       FOREIGN KEY("GeneradorId") REFERENCES "Generador_Cobros"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Casas" (
       "Id"	              TEXT NOT NULL UNIQUE,
       "UserId"             TEXT NOT NULL,
       "Direccion"	       TEXT,
       "DateCreated"	       TEXT NOT NULL,
       "DateModified"	TEXT NOT NULL,
       PRIMARY KEY("Id"),
       FOREIGN KEY("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Pagos" (
       "Id"	              TEXT NOT NULL UNIQUE,
       "CasaId"             TEXT NOT NULL,
       "CobroId"            TEXT NOT NULL,       
       "Monto"	       REAL,
       "DateCreated"	       TEXT NOT NULL,
       "DateModified"	TEXT NOT NULL,
       "FileName"	       TEXT,
       "Comprobante"	       BLOB,
       PRIMARY KEY("Id"),
       FOREIGN KEY("CasaId") REFERENCES "Casas"("Id") ON DELETE CASCADE
       FOREIGN KEY("CobroId") REFERENCES "Cobros"("Id") ON DELETE CASCADE
);

-- Siempre se crean, nunca se borran, el ultimo creado es el bueno
CREATE TABLE IF NOT EXISTS "Pagos_Estatus" (
       "Id"	              TEXT NOT NULL UNIQUE,
       "UserId"             TEXT NOT NULL,
       "PagoId"             TEXT NOT NULL,       
       "Estatus"	       TEXT NOT NULL,
       "Comentario"	       TEXT,
       "DateCreated"	       TEXT NOT NULL,
       PRIMARY KEY("Id"),
       FOREIGN KEY("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
       FOREIGN KEY("PagoId") REFERENCES "Pagos"("Id") ON DELETE CASCADE
);


-- Views
DROP VIEW IF EXISTS v_cobros_vencimientos;
CREATE VIEW v_cobros_vencimientos
AS
SELECT 
	CASE WHEN 
		(gc.Recurrencia = 0 AND datetime('now','localtime') > datetime(c.FechaVencimiento)) OR
		(gc.Recurrencia = 1 AND gc.RecurrenciaCada >= 1 AND strftime('%d',  datetime('now','localtime')) > c.DiaVencimiento)
	THEN 
		1 
	ELSE 
		0 
	END AS EsPagoVencido,    
	IFNULL(c.Monto, 0) AS PorPagar,
	IFNULL(c.Monto, 0) + IFNULL(c.MontoRecargo, 0) AS PorPagarConRecargo,
	c.* 
FROM 
	Cobros c JOIN Generador_Cobros gc ON gc.Id = c.GeneradorId;


DROP VIEW IF EXISTS v_pagos_estatus;
CREATE VIEW v_pagos_estatus
AS
SELECT 
	CASE WHEN 
		EXISTS (SELECT * FROM Pagos_Estatus WHERE PagoId = p.Id)		
	THEN 
		(SELECT Estatus FROM  Pagos_Estatus WHERE PagoId = p.Id ORDER BY DATETIME(DateCreated) DESC LIMIT 1)
	ELSE 
		'Nuevo'
	END AS Estatus,
	CASE WHEN 
		EXISTS (SELECT * FROM Pagos_Estatus WHERE PagoId = p.Id)		
	THEN 
		(SELECT Comentario FROM  Pagos_Estatus WHERE PagoId = p.Id ORDER BY DATETIME(DateCreated) DESC LIMIT 1)
	ELSE 
		''
	END AS Comentario,
	p.* 
FROM Pagos p;


-- Init data
INSERT INTO Roles  (Id, Role) VALUES ('82d97bc9-c2d2-4eae-b7ca-754fd2dfe53a', 'Admin');
INSERT INTO Roles  (Id, Role) VALUES ('28694aae-6193-4678-8c93-b1b9654a503f', 'User');
INSERT INTO Roles  (Id, Role) VALUES ('524354a2-ab68-4474-a2a0-ed6217029a55', 'Tesorero');

COMMIT;




