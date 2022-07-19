#!/bin/bash

DBFILE="inventario.db"
SCRIPTFILE="reports-recreate.sh"
PROJ_DIR="Ro.Inventario.Web"
DEV_DB_DIR="Db-dev"
OUTPUTFILE="INSERTS.sql"

echo "updating db ../$PROJ_DIR/$DEV_DB_DIR/$DBFILE"
./$SCRIPTFILE

sqlite3 ../$PROJ_DIR/$DEV_DB_DIR/$DBFILE .dump | grep INSERT > $OUTPUTFILE

echo "done output ./$OUTPUTFILE"