using System;
using System.Collections.Generic;
using System.Linq;

namespace Comun
{
    public partial class Camino<TPunto> where TPunto : Punto
    {
        public List<PuntoCamino<TPunto>> Nodos { get; set; }  = new List<PuntoCamino<TPunto>>();

        public List<Grupoide<TPunto>> Grupoides
        {
            get
            {
                List<Grupoide<TPunto>> ret = new List<Grupoide<TPunto>>();
                string actual = null;

                foreach (PuntoCamino<TPunto> nodox in Nodos)
                {
                    if (actual != nodox.PuntaDeLinea.Nombre)
                    {
                        actual = nodox.PuntaDeLinea.Nombre;
                        ret.Add(new Grupoide<TPunto> { 
                            Nombre = nodox.PuntaDeLinea.Nombre, 
                            PuntaLinea = nodox.PuntaDeLinea 
                        });
                    }
                    
                    ret[ret.Count - 1].Nodos.Add(nodox);
                }

                return QuitarRuido( ret );
            }
        }

        public string Description
        {
            get
            {
                var arr = DescriptionRawSinRuido
                    .Where(s => s != '.' && s != '?')
                    .ToArray()
                ;

                return string.Join(string.Empty, arr); 
            }
        }

        public string DescriptionRawSinRuido
        {
            get
            {
                return QuitarRuido(DescriptionRaw);
            }
        }

        public string DescriptionRaw
        {
            get
            {
                var arr = Nodos
                    .Select(puntoCamino => puntoCamino.PuntaDeLinea.Nombre)
                    .Simplificar((nombre1, nombre2) => nombre1 == nombre2)
                    .ToArray()
                ;

                return string.Join(string.Empty, arr);
            }
        }

        public string DescriptionRawSinSimplificar
        {
            get
            {
                var arr = Nodos
                    .Select(puntoCamino => puntoCamino.PuntaDeLinea.Nombre)
                    .ToArray()
                ;

                return string.Join(string.Empty, arr);
            }
        }

        // TODO: explicar
        static string QuitarRuido(string sRuidosa)
        {
            var simbols = sRuidosa
                .ToCharArray()
                .Distinct()
                .Where(c => c != '?')
                .Where(c => c != '.')
                .ToArray()
            ;

            foreach (var simbol in simbols)
            {
                string deforme = $"{simbol}?{simbol}";
                sRuidosa = sRuidosa.Replace($"{simbol}?{simbol}", simbol.ToString());
            }

            return sRuidosa;
        }

        static List<Grupoide<T>> QuitarRuido<T>(List<Grupoide<T>> grupoidesRuidosos) 
            where T : Punto
        {
            var nombresDeGrupoides = grupoidesRuidosos
                .Select(g => g.Nombre)
                .Distinct()
                .Where(n => n != "?")
                .Where(n => n != ".")
                .ToArray()
            ;

            var nuevaLista = grupoidesRuidosos;

            foreach (var nombre in nombresDeGrupoides)
            {
                for (; ; )
                {
                    var indexRuido = IndexPatronRuidoso(nuevaLista, nombre);

                    if (indexRuido == -1)
                    {
                        break;
                    }
                    else
                    {
                        nuevaLista = ReemplazarPatronRuidoso(nuevaLista, indexRuido);
                    }
                }
            }

            return nuevaLista;
        }

        static List<Grupoide<T>> ReemplazarPatronRuidoso<T>(List<Grupoide<T>> grupoides, int indexRuido)
            where T : Punto
        {
            List<Grupoide<T>> ret = new List<Grupoide<T>>();

            for (int i = 0; i < grupoides.Count; i++)
            {
                if (i < indexRuido)
                {
                    ret.Add(grupoides[i]);
                }
                else if (i == indexRuido)
                {
                    Grupoide<T> grupoideNuevo = new Grupoide<T>()
                    {
                        Nombre = grupoides[i].Nombre
                    };

                    grupoideNuevo.Nodos.AddRange(grupoides[i + 0].Nodos);
                    grupoideNuevo.Nodos.AddRange(grupoides[i + 1].Nodos);
                    grupoideNuevo.Nodos.AddRange(grupoides[i + 2].Nodos);

                    ret.Add(grupoideNuevo);
                }
                else if (i > indexRuido + 2)
                {
                    ret.Add(grupoides[i]);
                }
            }

            return ret;
        }

        static int IndexPatronRuidoso<T>(List<Grupoide<T>> grupoides, string nombre) 
            where T : Punto
        {
            for (int i = 0; i < grupoides.Count - 2; i++)
            {
                if (grupoides[i + 0].Nombre == nombre &&
                    grupoides[i + 1].Nombre == "?" &&
                    grupoides[i + 2].Nombre == nombre)
                {
                    return i;
                }
            }

            return -1;
        }

