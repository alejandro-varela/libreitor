#!/usr/bin/env bash

echo "me llamaron $(date)" >> r3dcalls.txt

desde=$(date --date="3 days ago" +"%Y-%m-%d")
hoyoy=$(date +"%Y-%m-%d")T$(date +"%H%M%S")

function subirArchiIfExit0() {
        procExit=$1
        nArchivo=$2
        if [ $procExit -eq 0 ]
        then
                echo "tratando de subir $nArchivo"
                dotnet qpapp_uploader/QPAppUploaderHttp.dll "local=$nArchivo"
        fi
}

archi=$(dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20 soloNombreArchivo=true)
        dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20
        subirArchiIfExit0 $? $archi

archi=$(dotnet qpapp/QPApp.dll desde=$desde modo=picobus lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20 soloNombreArchivo=true)
        dotnet qpapp/QPApp.dll desde=$desde modo=picobus lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20
        subirArchiIfExit0 $? $archi

