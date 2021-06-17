using Recorridos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PruebaLecturaDeRecorridos
{
    public class Pruebas
    {
        // ENCONTRAR LAS MINIMAS DIFERENCIAS ENTRE RECORRIDOS EQUIVALENTES
        // SI HAY PRESENTE UNA MINIMA DIFERENCIA ENTONCES, UNIVOCAMENTE, ES ESE RECORRIDO ...
        //  -> PARA EMPEZAR A HACER ALGO UTIL: SUMAR LAS BANDERAS DE CADA SUBHISTORIA => RANKING
        //  -> DETECTAR (para cada recorrido) SI EN EL MEDIO TIENEN UNA CERCANIA PELIGROSA A ALGUNA PUNTA DE LINEA
        //      -> LA MINIMA CERCANIA ES EL MINIMO QUE PODREMOS USAR PARA DETERMINAR LOS CORTES POR PUNTA DE LINEA
        //  -> AVERIGUAR ESAS DIFERENCIAS Y QUE HACE "UNICO" A UN RECORRIDO
        // AL FINAL, SI LAS PUNTAS EQUIVALEN, ENTONCES HAY COINCIDENCIA...

        public const int GRANU = 15;

        public  static void PruebaPuntosDesechados(
            IEnumerable<RecorridoLinBan> recorridosRBus, 
            Topes2D topes2D
        )
        {
            var puntosLinBan = recorridosRBus.SelectMany(
                (reco) => reco.Puntos,
                (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
            );

            // 30 35! 50

            var cantidadPuntosLinBan = puntosLinBan.Count();
            HashSet<Casillero> casilleros = new();
            foreach (var plbX in puntosLinBan)
            {
                var casillero = Casillero.Create(topes2D, plbX, GRANU);
                casilleros.Add(casillero);
            }
            var cantidadCasilleros = casilleros.Count;
            int foo = 0;

            // filtramos ahora un reco real...
            var puntosHistoricos = GetFromCSV(
                "2021-06-02.csv", 
                3850, 
                new DateTime(2021, 6, 2), 
                new DateTime(2021, 6, 3), 
                new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }
            ).ToList();


            int start = Environment.TickCount;
            //for (int i=0; i<1000; i++)
            foreach (var phX in puntosHistoricos)
            {
                var errores = 0;
                var perfect = 0;
                var bias = 0;

                var casillero = Casillero.Create(topes2D, phX.Punto, GRANU);
                if (casilleros.Contains(casillero))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("**");
                    perfect++;
                }
                else
                {
                    // nor-oeste
                    if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal - 1, IndexVertical = casillero.IndexVertical - 1 }))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("no");
                        bias++;
                    }
                    // norte puro
                    else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal, IndexVertical = casillero.IndexVertical - 1 }))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("nn");
                        bias++;
                    }
                    // nor-este
                    else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 1, IndexVertical = casillero.IndexVertical - 1 }))
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("ne");
                        bias++;
                    }
                    // este
                    else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 1, IndexVertical = casillero.IndexVertical }))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("ee");
                        bias++;
                    }
                    // sur este
                    else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 1, IndexVertical = casillero.IndexVertical + 1 }))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("se");
                        bias++;
                    }
                    // sur
                    else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal, IndexVertical = casillero.IndexVertical + 1 }))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("ss");
                        bias++;
                    }
                    // sur oeste
                    else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal - 1, IndexVertical = casillero.IndexVertical + 1 }))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("so");
                        bias++;
                    }
                    // oeste
                    else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal - 1, IndexVertical = casillero.IndexVertical }))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("oo");
                        bias++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("XX");
                        errores++;
                    }
                    
                    //Console.WriteLine(casillero);
                }
            }
            int end = Environment.TickCount;
            int milis = end - start;

            int faa = 0;
        }


        // TODO:
        //       DETECTAR LOS CASILLEROS QUE TIENEN UN "ÚNICO RECORRIDO" Y COMPARARLOS SIN FLEX O VERIFICAR CUAN FLEX PUEDEN SER...

        public static void EnQueRecoEstaEstePunto(List<RecorridoLinBan> recorridosRBus, Topes2D topes2D)
        {
            Console.WriteLine("EMPIEZA");

            /////////////////////////////////////////////
            // Creo los casilleros para cada recorrido

            Dictionary<string, HashSet<Casillero>> casillerosXLinBan = new();

            foreach (RecorridoLinBan reco in recorridosRBus)
            {
                var key = $"{reco.Linea:0000}{reco.Bandera:0000}";
                casillerosXLinBan.Add(key, new HashSet<Casillero>());

                foreach (var puntoX in reco.Puntos.HacerGranular(15))
                {
                    var casillero = Casillero.Create(topes2D, puntoX, GRANU);
                    casillerosXLinBan[key].Add(casillero);
                }

                if (reco.Linea == 163 && reco.Bandera == 2777)
                {
                    int parameAca = 0;
                }
            }

            /////////////////////////////////////////////
            // Obtengo las puntas de línea
            var puntasDeLinea = PuntasDeLinea
               .Get(recorridosRBus)
               .ToList()
            ;

            /////////////////////////////////////////////
            // Tomo los puntos de ejemplo...
            // filtramos ahora un reco real...
            var puntosHistoricos = GetFromCSV(
                "2021-06-02.csv",
                3850,
                new DateTime(2021, 6, 2),
                new DateTime(2021, 6, 3),
                new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }
            ).ToList();

            /////////////////////////////////////////////
            // Ahora, para cada punto, muestro que recorrido lo contiene...

            //Dictionary<string, int> acumBanderaas = new();
            Dictionary<string, List<PuntoHistorico>> acumBanderaas = new();

            foreach (var phx in puntosHistoricos)
            {
                bool esPunta = PuntasDeLinea.EsPunta(phx, puntasDeLinea, 400);

                if (esPunta)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    if (acumBanderaas.Keys.Count > 0)
                    {
                        var listOrdenada = acumBanderaas.ToList().OrderByDescending(kvp => kvp.Value.Count);
                        foreach (var kvp in listOrdenada)
                        {
                            var sConcuerda = ConcuerdaPorCuenta(kvp, recorridosRBus) ? "SI" : "_";
                            Console.WriteLine($"{kvp.Key} {kvp.Value.Count} {sConcuerda}");
                        }
                        acumBanderaas = new Dictionary<string, List<PuntoHistorico>>();
                    }
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                var casilleroActual = Casillero.Create(topes2D, phx.Punto, GRANU);
                Console.Write(phx.Punto);
                foreach (var key in casillerosXLinBan.Keys)
                {
                    // if (casillerosXBandera[key].Contains(casilleroActual))
                    if (ContieneCasilleroFlex(casillerosXLinBan[key], casilleroActual, GRANU))
                    {
                        if (!esPunta)
                        {
                            if (!acumBanderaas.ContainsKey(key))
                            {
                                //acumBanderaas.Add(key, 0);
                                acumBanderaas.Add(key, new List<PuntoHistorico>());
                            }
                            //acumBanderaas[key] += 1;
                            acumBanderaas[key].Add(phx);
                        }

                        Console.Write(key);
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }

            // 2777 WTF?????????????????

            int foo = 0;
        }


        public static void PintarRecorridos(List<RecorridoLinBan> recorridosRBus, Topes2D topes2D, int granularidad, string nombreArchivo)
        {
            int ancho   = topes2D.GetAnchoGranular  (granularidad);
            int altura  = topes2D.GetAlturaGranular (granularidad);

            var bitmap = new Bitmap(width: ancho, height: altura);

            var cont = 0;
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // fondo:
                Color colorFondo = Color.DarkSlateGray;
                Brush brushFondo = new SolidBrush(colorFondo);
                g.FillRectangle(brushFondo, new Rectangle(0, 0, ancho, altura));

                // puntos de recorrido:
                //Color color = Color.FromArgb(alpha: 50, red: 255, green: 0, blue: 0);
                
                foreach (var reco in recorridosRBus)
                {
                    Color color = Color.FromArgb(alpha: 200, Color.FromName(Colores[cont]));
                    Brush brush = new SolidBrush(color);

                    foreach (var pt in reco.Puntos)
                    {
                        var casillero = Casillero.Create(topes2D, pt, granularidad);
                        var pos = new Point { X = casillero.IndexHorizontal, Y = casillero.IndexVertical };
                        var size = new Size(3, 3);
                        Rectangle rect = new Rectangle(pos, size);
                        g.FillEllipse(brush, rect);
                    }

                    cont++;
                }

                
            }

            bitmap.Save(nombreArchivo, System.Drawing.Imaging.ImageFormat.Png);
        }

        //public static Bitmap PintarRecorridos(Topes2D topes2D, int granularidad, params TareaDePintura[] tareasDePintura)
        //{
        //    int ancho = topes2D.GetAnchoGranular(granularidad);
        //    int altura = topes2D.GetAlturaGranular(granularidad);

        //    var bitmap = new Bitmap(width: ancho, height: altura);

        //    using (Graphics g = Graphics.FromImage(bitmap))
        //    {

        //    }

        //    return bitmap;
        //}


        static bool ConcuerdaPorCuenta(List<Punto> puntos, IEnumerable<PuntoRecorrido> recorrido)
        {
            if (puntos == null)
            {
                throw new ArgumentNullException(nameof(recorrido));
            }

            if (recorrido == null)
            {
                throw new ArgumentNullException(nameof(recorrido));
            }

            // si la lista de puntos es menor que 2 no puedo averiguar el "sentido"
            if (puntos.Count < 2)
            {
                return false;
            }

            // tomo el primero y el último punto de la lista
            var primerPuntoHistorico = puntos[0];
            var ultimoPuntoHistorico = puntos[puntos.Count - 1]; // ^1 se podría usar este index operator

            // con esos puntos puedo tratar de averiguar las cuentas y saber si concuerda o no con ese recorrido...
            var primeraCuenta = Recorrido.DameCuentaMasCercanaA(primerPuntoHistorico, recorrido);
            var ultimaCuenta =  Recorrido.DameCuentaMasCercanaA(ultimoPuntoHistorico, recorrido);

            return primeraCuenta < ultimaCuenta;
        }

        static bool ConcuerdaPorCuenta(List<Punto> puntos, Recorrido recorrido)
        {
            return ConcuerdaPorCuenta(puntos, recorrido.Puntos);
        }

        static bool ConcuerdaPorCuenta(int linea, int bandera, List<Punto> puntos, IEnumerable<RecorridoLinBan> recorridosRBus)
        {
            var recorridoLinBan = recorridosRBus.FirstOrDefault(r => r.Linea == linea && r.Bandera == bandera);
            return ConcuerdaPorCuenta(puntos, recorridoLinBan);
        }

        static bool ConcuerdaPorCuenta(KeyValuePair<string, List<PuntoHistorico>> kvp, IEnumerable<RecorridoLinBan> recorridosRBus)
        {
            // en la clave de tipo string, me dice que linea y bandera es
            var linea = int.Parse(kvp.Key.Substring(0, 4));
            var bande = int.Parse(kvp.Key.Substring(4, 4));

            var puntos = kvp.Value.Select(ph => ph.Punto).ToList();

            return ConcuerdaPorCuenta(
                linea,
                bande,
                puntos,
                recorridosRBus
            );
        }

        static bool ContieneCasilleroFlex(HashSet<Casillero> casilleros, Casillero casillero, int granularidad)
        {
            if (casilleros.Contains(casillero))
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 0, IndexVertical = casillero.IndexVertical - 1 })) // norte
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal - 1, IndexVertical = casillero.IndexVertical - 1 })) // nor oeste
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 1, IndexVertical = casillero.IndexVertical - 1 })) // nor este
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 0, IndexVertical = casillero.IndexVertical + 1 })) // sur
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal - 1, IndexVertical = casillero.IndexVertical + 1 })) // sur oeste
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 1, IndexVertical = casillero.IndexVertical + 1 })) // sur este
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal - 1, IndexVertical = casillero.IndexVertical + 0 })) // oeste
            {
                return true;
            }
            else if (casilleros.Contains(new Casillero { IndexHorizontal = casillero.IndexHorizontal + 1, IndexVertical = casillero.IndexVertical + 0 })) // este
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<PuntoHistorico> GetFromCSV(string path, int ficha, DateTime desde, DateTime hasta, PuntosHistoricosGetFromCSVConfig config)
        {
            double latInverter = config.InvertLat ? -1 : 1;
            double lngInverter = config.InvertLng ? -1 : 1;

            foreach (string line in File.ReadLines(path)) // "2021-06-02.csv"
            {
                // parto la linea
                string[] lineParts = line.Split(config.Separator);

                // leo ficha
                int fichaX = int.Parse(lineParts[config.FichaPos]);
                if (fichaX != ficha)
                {
                    continue;
                }

                // leo la fecha
                DateTime fechaHoraX = DateTime.Parse(lineParts[config.FechaHoraPos]);
                if (fechaHoraX < desde || fechaHoraX > hasta)
                {
                    continue;
                }

                // leo lat lng
                double latX = double.Parse(lineParts[config.LatitudPos]) * latInverter;
                double lngX = double.Parse(lineParts[config.LongitudPos]) * lngInverter;

                // elimino de los resultados los valores en 0
                if (config.ExcluirCeros)
                {
                    if (latX == 0.0 || lngX == 0.0)
                    {
                        continue;
                    }
                }

                yield return new PuntoHistorico
                {
                    Fecha = fechaHoraX,
                    Punto = new Punto
                    {
                        Lat = latX,
                        Lng = lngX,
                    }
                };
            }

        }

        public static readonly string[] Colores = new string[] {
            "MediumAquamarine",
            "MediumBlue",
            "MediumOrchid",
            "MediumPurple",
            "MediumSeaGreen",
            "MediumSlateBlue",
            "MediumSpringGreen",
            "Maroon",
            "MediumTurquoise",
            "MidnightBlue",
            "MintCream",
            "MistyRose",
            "Moccasin",
            "NavajoWhite",
            "Navy",
            "OldLace",
            "MediumVioletRed",
            "Magenta",
            "Linen",
            "LimeGreen",
            "LavenderBlush",
            "LawnGreen",
            "LemonChiffon",
            "LightBlue",
            "LightCoral",
            "LightCyan",
            "LightGoldenrodYellow",
            "LightGray",
            "LightGreen",
            "LightPink",
            "LightSalmon",
            "LightSeaGreen",
            "LightSkyBlue",
            "LightSlateGray",
            "LightSteelBlue",
            "LightYellow",
            "Lime",
            "Olive",
            "OliveDrab",
            "Orange",
            "OrangeRed",
            "Silver",
            "SkyBlue",
            "SlateBlue",
            "SlateGray",
            "Snow",
            "SpringGreen",
            "SteelBlue",
            "Tan",
            "Teal",
            "Thistle",
            "Tomato",
            "Transparent",
            "Turquoise",
            "Violet",
            "Wheat",
            "White",
            "WhiteSmoke",
            "Sienna",
            "Lavender",
            "SeaShell",
            "SandyBrown",
            "Orchid",
            "PaleGoldenrod",
            "PaleGreen",
            "PaleTurquoise",
            "PaleVioletRed",
            "PapayaWhip",
            "PeachPuff",
            "Peru",
            "Pink",
            "Plum",
            "PowderBlue",
            "Purple",
            "Red",
            "RosyBrown",
            "RoyalBlue",
            "SaddleBrown",
            "Salmon",
            "SeaGreen",
            "Yellow",
            "Khaki",
            "Cyan",
            "DarkMagenta",
            "DarkKhaki",
            "DarkGreen",
            "DarkGray",
            "DarkGoldenrod",
            "DarkCyan",
            "DarkBlue",
            "Ivory",
            "Crimson",
            "Cornsilk",
            "CornflowerBlue",
            "Coral",
            "Chocolate",
            "DarkOliveGreen",
            "Chartreuse",
            "BurlyWood",
            "Brown",
            "BlueViolet",
            "Blue",
            "BlanchedAlmond",
            "Black",
            "Bisque",
            "Beige",
            "Azure"
        };

    }

    public class PintorDeRecorrido
    {
        // Límites
        public Topes2D Topes2D { get; private set; }
        public int Granularidad { get; private set; }

        // Tamaño
        public int Width  { get; private set; }
        public int Height { get; private set; }

        // Color de fondo
        public Color ColorFondo { get; private set; } = Color.White;
        public List<ITarea> Tareas { get; private set; }

        public PintorDeRecorrido(Topes2D topes2D, int granularidad)
        {
            Topes2D         = topes2D;
            Granularidad    = granularidad;
            Width           = topes2D.GetAnchoGranular(granularidad);
            Height          = topes2D.GetAlturaGranular(granularidad);
            Tareas          = new();
        }

        public PintorDeRecorrido SetColorFondo(Color color)
        {
            ColorFondo = color;
            return this;
        }

        public PintorDeRecorrido PintarPunto(Punto punto, Color color, int size = 1)
        {
            Tareas.Add(
                new TareaPunto { 
                    Punto = punto, 
                    Color = color, 
                    Size  = size, 
                    Granularidad = Granularidad, 
                    Topes2D = Topes2D 
                }
            );
            return this;
        }

        public PintorDeRecorrido PintarPuntos(IEnumerable<Punto> puntos, Color color, int size = 1)
        {
            foreach (var p in puntos)
            {
                PintarPunto(p, color, size);
            }

            return this;
        }

        public Bitmap Render()
        {
            // tamaño
            var bitmap = new Bitmap(Width, Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // fondo
                g.FillRectangle(new SolidBrush(ColorFondo), new Rectangle(0, 0, Width, Height));

                // tareas de puntos...
                foreach (ITarea itarea in Tareas)
                {
                    itarea.Paint(g);
                }
            }

            return bitmap;
        }

        public interface ITarea
        {
            public void Paint(Graphics g);
        }

        public class TareaPunto : ITarea
        {
            public Punto    Punto       { get; set; }
            public Color    Color       { get; set; }
            public int      Size        { get; set; }
            public Topes2D  Topes2D     { get; set; }
            public int      Granularidad{ get; set; }

            public void Paint(Graphics g)
            {
                var casillero = Casillero.Create(Topes2D, Punto, Granularidad);

                if (Size == 1)
                {
                    g.FillRectangle(
                        new SolidBrush(Color), 
                        casillero.IndexHorizontal, 
                        casillero.IndexVertical, 
                        1, 
                        1
                    );
                }
                else
                {
                    Rectangle rect = new()
                    {
                        X = casillero.IndexHorizontal,
                        Y = casillero.IndexVertical,
                        Width = Size,
                        Height = Size,
                    };

                    g.FillEllipse(new SolidBrush(Color), rect);
                }
            }
        }
    }
}
