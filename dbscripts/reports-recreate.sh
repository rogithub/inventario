#!/bin/bash

DBFILE="inventario.db"
SCRIPTFILE="inventario.sql"
PROJ_DIR="Ro.Inventario.Web"
DEV_DB_DIR="Db-dev"
DB_URL="https://github.com/rogithub/inventario/raw/main/Ro.Inventario.Web/Db/inventario.db"



echo "creating path if not exists ../$PROJ_DIR/$DEV_DB_DIR"
if [ ! -d "../$PROJ_DIR/$DEV_DB_DIR" ]; then
    mkdir "../$PROJ_DIR/$DEV_DB_DIR"
fi

echo "getting lates db from $DB_URL"
curl -O "../$PROJ_DIR/$DEV_DB_DIR/$DBFILE" "$DB_URL"
echo "executing ./reportes.sql"
sqlite3 "../$PROJ_DIR/$DEV_DB_DIR/$DBFILE" ".read ./reportes.sql"