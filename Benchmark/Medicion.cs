namespace TareaExtraclase2.Benchmark;

public sealed record Medicion(
    int Iteracion,
    int Tamano,
    string TipoArreglo,
    string Algoritmo,
    long TiempoTicks,
    double TiempoMilisegundos);