using TareaExtraclase2.Algoritmos;
using TareaExtraclase2.Benchmark;

Console.WriteLine("Tarea Extraclase 2 - Benchmark de algoritmos de ordenamiento");

var algoritmos = new (string Nombre, Action<int[]> Ordenar)[]
{
    ("Bubble Sort", BubbleSort.Ordenar),
    ("Insertion Sort", InsertionSort.Ordenar),
    ("Merge Sort", MergeSort.Ordenar),
    ("Quick Sort", QuickSort.Ordenar),
    ("Radix Sort", RadixSort.Ordenar),
    ("Selection Sort", SelectionSort.Ordenar),
    ("Shell Sort", ShellSort.Ordenar)
};

Console.WriteLine("Seleccione el algoritmo:");
for (int i = 0; i < algoritmos.Length; i++)
{
    Console.WriteLine($"{i + 1}. {algoritmos[i].Nombre}");
}
Console.WriteLine($"{algoritmos.Length + 1}. Radix Sort (Especial)");

int opcion = LeerOpcion(algoritmos.Length + 1);
string carpetaResultados = Path.Combine(Directory.GetCurrentDirectory(), "Resultados");
Directory.CreateDirectory(carpetaResultados);

if (opcion == algoritmos.Length + 1)
{
    const int tamanoMaximoEspecial = 1_000_000;
    int tamano = LeerEnteroPositivoHasta(
        $"Ingrese n (recomendado: 100000; máximo: {tamanoMaximoEspecial}): ",
        tamanoMaximoEspecial);
    string archivoEspecial = CrearRutaResultados(
        carpetaResultados,
        "RadixSortEspecial",
        tamano);

    EjecutorBenchmark.EjecutarPruebaEspecialRadix(
        tamano,
        RadixSort.Ordenar,
        archivoEspecial);

    Console.WriteLine($"Prueba especial finalizada. Resultados: {archivoEspecial}");
}
else
{
    int minN = LeerEnteroPositivo("Ingrese minN: ");
    int maxN = LeerEnteroPositivo("Ingrese maxN: ");
    while (maxN < minN)
    {
        Console.WriteLine("maxN debe ser mayor o igual que minN.");
        maxN = LeerEnteroPositivo("Ingrese maxN: ");
    }

    string nombreAlgoritmo = algoritmos[opcion - 1].Nombre.Replace(" ", string.Empty);
    string archivoResultados = CrearRutaResultados(
        carpetaResultados,
        nombreAlgoritmo,
        maxN);

    EjecutorBenchmark.Ejecutar(
        minN,
        maxN,
        algoritmos[opcion - 1].Nombre,
        algoritmos[opcion - 1].Ordenar,
        archivoResultados);

    Console.WriteLine($"Benchmark finalizado. Resultados: {archivoResultados}");
}

static string CrearRutaResultados(string carpeta, string nombre, int tamano)
{
    int indiceIteracion = 0;
    string archivoResultados;
    do
    {
        archivoResultados = Path.Combine(
            carpeta,
            $"{nombre}_IT{indiceIteracion}_{tamano}.csv");
        indiceIteracion++;
    }
    while (File.Exists(archivoResultados));

    return archivoResultados;
}

static int LeerEnteroPositivo(string mensaje)
{
    while (true)
    {
        Console.Write(mensaje);
        if (int.TryParse(Console.ReadLine(), out int valor) && valor > 0)
        {
            return valor;
        }

        Console.WriteLine("Ingrese un entero mayor que cero.");
    }
}

static int LeerEnteroPositivoHasta(string mensaje, int maximo)
{
    while (true)
    {
        int valor = LeerEnteroPositivo(mensaje);
        if (valor <= maximo)
        {
            return valor;
        }

        Console.WriteLine($"El tamaño debe ser menor o igual que {maximo}.");
    }
}

static int LeerOpcion(int cantidadOpciones)
{
    while (true)
    {
        Console.Write("Opción: ");
        if (int.TryParse(Console.ReadLine(), out int opcion)
            && opcion >= 1
            && opcion <= cantidadOpciones)
        {
            return opcion;
        }

        Console.WriteLine($"Seleccione un número entre 1 y {cantidadOpciones}.");
    }
}
