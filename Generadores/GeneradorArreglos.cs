namespace TareaExtraclase2.Generadores;

public static class GeneradorArreglos
{
    private const int Semilla = 676767;
    private const int MaximoExclusivo = 1_000_000;

    public static int[] CrearArreglo(int cantidad, string tipo)
    {
        var arreglo = new int[cantidad];
        var generador = new Random(Semilla);
        for (int indice = 0; indice < cantidad; indice++)
        {
            int valor = generador.Next(MaximoExclusivo);
            arreglo[indice] = tipo == "Aleatorio_0_a_10" ? valor % 11 : valor;
        }

        if (tipo == "Ascendente")
        {
            Array.Sort(arreglo);
        }
        else if (tipo == "Descendente")
        {
            Array.Sort(arreglo);
            Array.Reverse(arreglo);
        }

        return arreglo;
    }

    public static int[] CrearArregloConMaximoDigitos(int cantidad, int digitos)
    {
        int maximoExclusivo = digitos switch
        {
            3 => 1_000,
            6 => 1_000_000,
            9 => 1_000_000_000,
            _ => throw new ArgumentOutOfRangeException(
                nameof(digitos),
                "La prueba especial de Radix admite 3, 6 o 9 dígitos.")
        };

        var arreglo = new int[cantidad];
        var generador = new Random(Semilla);
        for (int indice = 0; indice < cantidad; indice++)
        {
            arreglo[indice] = generador.Next(1_000_000_000) % maximoExclusivo;
        }

        return arreglo;
    }
}