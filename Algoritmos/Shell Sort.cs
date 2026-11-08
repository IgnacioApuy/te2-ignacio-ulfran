namespace TareaExtraclase2.Algoritmos;

public static class ShellSort
{
    public static void Ordenar(int[] arreglo)
    {
        int n = arreglo.Length;

        // Secuencia de brechas original de Shell: n/2, n/4, ..., 1
        for (int brecha = n / 2; brecha > 0; brecha /= 2)
        {
            // Insertion Sort, pero comparando elementos separados por "brecha"
            for (int i = brecha; i < n; i++)
            {
                int clave = arreglo[i];
                int j = i;

                while (j >= brecha && arreglo[j - brecha] > clave)
                {
                    arreglo[j] = arreglo[j - brecha];
                    j -= brecha;
                }

                arreglo[j] = clave;
            }
        }
    }
}