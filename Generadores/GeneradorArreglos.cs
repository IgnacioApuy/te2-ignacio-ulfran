using System.Globalization;

namespace TareaExtraclase2.Generadores;

public static class GeneradorArreglos
{
    private const int Semilla = 20261107;
    private const int MaximoExclusivo = 1_000_000;

    // Conserva una secuencia reproducible en disco y la amplía si se solicita un tamaño mayor.
    public static void AsegurarDatosReferencia(string ruta, int cantidad)
    {
        long valoresExistentes = 0;
        if (File.Exists(ruta))
        {
            using var lector = new StreamReader(ruta);
            if (lector.ReadLine() != "Semilla,Valor")
            {
                throw new InvalidDataException($"El archivo de referencia no tiene un encabezado válido: {ruta}");
            }

            string? linea;
            while ((linea = lector.ReadLine()) is not null)
            {
                LeerValor(linea, ruta);
                valoresExistentes++;
            }
        }
        else
        {
            using var escritor = new StreamWriter(ruta, append: false);
            escritor.WriteLine("Semilla,Valor");
        }

        if (valoresExistentes >= cantidad)
        {
            return;
        }

        var generador = new Random(Semilla);
        for (long i = 0; i < valoresExistentes; i++)
        {
            generador.Next(MaximoExclusivo);
        }

        using var escritorAdicional = new StreamWriter(ruta, append: true);
        for (long i = valoresExistentes; i < cantidad; i++)
        {
            escritorAdicional.WriteLine(
                $"{Semilla},{generador.Next(MaximoExclusivo).ToString(CultureInfo.InvariantCulture)}");
        }
    }

    // Carga solo el arreglo de la medición actual; el ordenamiento ocurre fuera del cronómetro.
    public static int[] CrearArreglo(string ruta, int cantidad, string tipo)
    {
        var arreglo = new int[cantidad];
        using var lector = new StreamReader(ruta);
        if (lector.ReadLine() != "Semilla,Valor")
        {
            throw new InvalidDataException($"El archivo de referencia no tiene un encabezado válido: {ruta}");
        }

        int indice = 0;
        string? linea;
        while (indice < cantidad && (linea = lector.ReadLine()) is not null)
        {
            int valor = LeerValor(linea, ruta);
            arreglo[indice++] = tipo == "Aleatorio_0_a_10" ? valor % 11 : valor;
        }

        if (indice != cantidad)
        {
            throw new InvalidDataException(
                $"El archivo de referencia contiene {indice} valores; se requieren {cantidad}: {ruta}");
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

    private static int LeerValor(string linea, string ruta)
    {
        string[] columnas = linea.Split(',');
        if (columnas.Length != 2
            || !int.TryParse(columnas[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int semilla)
            || semilla != Semilla
            || !int.TryParse(columnas[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int valor)
            || valor < 0
            || valor >= MaximoExclusivo)
        {
            throw new InvalidDataException($"Fila inválida en el archivo de referencia: {ruta}");
        }

        return valor;
    }
}