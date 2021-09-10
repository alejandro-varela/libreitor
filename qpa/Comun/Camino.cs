using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comun;

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

        // debe retornar tres cosas:
        //    enpunta...
        //    no punta...
        //    indet (por estar muy cerca del borde de lo que es una punta o no)
        static CaminoParteTristate EnPuntaTristate(Punto punto, IEnumerable<PuntaLinea> puntas)
        {
            foreach (var puntaX in puntas)
            {
                var dist = Haversine.GetDist(punto, puntaX.Punto);

                if (dist < puntaX.Radio)
                {
                    return new CaminoParteTristate
                    {
                        Type = CaminoParteTristate.PuntoTristateType.Punta,
                        Punta = puntaX
                    };
                }

                if (dist < (puntaX.Radio + 150))
                {
                    return new CaminoParteTristate
                    {
                        Type = CaminoParteTristate.PuntoTristateType.Indet
                    };
                }
            }

            return new CaminoParteTristate
            {
                Type = CaminoParteTristate.PuntoTristateType.Normal
            };
        }

        public static Camino<TPunto> CreateFromPuntos(IEnumerable<PuntaLinea> puntas, IEnumerable<TPunto> puntos) 
        {
            Camino<TPunto> camino = new Camino<TPunto>();

            foreach (var puntoX in puntos)
            {
                var puntaTristate = EnPuntaTristate(puntoX, puntas);

                switch (puntaTristate.Type)
                {
                    case CaminoParteTristate.PuntoTristateType.Punta:
                        camino.Nodos.Add(new PuntoCamino<TPunto>
                        {
                            PuntaDeLinea  = puntaTristate.Punta,
                            PuntoAsociado = puntoX
                        });
                        break;
                    case CaminoParteTristate.PuntoTristateType.Normal:
                        camino.Nodos.Add(new PuntoCamino<TPunto>
                        {
                            PuntaDeLinea  = new PuntaLinea { Nombre = "." },
                            PuntoAsociado = puntoX
                        });
                        break;
                    case CaminoParteTristate.PuntoTristateType.Indet:
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
