#!/usr/bin/env bash

diasAtras=$1

if [ -z "$diasAtras" ]
then
        diasAtras=0
fi

desde=$(date --date="$diasAtras days ago" +"%Y-%m-%d")
hoyoy=$(date +"%Y-%m-%d")T$(date +"%H%M%S")

function subirArchiIfExit0() {
        procExit=$1
        pathArchivo=$2
        soloNombreArchivo=$(basename -- $pathArchivo)
        newArchivo="con-${diasAtras}-dias-atras/$soloNombreArchivo"

        if [ $procExit -eq 0 ]
        then
                echo "tratando de subir $pathArchivo a $newArchivo"
                dotnet qpapp_uploader/QPAppUploaderHttp.dll "local=$pathArchivo" "remote=$newArchivo"
        fi
}

echo "me llamaron $(date) con $diasAtras días atrás " >> r3dcalls.txt


# Línea (159,163) 203 PILAR, 203 MORENO - driveup
archi=$(dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20 soloNombreArchivo=true)
        dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20
        subirArchiIfExit0 $? $archi

# Línea (159,163) 203 PILAR, 203 MORENO - picobus
archi=$(dotnet qpapp/QPApp.dll desde=$desde modo=picobus lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20 soloNombreArchivo=true)
        dotnet qpapp/QPApp.dll desde=$desde modo=picobus lineas=159,163 tipoPuntas=PuntaLinea radioPuntas=500 granularidad=20
        subirArchiIfExit0 $? $archi

# Línea 100 (CP) - tenobus-smgps
archi=$(dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=100 tipoPuntas=PuntaLinea radioPuntas=300 granularidad=20 soloNombreArchivo=true)
        dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=100 tipoPuntas=PuntaLinea radioPuntas=300 granularidad=20
        subirArchiIfExit0 $? $archi

# Línea 101 (CN) - tenobus-smgps
archi=$(dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=101 tipoPuntas=PuntaLinea radioPuntas=300 granularidad=20 soloNombreArchivo=true)
        dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=101 tipoPuntas=PuntaLinea radioPuntas=300 granularidad=20
        subirArchiIfExit0 $? $archi

# Línea 21 (35/9) - tenobus-smgps
archi=$(dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=21 tipoPuntas=PuntaLinea radioPuntas=300 granularidad=20 soloNombreArchivo=true)
        dotnet qpapp/QPApp.dll desde=$desde modo=driveup lineas=21 tipoPuntas=PuntaLinea radioPuntas=300 granularidad=20
        subirArchiIfExit0 $? $archi
