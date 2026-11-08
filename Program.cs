using TareaExtraclase2.Algoritmos;
using TareaExtraclase2.Benchmark;
using TareaExtraclase2.Generadores;

Console.WriteLine("Tarea Extraclase 2 - Benchmark de algoritmos de ordenamiento");

int minN = LeerEnteroPositivo("Ingrese minN: ");
int maxN = LeerEnteroPositivo("Ingrese maxN: ");
while (maxN < minN)
{
    Console.WriteLine("maxN debe ser mayor o igual que minN.");
    maxN = LeerEnteroPositivo("Ingrese maxN: ");
}

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

int opcion = LeerOpcion(algoritmos.Length);
string carpetaResultados = Path.Combine(Directory.GetCurrentDirectory(), "Resultados");
Directory.CreateDirectory(carpetaResultados);

string archivoReferencia = Path.Combine(carpetaResultados, "datos_referencia.csv");
GeneradorArreglos.AsegurarDatosReferencia(archivoReferencia, maxN);

string nombreSeguro = algoritmos[opcion - 1].Nombre.Replace(' ', '_');
string archivoResultados = Path.Combine(
    carpetaResultados,
    $"resultados_{nombreSeguro}_{DateTime.Now:yyyyMMdd_HHmmss_fffffff}_{Guid.NewGuid():N}.csv");

EjecutorBenchmark.Ejecutar(
    minN,
    maxN,
    algoritmos[opcion - 1].Nombre,
    algoritmos[opcion - 1].Ordenar,
    archivoReferencia,
    archivoResultados);

Console.WriteLine($"Benchmark finalizado. Resultados: {archivoResultados}");

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
