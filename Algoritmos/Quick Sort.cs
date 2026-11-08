namespace TareaExtraclase2.Algoritmos;

public static class QuickSort
{
    public static void Ordenar(int[] arreglo)
    {
        OrdenarRecursivo(arreglo, 0, arreglo.Length - 1);
    }

    private static void OrdenarRecursivo(int[] arreglo, int inicio, int fin)
    {
        if (inicio >= fin) return;   

        int posicionPivote = Particionar(arreglo, inicio, fin);

        OrdenarRecursivo(arreglo, inicio, posicionPivote - 1); 
        OrdenarRecursivo(arreglo, posicionPivote + 1, fin);     
    }

    private static int Particionar(int[] arreglo, int inicio, int fin)
    {
        int pivote = arreglo[fin];
        int i = inicio - 1;  

        for (int j = inicio; j < fin; j++)
        {
            if (arreglo[j] <= pivote)
            {
                i++;
                int temporal = arreglo[i];
                arreglo[i] = arreglo[j];
                arreglo[j] = temporal;
            }
        }

        int temp = arreglo[i + 1];
        arreglo[i + 1] = arreglo[fin];
        arreglo[fin] = temp;

        return i + 1;
    }
}