using System.Diagnostics;
using System.Globalization;
using TareaExtraclase2.Generadores;

namespace TareaExtraclase2.Benchmark;

public static class EjecutorBenchmark
{
    private static readonly string[] TiposArreglo =
    [
        "Aleatorio",
        "Ascendente",
        "Descendente",
        "Aleatorio_0_a_10"
    ];

    // Mide y registra cada arreglo por separado para no mantener los cuatro en memoria.
    public static void Ejecutar(
        int minN,
        int maxN,
        string nombreAlgoritmo,
        Action<int[]> ordenar,
        string archivoReferencia,
        string archivoResultados)
    {
        using var escritor = new StreamWriter(archivoResultados, append: false);
        escritor.WriteLine(
            "Iteracion,Tamano,TipoArreglo,Algoritmo,TiempoTicks,TiempoMilisegundos");

        int iteracion = 1;
        for (int tamano = minN; ; )
        {
            foreach (string tipo in TiposArreglo)
            {
                Medicion medicion = Medir(
                    iteracion,
                    tamano,
                    tipo,
                    nombreAlgoritmo,
                    ordenar,
                    archivoReferencia);

                escritor.WriteLine(
                    $"{medicion.Iteracion},{medicion.Tamano},{medicion.TipoArreglo},{medicion.Algoritmo}," +
                    $"{medicion.TiempoTicks}," +
                    medicion.TiempoMilisegundos.ToString("F6", CultureInfo.InvariantCulture));
                iteracion++;
            }

            if (tamano > maxN / 2)
            {
                break;
            }

            tamano *= 2;
        }
    }

    private static Medicion Medir(
        int iteracion,
        int tamano,
        string tipo,
        string nombreAlgoritmo,
        Action<int[]> ordenar,
        string archivoReferencia)
    {
        int[] arreglo = GeneradorArreglos.CrearArreglo(archivoReferencia, tamano, tipo);

        // El cronómetro abarca únicamente la llamada al algoritmo seleccionado.
        var cronometro = Stopwatch.StartNew();
        ordenar(arreglo);
        cronometro.Stop();

        for (int i = 1; i < arreglo.Length; i++)
        {
            if (arreglo[i - 1] > arreglo[i])
            {
                throw new InvalidOperationException(
                    $"{nombreAlgoritmo} no ordenó correctamente el arreglo de tipo {tipo}.");
            }
        }

        return new Medicion(
            iteracion,
            tamano,
            tipo,
            nombreAlgoritmo,
            cronometro.ElapsedTicks,
            cronometro.Elapsed.TotalMilliseconds);
    }
}