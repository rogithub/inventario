#!/bin/bash

MIGRATIONSCRIPT="MIGRATION.sql"
EXTRASCRIPTFILE="reportes.sql"
DBFILE="inventario.db"
SCRIPTFILE="inventario.sql"
PROJ_DIR="Ro.Inventario.Web"
DB_DIR="Db-dev"

if [ ! -f "./$SCRIPTFILE" ]; then
    echo "./$SCRIPTFILE does not exist."
    exit 0
fi

if [ ! -d "../$PROJ_DIR/$DB_DIR" ]; then
    mkdir "../$PROJ_DIR/$DB_DIR"
fi

remove_file_if_exists () {    
    if [ -f "$1" ]; then
	    rm $1
    fi
}

exec_script_if_exists () {    
    if [ -f "$1" ]; then
	sqlite3 $DBFILE < "$1";
    fi
}

OLD="../$PROJ_DIR/$DB_DIR/$DBFILE"
remove_file_if_exists $OLD

remove_file_if_exists "./$DBFILE"

cat $SCRIPTFILE | sqlite3 $DBFILE

exec_script_if_exists "./$EXTRASCRIPTFILE"
exec_script_if_exists "./$MIGRATIONSCRIPT"

mv $DBFILE $OLD
