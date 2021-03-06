# Auth
https://www.youtube.com/watch?v=BWa7Mu-oMHk

# inventario
Applicación web para administrar inventarios


## Install
In order to install SQLite library uncomment
nugget.config. To install the rest of the liraries
add standard nugget.org repo.

## dep-raspi folder
It is built from source specific for raspberry pi os-64
https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
download
https://system.data.sqlite.org/downloads/1.0.115.5/sqlite-netFx-source-1.0.115.5.zip
``` bash
sudo apt-get update
sudo apt-get install build-essential
cd <source root>/Setup
chmod +x compile-interop-assembly-release.sh
./compile-interop-assembly-release.sh
```
Now, you will have a freshly built library file called libSQLite.Interop.so in the <source root>/bin/2013/Release/bin directory. This file might have execution permission which isn’t relevant for a library, so remove it by
chmod -x <source root>/bin/2013/Release/bin/libSQLite.Interop.so
Copy libSQLite.Interop.so the directory where your application’s binaries reside (not the x64 or x86 subdirectories containing SQLite.Interop.dll), and you’re set to go.

# podman
Existe un script que hace todo lo de podman automaticamente. Note que la base de datos
se crea con un volumen compartido entre el repo y el container.
``` bash
./podman_deploy.sh
```

## podman detallado
``` bash
podman build -f Containerfile -t inventario-img
podman images
podman run -v $(pwd)/Ro.Inventario.Web/Db:/app/Db -d --name inventario -p 5002:5002 inventario-img
podman ps
podman logs inventario
podman exec -it inventario bash
```
## clean up
``` bash
podman stop inventario && \
podman rm inventario && \
podman rmi inventario-img
```

## run
``` bash
podman build -f Containerfile -t inventario-img && podman run -v $(pwd)/Ro.Inventario.Web/Db:/app/Db -d --name inventario -p 5002:5002 inventario-img
```

## refresh reports
``` bash
podman exec inventario sqlite3 /app/Db/inventario.db < ./dbscripts/reportes.sql
```

## Export Db data
``` bash
sqlite3 Db-dev/inventario.db
.output ./backup.sql
.dump
.exit
```

## Export Db data from BASH
``` bash
$ sqlite3 Db-dev/inventario.db .dump | grep INSERT

$ sqlite3 ../Ro.Inventario.Web/Db-dev/inventario.db < INSERTS.sql
```

## user & groups
``` bash
id
id -u $USER
id -g $USER
```

## user namespaces
``` bash
lsns -t user
```

## ssl certs for development
https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0
