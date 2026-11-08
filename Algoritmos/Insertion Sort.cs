namespace TareaExtraclase2.Algoritmos;

public static class InsertionSort
{
    public static void Ordenar(int[] arreglo)
    {
        int n = arreglo.Length;
        for (int i = 1; i < n; i++)
        {
            int clave = arreglo[i];
            int j = i - 1;
            while (j >= 0 && arreglo[j] > clave)
            {
                arreglo[j + 1] = arreglo[j];
                j--;
            }
            arreglo[j + 1] = clave;
        }
    }
}