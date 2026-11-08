namespace TareaExtraclase2.Algoritmos;

public static class QuickSort
{
    public static void Ordenar(int[] arreglo)
    {
        OrdenarRecursivo(arreglo, 0, arreglo.Length - 1);
    }

    private static void OrdenarRecursivo(int[] arreglo, int inicio, int fin)
    {
        if (inicio >= fin) return;   // 0 o 1 elemento: nada que ordenar

        int posicionPivote = Particionar(arreglo, inicio, fin);

        OrdenarRecursivo(arreglo, inicio, posicionPivote - 1);   // lado izquierdo
        OrdenarRecursivo(arreglo, posicionPivote + 1, fin);      // lado derecho
    }

    // Esquema de Lomuto: el pivote es SIEMPRE el último elemento (pivote fijo)
    private static int Particionar(int[] arreglo, int inicio, int fin)
    {
        int pivote = arreglo[fin];
        int i = inicio - 1;   // última posición de la zona de "menores o iguales"

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

        // Colocamos el pivote justo después de la zona de menores o iguales
        int temp = arreglo[i + 1];
        arreglo[i + 1] = arreglo[fin];
        arreglo[fin] = temp;

        return i + 1;
    }
}