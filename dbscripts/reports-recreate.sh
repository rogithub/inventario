#!/bin/bash

DBFILE="inventario.db"
SCRIPTFILE="inventario.sql"
PROJ_DIR="Ro.Inventario.Web"
DEV_DB_DIR="Db-dev"
DB_DIR="Db"


if [ ! -f "../$PROJ_DIR/$DB_DIR/$DBFILE" ]; then
    echo "../$PROJ_DIR/$DB_DIR/$DBFILE does not exist."
    exit 1
fi

echo "creating path if not exists ../$PROJ_DIR/$DEV_DB_DIR"
if [ ! -d "../$PROJ_DIR/$DEV_DB_DIR" ]; then
    mkdir "../$PROJ_DIR/$DEV_DB_DIR"
fi

echo "copiying ../$PROJ_DIR/$DB_DIR/$DBFILE to ../$PROJ_DIR/$DEV_DB_DIR/$DBFILE"
cp "../$PROJ_DIR/$DB_DIR/$DBFILE" "../$PROJ_DIR/$DEV_DB_DIR/$DBFILE"
echo "executing ./reportes.sql"
sqlite3 "../$PROJ_DIR/$DEV_DB_DIR/$DBFILE" ".read ./reportes.sql"