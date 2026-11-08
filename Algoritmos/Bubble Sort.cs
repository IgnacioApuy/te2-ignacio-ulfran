namespace TareaExtraclase2.Algoritmos;

public static class BubbleSort
{
    public static void Ordenar(int[] arreglo)
    {
        int n = arreglo.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (arreglo[j] > arreglo[j + 1])
                {
                    int temporal = arreglo[j];
                    arreglo[j] = arreglo[j + 1];
                    arreglo[j + 1] = temporal;
                }
            }
        }
    }
}