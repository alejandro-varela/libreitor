#!/usr/bin/env bash

desde=$(date --date="yesterday" +"%Y-%m-%d")

#####################################################################
# Zona Bs As                                                        #
#####################################################################

# Línea 203
echo "Ejecutando Línea 203 modo driveup"
dotnet QPApp.dll modo=driveup       desde=$desde lineas=159,163 password=Bondi.amarillo tipoPuntas=PuntaLinea radioPuntas=500

echo "Ejecutando Línea 203 modo picobus"
dotnet QPApp.dll modo=picobus       desde=$desde lineas=159,163 password=Bondi.amarillo tipoPuntas=PuntaLinea radioPuntas=500

#####################################################################
# Zona Rosario                                                      #
#####################################################################

# Línea 35/9
echo "Ejecutando Línea 35/9 modo tecnobus/smgps"
dotnet QPApp.dll modo=tecnobussmgps desde=$desde lineas=21      password=Bondi.amarillo tipoPuntas=PuntaLinea radioPuntas=300

# Línea CP
echo "Ejecutando Línea CP modo tecnobus/smgps"
dotnet QPApp.dll modo=tecnobussmgps desde=$desde lineas=100     password=Bondi.amarillo tipoPuntas=PuntaLinea radioPuntas=300

# Línea CN
echo "Ejecutando Línea CN modo tecnobus/smgps"
dotnet QPApp.dll modo=tecnobussmgps desde=$desde lineas=101     password=Bondi.amarillo tipoPuntas=PuntaLinea radioPuntas=300

# Línea Expreso
echo "Ejecutando Línea EXPRESO modo tecnobus/smgps"
dotnet QPApp.dll modo=tecnobussmgps desde=$desde lineas=103     password=Bondi.amarillo tipoPuntas=PuntaLinea radioPuntas=300
