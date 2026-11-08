namespace TareaExtraclase2.Algoritmos;

public static class MergeSort
{
    public static void Ordenar(int[] arreglo)
    {
        if (arreglo.Length < 2) return;
        int[] auxiliar = new int[arreglo.Length];
        OrdenarRecursivo(arreglo, auxiliar, 0, arreglo.Length - 1);
    }

    private static void OrdenarRecursivo(int[] arreglo, int[] auxiliar, int izq, int der)
    {
        if (izq >= der) return; 

        int medio = izq + (der - izq) / 2;

        OrdenarRecursivo(arreglo, auxiliar, izq, medio);   
        OrdenarRecursivo(arreglo, auxiliar, medio + 1, der); 
        Mezclar(arreglo, auxiliar, izq, medio, der);           
    }
    private static void Mezclar(int[] arreglo, int[] auxiliar, int izq, int medio, int der)
    {
        for (int x = izq; x <= der; x++)
        {
            auxiliar[x] = arreglo[x];
        }

        int i = izq;        
        int j = medio + 1;   
        int k = izq;         
        while (i <= medio && j <= der)
        {
            if (auxiliar[i] <= auxiliar[j])
            {
                arreglo[k++] = auxiliar[i++];
            }
            else
            {
                arreglo[k++] = auxiliar[j++];
            }
        }
        while (i <= medio)
        {
            arreglo[k++] = auxiliar[i++];
        }

        while (j <= der)
        {
            arreglo[k++] = auxiliar[j++];
        }
    }
}