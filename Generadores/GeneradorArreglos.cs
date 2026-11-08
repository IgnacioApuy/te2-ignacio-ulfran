namespace TareaExtraclase2.Generadores;

public static class GeneradorArreglos
{
    private const int Semilla = 676767;
    private const int MaximoExclusivo = 999_999_999;

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
}