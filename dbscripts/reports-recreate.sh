#!/bin/bash

DBFILE="inventario.db"
SCRIPTFILE="reportes.sql"
PROJ_DIR="Ro.Inventario.Web"
DEV_DB_DIR="Db-dev"
DB_URL="https://github.com/rogithub/inventario/raw/main/Ro.Inventario.Web/Db/inventario.db"


if [ ! -d "../$PROJ_DIR/$DEV_DB_DIR" ]; then
    echo "creating path ../$PROJ_DIR/$DEV_DB_DIR"
    mkdir "../$PROJ_DIR/$DEV_DB_DIR"
fi

echo "getting lates db from $DB_URL"
curl -O "$DB_URL"
mv "./$DBFILE" "../$PROJ_DIR/$DEV_DB_DIR/$DBFILE"
echo "executing ./reportes.sql"
sqlite3 "../$PROJ_DIR/$DEV_DB_DIR/$DBFILE" ".read ./$SCRIPTFILE"