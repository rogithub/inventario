#!/bin/bash

CONTAINER_NAME="inventario"
IMAGE_NAME="inventario-img"
PROJ_DIR="Ro.Inventario.Web"
CONTAINER_FILE="./devContainerfile"
DB_PATH="./$PROJ_DIR/Db-dev"
CONT_DB_PATH="/app/Db"
PORT="5002:5002"

echo "--> 🚀 Ro script deploying podman container DEVELOPMENT"
# Get latest
echo "--> Updating git repository"
git pull

echo "--> Stop container if running"
## Stop if running 
if podman container exists $CONTAINER_NAME; then
    echo "--> Stopping and removing container: $CONTAINER_NAME"
    podman stop $CONTAINER_NAME
    podman rm $CONTAINER_NAME
fi

echo "--> Remove image if exists"
if podman image exists $IMAGE_NAME; then
    echo "--> Removing image: $IMAGE_NAME"
    podman rmi $IMAGE_NAME
fi

echo "--> Building image"
## Build image
podman build -f $CONTAINER_FILE -t $IMAGE_NAME

echo "--> Running container"
## Run
podman run -v $DB_PATH:$CONT_DB_PATH -d --name $CONTAINER_NAME -p $PORT $IMAGE_NAME

echo "--> Should be listed bellow"
podman ps

echo "--> End of Ro podman deployment script"
