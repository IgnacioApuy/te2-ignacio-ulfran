using System.Diagnostics;
using System.Globalization;
using TareaExtraclase2.Generadores;

namespace TareaExtraclase2.Benchmark;

public static class EjecutorBenchmark
{
    private const int Repeticiones = 5;

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
        string archivoResultados)
    {
        using var escritor = new StreamWriter(archivoResultados, append: false);
        escritor.WriteLine(
            "Iteracion,Tamano,TipoArreglo,Algoritmo,TiempoTicks,TiempoMilisegundos");
        escritor.Flush();

        int iteracion = 1;
        for (int tamano = minN; ; )
        {
            foreach (string tipo in TiposArreglo)
            {
                Medicion[] mediciones = Medir(
                    iteracion,
                    tamano,
                    tipo,
                    nombreAlgoritmo,
                    ordenar);

                foreach (Medicion medicion in mediciones)
                {
                    escritor.WriteLine(
                        $"{medicion.Iteracion},{medicion.Tamano},{medicion.TipoArreglo},{medicion.Algoritmo}," +
                        $"{medicion.TiempoTicks}," +
                        medicion.TiempoMilisegundos.ToString("F6", CultureInfo.InvariantCulture));
                    escritor.Flush();
                    iteracion++;
                }
            }

            if (tamano > maxN / 2)
            {
                break;
            }

            tamano *= 2;
        }
    }

    public static void EjecutarPruebaEspecialRadix(
        int tamano,
        Action<int[]> ordenar,
        string archivoResultados)
    {
        int[] cantidadesDigitos = [3, 6, 9];
        using var escritor = new StreamWriter(archivoResultados, append: false);
        escritor.WriteLine(
            "Iteracion,Tamano,TipoArreglo,Algoritmo,TiempoTicks,TiempoMilisegundos");
        escritor.Flush();

        int iteracion = 1;
        foreach (int digitos in cantidadesDigitos)
        {
            int[] entrada = GeneradorArreglos.CrearArregloConMaximoDigitos(
                tamano,
                digitos);

            int[] esperado = (int[])entrada.Clone();
            Array.Sort(esperado);
            int[] calentamiento = (int[])entrada.Clone();
            ordenar(calentamiento);
            ValidarContraReferencia(calentamiento, esperado, digitos);

            var mediciones = new (long Ticks, double Milisegundos)[Repeticiones];
            for (int repeticion = 0; repeticion < mediciones.Length; repeticion++)
            {
                int[] arreglo = (int[])entrada.Clone();
                var cronometro = Stopwatch.StartNew();
                ordenar(arreglo);
                cronometro.Stop();

                ValidarContraReferencia(arreglo, esperado, digitos);

                mediciones[repeticion] = (
                    cronometro.ElapsedTicks,
                    cronometro.Elapsed.TotalMilliseconds);
            }

            Array.Sort(mediciones, static (left, right) =>
                left.Ticks.CompareTo(right.Ticks));
            var mediana = mediciones[mediciones.Length / 2];
            escritor.WriteLine(
                $"{iteracion},{tamano},Digitos_{digitos},Radix Sort," +
                $"{mediana.Ticks}," +
                mediana.Milisegundos.ToString("F6", CultureInfo.InvariantCulture));
            escritor.Flush();
            iteracion++;
        }
    }

    private static Medicion[] Medir(
        int iteracion,
        int tamano,
        string tipo,
        string nombreAlgoritmo,
        Action<int[]> ordenar)
    {
        int[] entrada = GeneradorArreglos.CrearArreglo(tamano, tipo);
        int[] esperado = (int[])entrada.Clone();
        Array.Sort(esperado);

        int[] calentamiento = (int[])entrada.Clone();
        ordenar(calentamiento);
        ValidarContraReferencia(calentamiento, esperado, nombreAlgoritmo, tipo);

        var mediciones = new Medicion[Repeticiones];
        for (int repeticion = 0; repeticion < Repeticiones; repeticion++)
        {
            int[] arreglo = (int[])entrada.Clone();
            var cronometro = Stopwatch.StartNew();
            ordenar(arreglo);
            cronometro.Stop();

            ValidarContraReferencia(arreglo, esperado, nombreAlgoritmo, tipo);
            mediciones[repeticion] = new Medicion(
                iteracion,
                tamano,
                tipo,
                nombreAlgoritmo,
                cronometro.ElapsedTicks,
                cronometro.Elapsed.TotalMilliseconds);
            iteracion++;
        }

        return mediciones;
    }

    private static void ValidarContraReferencia(
        int[] resultado,
        int[] esperado,
        string nombreAlgoritmo,
        string tipoArreglo)
    {
        if (!resultado.AsSpan().SequenceEqual(esperado))
        {
            throw new InvalidOperationException(
                $"{nombreAlgoritmo} no ordenó correctamente el arreglo de tipo {tipoArreglo}.");
        }
    }

    private static void ValidarContraReferencia(
        int[] resultado,
        int[] esperado,
        int digitos)
    {
        if (!resultado.AsSpan().SequenceEqual(esperado))
        {
            throw new InvalidOperationException(
                $"Radix Sort no ordenó correctamente la entrada de {digitos} dígitos.");
        }
    }
}