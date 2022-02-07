// See https://aka.ms/new-console-template for more information

var datos1 = new int[] {
    1, 2, 3, 4, 5, 6, 7, 8, 9, 13, 14, 15, 16, 17, 18, 19, 23, 24, 25, 26, 29, 30, 31, 32, 
};

var datos2 = new char[] { 'a', 'b', 'c', 'x', 'y', 'z' };

var ahora = new DateTime(2022, 2, 7, 16, 0, 0); // 2 feb 2022, 16:00 hs
var datos3 = new DateTime[] {
    ahora.AddSeconds(1),
    ahora.AddSeconds(2),
    ahora.AddSeconds(3),
    ahora.AddSeconds(1000),
    ahora.AddSeconds(1020),
    ahora.AddSeconds(1100),
};

var islas1 = GetIslas<int>(
    datos1, 
    (n1, n2) => Math.Abs(n1 - n2) < 2
).ToList();

var islas2 = GetIslas<char>(
    datos2,
    (n1, n2) => Math.Abs(n1 - n2) < 2
).ToList();

var islas3 = GetIslas<DateTime>(
    datos3,
    (d1, d2) => d2.Subtract(d1).TotalSeconds <= 100
);

int foo = 0;

static List<List<T>> GetIslas<T>(IEnumerable<T> datos, Func<T, T, bool> sonDeLaMismaIsla)
{
    if (datos == null)
    {
        throw new ArgumentNullException(nameof(datos));
    }

    List<List<T>> listaRet = new List<List<T>>();

    if (!datos.Any())
    {
        return listaRet;
    }
    
    T datoAnterior = datos.First();

    List<T> listaAux = new List<T>();
    listaAux.Add(datoAnterior);

    foreach (T datoActual in datos.Skip(1))
    {
        if (!sonDeLaMismaIsla(datoAnterior, datoActual))
        {
            listaRet.Add(listaAux);
            listaAux = new List<T>();
            //Console.WriteLine();
        }

        listaAux.Add(datoActual);
        //Console.Write(datoActual  + " ");
        datoAnterior = datoActual;
    }

    listaRet.Add(listaAux);

    return listaRet;
}

static IEnumerable<List<T>> GetIslasEnumerable<T>(IEnumerable<T> datos, Func<T, T, bool> sonDeLaMismaIsla)
{
    if (datos == null)
    { 
        throw new ArgumentNullException(nameof(datos));
    }

    if (!datos.Any())
    {
        yield break;
    }

    List<T> listaAux = new List<T>();
    T datoAnterior = datos.First();
    
    listaAux.Add(datoAnterior);
    //Console.Write(datoAnterior + " ");

    foreach (T datoActual in datos.Skip(1))
    {
        if (! sonDeLaMismaIsla(datoAnterior, datoActual))
        {
            yield return listaAux;
            listaAux = new List<T>();
            //Console.WriteLine();
        }
        
        listaAux.Add(datoActual);
        //Console.Write(datoActual  + " ");

        datoAnterior = datoActual;
    }

    yield return listaAux;
}
