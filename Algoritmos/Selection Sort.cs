namespace TareaExtraclase2.Algoritmos;

public static class SelectionSort
{
    public static void Ordenar(int[] arreglo)
    {
        int n = arreglo.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int indiceMinimo = i;
            for (int j = i + 1; j < n; j++)
            {
                if (arreglo[j] < arreglo[indiceMinimo])
                {
                    indiceMinimo = j;
                }
            }
            int temporal = arreglo[i];
            arreglo[i] = arreglo[indiceMinimo];
            arreglo[indiceMinimo] = temporal;
        }
    }
}