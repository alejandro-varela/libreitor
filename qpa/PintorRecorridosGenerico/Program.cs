using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Comun;
using LibQPA.ProveedoresVentas.DbSUBE;
using Newtonsoft.Json;
using Pinturas;

namespace PintorRecorridosGenerico
{
    class Program
    {
        public static void Main()
        {
            var lineas = new List<int> { 167 };
            var date = DateTime.Now;

            #region boletos
            var boletosStringBuilder = new StringBuilder();
            boletosStringBuilder.AppendLine("[");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256845\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 10.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 10.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:10:54\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256846\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:02\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256847\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 15.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 5.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:06\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256848\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 10.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 10.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:11\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256849\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:18\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256850\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:23\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256851\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 6.75,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 2.25,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:28\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256852\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:31\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256853\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 4.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 4.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:37\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256854\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 10.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 10.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:42\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256855\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 4.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 4.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:45\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256856\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:49\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256857\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:54\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256858\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 4.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 4.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:11:59\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256859\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 15.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 5.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:12:04\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256860\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:12:08\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256861\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:12:13\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256862\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 18.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 18.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:12:18\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256863\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:12:23\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256864\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 7.09,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 2.36,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:13:41\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56393,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68387");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256865\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:16:19\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56702,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68617");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256866\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 4.73,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 4.72,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:18:08\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56702,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68617");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256867\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 6.75,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 2.25,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:18:13\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56702,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68617");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256868\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 6.75,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 2.25,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:18:19\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56702,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68617");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256869\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:18:26\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56702,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68617");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256870\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 6.75,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 2.25,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:18:31\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56702,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.68617");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256871\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:19:33\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256872\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:19:50\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256873\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 18.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 18.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:19:54\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256874\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 8.1,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 8.1,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:20:31\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256875\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:20:37\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256876\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:20:41\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256877\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:20:47\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256878\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:20:51\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256879\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:20:57\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256880\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:21:01\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256881\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:22:32\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256882\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:22:42\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57427,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.69432");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256883\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:23:10\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.58456,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.70375");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256884\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:26:52\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256885\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:26:56\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256886\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:27:00\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256887\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:27:04\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256888\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:27:58\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256889\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 19.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:28:08\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256890\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:28:12\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256891\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:28:17\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256892\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 8.1,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 8.1,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:28:22\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256893\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 8.1,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 8.1,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:28:26\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256894\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:29:07\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256895\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:29:38\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256896\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:30:10\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256897\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:30:15\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256898\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:30:18\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59359,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.7149");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256899\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:32:32\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256900\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:33:13\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256901\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 10.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 10.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:33:17\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256902\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:33:21\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256903\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:33:33\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256904\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:33:37\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256905\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:34:04\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256906\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:34:08\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256907\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:34:13\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256908\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.45,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.45,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:35:09\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.59788,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72175");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256909\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:36:04\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256910\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:36:35\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256911\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:36:40\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256912\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:37:33\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256913\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:38:42\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256914\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:39:09\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256915\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:39:15\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": 0.0,");
            boletosStringBuilder.AppendLine("    \"Longitud\": 0.0");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256916\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:42:16\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.58296,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72989");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256917\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:43:00\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.58296,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72989");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256918\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:43:06\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.58296,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.72989");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256919\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.5,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.5,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:45:01\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57863,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73699");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256920\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 18.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 18.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:45:36\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57863,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73699");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256921\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 0.1,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 0.1,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:45:40\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57863,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73699");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256922\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:46:16\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57863,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73699");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256923\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:47:04\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57863,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73699");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256924\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:47:12\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57863,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73699");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256925\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 21.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 21.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:47:15\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.57863,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73699");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256926\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:51:17\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56976,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.74127");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256927\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 18.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 18.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:51:35\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56976,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.74127");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256928\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 18.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 18.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:52:18\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56279,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73539");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256929\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 18.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 18.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:52:24\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56279,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73539");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256930\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 20.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 20.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:52:33\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56279,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73539");
            boletosStringBuilder.AppendLine("  },");
            boletosStringBuilder.AppendLine("  {");
            boletosStringBuilder.AppendLine("    \"Id\": \"id-256931\",");
            boletosStringBuilder.AppendLine("    \"ValorInicial\": 9.0,");
            boletosStringBuilder.AppendLine("    \"ValorDescuento\": 0.0,");
            boletosStringBuilder.AppendLine("    \"ValorFinal\": 9.0,");
            boletosStringBuilder.AppendLine("    \"FechaCancelacion\": \"2021-10-01T13:55:09\",");
            boletosStringBuilder.AppendLine("    \"Latitud\": -34.56279,");
            boletosStringBuilder.AppendLine("    \"Longitud\": -58.73539");
            boletosStringBuilder.AppendLine("  }");
            boletosStringBuilder.AppendLine("]");
            var boletos = JsonConvert.DeserializeObject<List<BoletoComun>>( boletosStringBuilder.ToString() );
            #endregion boletos

            var cantTodosLosBoletos = boletos.Count();
            var filtrados = boletos.Where(bol => bol.Latitud != 0 && bol.Longitud != 0);
            var cantFiltrados = filtrados.Count();
            var cantUnicos = filtrados
                .Select(bol => $"{bol.Latitud}-{bol.Longitud}")
                .Distinct()
                .Count()
            ;

            var pintor = MakePintorBoletosYRecorridoTeorico(lineas, date, filtrados, 167, 3083, true, 20);

            using var bitmap = pintor.Render();
            bitmap.Save($"boleto_en_reco_out.png", ImageFormat.Png);
        }

        static PintorDeRecorrido MakePintorBoletosYRecorridoTeorico(
            IEnumerable<int> lineas,
            DateTime date,
            IEnumerable<BoletoComun> boletos, 
            int lineaRecoTeorico, 
            int banderaRecoTeorico,
            bool agregarPuntosDeLosBoletos = false,
            int granularidad = 20,
            string dirRecorridos = "../../../../Datos/ZipRepo/"
        )
        {
            if (!lineas.Contains(lineaRecoTeorico))
            {
                lineas = lineas
                    .Append(lineaRecoTeorico)
                ;
            }

            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos(dirRecorridos, lineas.ToArray(), date);

            var todosLosPuntosRec = recorridosRBus
                .SelectMany(
                    (reco) => reco.Puntos.HacerGranular(granularidad),
                    (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
                )
                .ToList()
            ;

            if (agregarPuntosDeLosBoletos)
            {
                var puntosDeLosBoletos = boletos
                    .Where(bol => bol.Latitud != 0 && bol.Longitud != 0)
                    .Select(bol => new PuntoRecorridoLinBan { Lat=bol.Latitud, Lng=bol.Longitud })
                ;
                todosLosPuntosRec.AddRange(puntosDeLosBoletos);
            }

            // topes
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosRec);

            // pinturitas
            var pintor = new PintorDeRecorrido(topes2D, granularidad);

            pintor
                .SetColorFondo(Color.Gray)
                .PintarPuntos(recorridosRBus.Where(reco => reco.Linea == lineaRecoTeorico && reco.Bandera == banderaRecoTeorico).First().Puntos.HacerGranular(granularidad), Color.LimeGreen, 3)
                .PintarPuntos(todosLosPuntosRec, Color.DarkGray)
                .PintarPuntos(boletos.Select(bol => new Punto { Lat=bol.Latitud, Lng=bol.Longitud }), Color.Fuchsia, 3)
            ;

            return pintor;
        }

        static void PintarBoletosYRecorridoTeorico(IEnumerable<BoletoComun> boletos, RecorridoLinBan recorridoTeorico)
        {
            var lineas = new int[] { 166 };
            var date = DateTime.Now;
            var dirRecorridos = "../../../../Datos/ZipRepo/";
            var granularidad = 20;

            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos(dirRecorridos, lineas, date);

            var todosLosPuntosRec = recorridosRBus
                .SelectMany(
                    (reco) => reco.Puntos.HacerGranular(granularidad),
                    (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
                )
                .ToList()
            ;

            // topes
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosRec);

            // pinturitas
            var pintor = new PintorDeRecorrido(topes2D, granularidad);

            pintor
                .SetColorFondo(Color.Gray)
                .PintarPuntos(recorridosRBus.Where(reco => reco.Linea == 166 && reco.Bandera == 1082).First().Puntos.HacerGranular(granularidad), Color.LimeGreen, 3)
                .PintarPuntos(todosLosPuntosRec, Color.DarkGray)
            //.PintarPunto(pBoleto, Color.Fuchsia, 3);
            ;

            // imagen
            using var bitmap = pintor.Render();
            bitmap.Save($"boleto_en_reco_out.png", ImageFormat.Png);
        }


        static void PintarRecorridos(int[] lineas, DateTime date, string dirRecorridos, int granularidad)
        {
            // recorridos y puntos
            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos(dirRecorridos, lineas, date);

            var todosLosPuntosRec = recorridosRBus
                .SelectMany(
                    (reco) => reco.Puntos.HacerGranular(granularidad),
                    (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
                )
                .ToList()
            ;

            // topes
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosRec);

            // pinturitas
            var pintor = new PintorDeRecorrido(topes2D, granularidad);

            pintor
                .SetColorFondo(Color.Gray)
                .PintarPuntos(recorridosRBus.Where(reco => reco.Linea == 166 && reco.Bandera == 1082).First().Puntos.HacerGranular(granularidad), Color.LimeGreen, 3)
                .PintarPuntos(todosLosPuntosRec, Color.DarkGray)
                //.PintarPunto(pBoleto, Color.Fuchsia, 3);
            ;

            // imagen
            using var bitmap = pintor.Render();
            bitmap.Save($"boleto_en_reco_out.png", ImageFormat.Png);
        }

        /*
        static void Main_(string[] args)
        {
            // params:
            var lineas = new int[] { 159, 163 };
            var date = DateTime.Now;
            var dir = "../../../../Datos/ZipRepo/";
            var granu = 20;
            var radioPuntas = 200;

            // recorridos y puntos
            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos(dir, lineas, date);

            var todosLosPuntosRec = recorridosRBus
                .SelectMany(
                    (reco) => reco.Puntos.HacerGranular(granu),
                    (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
                )
                .ToList()
            ;

            // puntas de línea
            var puntas = PuntasDeLinea.GetPuntasNombradas(
                recorridosRBus,
                radioPuntas
            );

            var puntas2 = PuntasDeLinea2.GetPuntasDeLinea(
                recorridosRBus,                     // una lista de recorridos
                radioPuntas,                        // el radio de detección de cada item de la punta de línea (puede ser mas que uno)
                radioAgrupacion: radioPuntas * 2    // la máxima distancia que pueden tener las puntas de línea para ser incluidas en un mismo grupo entre si
            )
                .ToList()
            ;

            var puntas2Nombradas = PuntasDeLinea2.GetPuntasNombradas(
                recorridosRBus,
                radioPuntas,
                radioAgrupacion: radioPuntas * 2
            );


            var indexxx = 0;
            foreach (var ppp in puntas2)
            {
                ppp.Nombre = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[indexxx].ToString();
                indexxx += 1;
            }

            var queseya = Camino<Punto>.CreateFromPuntos(puntas, recorridosRBus[17].Puntos);
            var queseyo = Camino<Punto>.CreateFromPuntos(puntas2, recorridosRBus[17].Puntos);

            // inicializar objetos
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosRec);

            // pintar (ver problema index 6)
            foreach (var rec in recorridosRBus)
            {
                var pintor = new PintorDeRecorrido(topes2D, granu)

                    //.SetColorFondo(Color.Aquamarine)
                    .SetColorFondo(Color.Gray)

                    //.PintarPuntos(recorridosRBus.Select(rec => rec.PuntoSalida), Color.Fuchsia, 15)
                    //.PintarPuntos(recorridosRBus.Select(rec => rec.PuntoLlegada), Color.Fuchsia, 15)

                    //.PintarPuntos(rec.Puntos, Color.Lime, 3)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 159), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 163), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 165), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 166), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 167), Color.DarkGray)

                //.PintarRadio(recorridosRBus[index].Puntos.First(), Color.Yellow, 800 / granu)
                //.PintarRadio(recorridosRBus[index].Puntos.Last (), Color.Yellow, 800 / granu)
                //.PintarRadios(puntas.Select(pu => pu.Punto), Color.Aqua, radioPuntas / granu)
                //.PintarRadiosNombrados(puntas.Select(pu => (pu.Punto, pu.Nombre)), Color.LightBlue, radioPuntas / (granu / 2))

                ;

                var random = new Random(Environment.TickCount);
                var colori = 40;
                foreach (PuntaLinea2 pun2 in puntas2)
                {
                    //var colorRandom = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    var colorRandom = Color.FromKnownColor((KnownColor)colori);
                    pintor.PintarPuntos(pun2.Puntos, Color.FromArgb(255, colorRandom), radioPuntas * 2 / granu);
                    pintor.PintarPuntos(pun2.Puntos, Color.Black, 3);
                    pintor.PintarPuntos(pun2.Puntos, Color.White, 1);
                    colori += 1;
                }

                pintor.PintarPuntos(rec.Puntos, Color.FromArgb(50, Color.Lime), 3);

                pintor.PintarPunto(rec.Puntos.First(), Color.Fuchsia, 5);
                pintor.PintarPunto(rec.Puntos.Last(), Color.Blue, 5);

                using (var bitmap = pintor.Render())
                {
                    bitmap.Save($"rec_{rec.Linea:0000}_{rec.Bandera:0000}.png", ImageFormat.Png);
                }

                int fin = 0;

            }



            static void Main(string[] args)
        {
            // params:
            var lineas      = new int[] { 159, 163 };
            var date        = DateTime.Now;
            var dir         = "../../../../Datos/ZipRepo/";
            var granu       = 20;
            var radioPuntas = 200;

            // recorridos y puntos
            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos(dir, lineas, date);

            var todosLosPuntosRec = recorridosRBus
                .SelectMany(
                    (reco) => reco.Puntos.HacerGranular(granu),
                    (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
                )
                .ToList()
            ;

            // puntas de línea
            var puntas = PuntasDeLinea.GetPuntasNombradas(
                recorridosRBus, 
                radioPuntas
            );

            var puntas2 = PuntasDeLinea2.GetPuntasDeLinea(
                recorridosRBus,                     // una lista de recorridos
                radioPuntas,                        // el radio de detección de cada item de la punta de línea (puede ser mas que uno)
                radioAgrupacion: radioPuntas * 2    // la máxima distancia que pueden tener las puntas de línea para ser incluidas en un mismo grupo entre si
            )
                .ToList()
            ;

            var puntas2Nombradas = PuntasDeLinea2.GetPuntasNombradas(
                recorridosRBus,
                radioPuntas,
                radioAgrupacion: radioPuntas * 2
            );


            var indexxx = 0;
            foreach (var ppp in puntas2)
            {
                ppp.Nombre = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[indexxx].ToString();
                indexxx += 1;
            }

            var queseya = Camino<Punto>.CreateFromPuntos(puntas , recorridosRBus[17].Puntos);
            var queseyo = Camino<Punto>.CreateFromPuntos(puntas2, recorridosRBus[17].Puntos);

            // inicializar objetos
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosRec);

            // pintar (ver problema index 6)
            foreach (var rec in recorridosRBus)
            {
                var pintor = new PintorDeRecorrido(topes2D, granu)

                    //.SetColorFondo(Color.Aquamarine)
                    .SetColorFondo(Color.Gray)

                    //.PintarPuntos(recorridosRBus.Select(rec => rec.PuntoSalida), Color.Fuchsia, 15)
                    //.PintarPuntos(recorridosRBus.Select(rec => rec.PuntoLlegada), Color.Fuchsia, 15)

                    //.PintarPuntos(rec.Puntos, Color.Lime, 3)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 159), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 163), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 165), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 166), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 167), Color.DarkGray)

                //.PintarRadio(recorridosRBus[index].Puntos.First(), Color.Yellow, 800 / granu)
                //.PintarRadio(recorridosRBus[index].Puntos.Last (), Color.Yellow, 800 / granu)
                //.PintarRadios(puntas.Select(pu => pu.Punto), Color.Aqua, radioPuntas / granu)
                //.PintarRadiosNombrados(puntas.Select(pu => (pu.Punto, pu.Nombre)), Color.LightBlue, radioPuntas / (granu / 2))

                ;

                var random = new Random(Environment.TickCount);
                var colori = 40;
                foreach (PuntaLinea2 pun2 in puntas2)
                {
                    //var colorRandom = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    var colorRandom = Color.FromKnownColor((KnownColor)colori);
                    pintor.PintarPuntos(pun2.Puntos, Color.FromArgb(255, colorRandom), radioPuntas * 2 / granu);
                    pintor.PintarPuntos(pun2.Puntos, Color.Black, 3);
                    pintor.PintarPuntos(pun2.Puntos, Color.White, 1);
                    colori += 1;
                }

                pintor.PintarPuntos(rec.Puntos, Color.FromArgb(50, Color.Lime), 3);

                pintor.PintarPunto(rec.Puntos.First(), Color.Fuchsia, 5);
                pintor.PintarPunto(rec.Puntos.Last(), Color.Blue, 5);

                using (var bitmap = pintor.Render())
                {
                    bitmap.Save($"rec_{rec.Linea:0000}_{rec.Bandera:0000}.png", ImageFormat.Png);
                }

                int fin = 0;
            }
        }
        */
    }
}
