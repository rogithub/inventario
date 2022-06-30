#!/bin/bash

DbPath=./Ro.Inventario.Web/Db
DbFile=inventario.db
FOLDERNAME=papeleria
FILENAME=inventario_$(date -d "today" +"%Y-%m-%dT%H_%M_%S").db

cp $DbPath/$DbFile $DbPath/$FILENAME

lftp -c "$FTP_CNN_STR;put -O "$FOLDERNAME" ./"$DbPath/$FILENAME""
rm $DbPath/$FILENAME


