namespace TareaExtraclase2.Algoritmos;

public static class RadixSort
{
    public static void Ordenar(int[] arreglo)
    {
        if (arreglo.Length < 2) return;

        int maximo = arreglo[0];
        for (int i = 1; i < arreglo.Length; i++)
        {
            if (arreglo[i] > maximo) maximo = arreglo[i];
        }

        int[] salida = new int[arreglo.Length];

        for (int exponente = 1; maximo / exponente > 0; exponente *= 10)
        {
            OrdenarPorDigito(arreglo, salida, exponente);
        }
    }

    private static void OrdenarPorDigito(int[] arreglo, int[] salida, int exponente)
    {
        int[] conteo = new int[10]; 
        for (int i = 0; i < arreglo.Length; i++)
        {
            int digito = (arreglo[i] / exponente) % 10;
            conteo[digito]++;
        }
        for (int d = 1; d < 10; d++)
        {
            conteo[d] += conteo[d - 1];
        }
        for (int i = arreglo.Length - 1; i >= 0; i--)
        {
            int digito = (arreglo[i] / exponente) % 10;
            conteo[digito]--;
            salida[conteo[digito]] = arreglo[i];
        }
        for (int i = 0; i < arreglo.Length; i++)
        {
            arreglo[i] = salida[i];
        }
    }
}