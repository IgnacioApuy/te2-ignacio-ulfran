namespace TareaExtraclase2.Algoritmos;

public static class ShellSort
{
    public static void Ordenar(int[] arreglo)
    {
        int n = arreglo.Length;

        for (int brecha = n / 2; brecha > 0; brecha /= 2)
        {
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