        // Devuelve el estado de cualquier punto y su punta asociada si la tiene.
        // Posibles valores de retorno:
        //  EstadoPunto.Punta , PuntaAsociada   : el punto se encuentra adentro de una punta de línea, se devuelve la punta de linea asociada también.
        //  EstadoPunto.Indet , null            : el punto se encuentra en un borde entre la punta de línea y los puntos normales. El ancho del borde esta dado por la variable "anchoBordeIndeterminacion" 
        //  EstadoPunto.Normal, null            : el punto se encuentra afuera de todas las puntas de línea dadas
        static (EstadoPuntoEnPunta, IPuntaLinea) GetEstadoPuntoYPuntaAsoc(
            Punto punto,                        // un punto cualquiera
            IEnumerable<IPuntaLinea> puntas     // una colección de puntas de línea
        )
        {
            //bool huboIndet = false;

            //foreach (IPuntaLinea ptx in puntas)
            //{
            //    var llamadas1 = Haversine.GetLlamadas();
            //    var info = ptx.GetInformacionPunto(punto);
            //    var llamadas2 = Haversine.GetLlamadas();

            //    if (info.Estado == EstadoPuntoEnPunta.Punta)
            //    { 
            //        return (EstadoPuntoEnPunta.Punta, ptx);
            //    }

            //    if (info.Estado == EstadoPuntoEnPunta.Indet)
            //    {
            //        huboIndet = true;
            //    }
            //}

            //if (huboIndet)
            //{
            //    return (EstadoPuntoEnPunta.Indet, null);
            //}

            //return (EstadoPuntoEnPunta.Normal, null);

            // SUBTLE:
            // *** ¿por qué hago esto en vez de hacer un foreach y encontrar la primera punta o indet? ***
            // *** si lo primero que encuentro es un "indet" terminaria devolviendo indet y probablemente el prox valor sea una punta válida...
            // *** por eso primero mapeo todas las puntas al punto
            // *** me fijo si hay alguna entrada en el mapping que sea "punta" (si la hay retorno punta)
            // *** si no la hay, me fijo si hay alguna entrada que sea "indet" (si la hay retorno indet)
            // *** por ultimo, si no encontré nada, develvo "normal"


            // creo pares "estado, punta" en donde el primer item me dice el estado del punto respecto a la punta y el segundo sería la punta de línea
            List<(EstadoPuntoEnPunta, IPuntaLinea)> estadosYPuntas = puntas
                .Select(puntaX => (puntaX.GetInformacionPunto(punto).Estado, puntaX))
                .ToList()
            ;

            // ¿hay algun par que sea Punta?
            if (estadosYPuntas.Any(parEstadoPunta => parEstadoPunta.Item1 == EstadoPuntoEnPunta.Punta))
            {
                // devuelvo ese par
                return estadosYPuntas
                    .Where(parEstadoPunta => parEstadoPunta.Item1 == EstadoPuntoEnPunta.Punta)
                    .First()
                ;
            }

            // ¿hay algun par que sea Indet?
            else if (estadosYPuntas.Any(parEstadoPunta => parEstadoPunta.Item1 == EstadoPuntoEnPunta.Indet))
            {
                // devuelvo indet
                return (EstadoPuntoEnPunta.Indet, null);
            }
            
            // ok, entonces es normal...
            else
            {
                // devuelvo normal
                return (EstadoPuntoEnPunta.Normal, null);
            }

        }

        public static Camino<TPunto> CreateFromPuntos(IEnumerable<IPuntaLinea> puntas, IEnumerable<TPunto> puntos)
        {
            Camino<TPunto> camino = new Camino<TPunto>();

            foreach (var puntoX in puntos)
            {
                //var llamadas1 = Haversine.GetLlamadas();
                var (estadoPunto, puntaAsociada) = GetEstadoPuntoYPuntaAsoc(puntoX, puntas);
                //var llamadas2 = Haversine.GetLlamadas();

                switch (estadoPunto)
                {
                    case EstadoPuntoEnPunta.Punta:
                        camino.Nodos.Add(new PuntoCamino<TPunto>
                        {
                            PuntaDeLinea  = puntaAsociada,
                            PuntoAsociado = puntoX
                        });
                        break;
                    case EstadoPuntoEnPunta.Normal:
                        camino.Nodos.Add(new PuntoCamino<TPunto>
                        {
                            PuntaDeLinea  = new PuntaLinea { Nombre = "." },
                            PuntoAsociado = puntoX
                        });
                        break;
                    case EstadoPuntoEnPunta.Indet:
                        camino.Nodos.Add(new PuntoCamino<TPunto>
                        {
                            PuntaDeLinea  = new PuntaLinea { Nombre = "?" },
                            PuntoAsociado = puntoX
                        });
                        break;
                }
            }

            return camino;
        }
    }
}
