"""Combina los CSV de Resultados, calcula medianas y genera gráficos.

Uso:
    python ProcesarResultados.py
    python ProcesarResultados.py --input-dir Resultados --output-dir Resultados/Analisis
"""

import argparse
import csv
import statistics
import sys
from collections import defaultdict
from decimal import Decimal, InvalidOperation
from pathlib import Path

try:
    import matplotlib
except ImportError as error:
    raise SystemExit(
        "Falta matplotlib. Instálelo con: python -m pip install -r requirements.txt"
    ) from error

matplotlib.use("Agg")
import matplotlib.pyplot as plt


ALGORITHMS = (
    "Bubble Sort",
    "Insertion Sort",
    "Merge Sort",
    "Quick Sort",
    "Radix Sort",
    "Selection Sort",
    "Shell Sort",
)
ARRAY_TYPES = ("Aleatorio", "Ascendente", "Descendente", "Aleatorio_0_a_10")
TIME_COLUMNS = ("TiempoTicks", "TiempoMilisegundos")
REQUIRED_COLUMNS = (
    "Iteracion",
    "Tamano",
    "TipoArreglo",
    "Algoritmo",
    *TIME_COLUMNS,
)


def parse_arguments():
    project_dir = Path(__file__).resolve().parent
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--input-dir",
        type=Path,
        default=project_dir / "Resultados",
        help="Carpeta con los CSV crudos (por defecto: Resultados).",
    )
    parser.add_argument(
        "--output-dir",
        type=Path,
        help="Carpeta de salida (por defecto: <input-dir>/Analisis).",
    )
    arguments = parser.parse_args()
    if arguments.output_dir is None:
        arguments.output_dir = arguments.input_dir / "Analisis"
    return arguments


def discover_csv_files(input_dir, output_dir):
    if not input_dir.is_dir():
        raise ValueError(f"No existe la carpeta de entrada: {input_dir}")

    output_resolved = output_dir.resolve()
    files = []
    for path in sorted(input_dir.rglob("*")):
        if not path.is_file() or path.suffix.lower() != ".csv":
            continue
        if path.resolve().is_relative_to(output_resolved):
            continue
        files.append(path)

    if not files:
        raise ValueError(f"No se encontraron archivos CSV en: {input_dir}")
    return files


def load_results(csv_files):
    fieldnames = None
    rows = []
    for path in csv_files:
        with path.open("r", newline="", encoding="utf-8-sig") as source:
            reader = csv.DictReader(source)
            if reader.fieldnames is None:
                raise ValueError(f"El archivo no contiene encabezados: {path}")
            if not set(REQUIRED_COLUMNS).issubset(reader.fieldnames):
                raise ValueError(
                    f"El archivo no contiene las columnas requeridas "
                    f"{', '.join(REQUIRED_COLUMNS)}: {path}"
                )
            if fieldnames is None:
                fieldnames = reader.fieldnames
            elif reader.fieldnames != fieldnames:
                raise ValueError(f"Los encabezados no coinciden con los demás CSV: {path}")

            for line_number, row in enumerate(reader, start=2):
                if None in row or any(value is None for value in row.values()):
                    raise ValueError(
                        f"La cantidad de valores no coincide con los encabezados "
                        f"en {path}, línea {line_number}."
                    )
                if row["Algoritmo"] not in ALGORITHMS:
                    raise ValueError(
                        f"Algoritmo desconocido en {path}, línea {line_number}: "
                        f"{row['Algoritmo']}"
                    )
                try:
                    for column in TIME_COLUMNS:
                        row[column] = Decimal(row[column])
                        if not row[column].is_finite():
                            raise InvalidOperation
                    int(row["Iteracion"])
                    int(row["Tamano"])
                except (InvalidOperation, TypeError, ValueError) as error:
                    raise ValueError(
                        f"Valor numérico inválido en {path}, línea {line_number}."
                    ) from error
                rows.append(row)

    if not rows:
        raise ValueError("Los archivos CSV no contienen filas de resultados.")
    return fieldnames, rows


def median_rows(fieldnames, rows):
    key_columns = [
        column for column in fieldnames
        if column not in TIME_COLUMNS and column != "Iteracion"
    ]
    grouped = defaultdict(list)
    for row in rows:
        key = tuple(row[column] for column in key_columns)
        grouped[key].append(row)

    medians = []
    for samples in grouped.values():
        result = {
            column: samples[0][column]
            for column in fieldnames
            if column not in TIME_COLUMNS
        }
        for column in TIME_COLUMNS:
            result[column] = statistics.median(sample[column] for sample in samples)
        medians.append(result)

    algorithm_order = {algorithm: index for index, algorithm in enumerate(ALGORITHMS)}
    type_order = {array_type: index for index, array_type in enumerate(ARRAY_TYPES)}
    medians.sort(
        key=lambda row: (
            algorithm_order[row["Algoritmo"]],
            int(row["Tamano"]),
            type_order.get(row["TipoArreglo"], len(type_order)),
            row["TipoArreglo"],
            int(row["Iteracion"]),
        )
    )
    return medians


def format_decimal(value):
    text = format(value, "f")
    if "." in text:
        text = text.rstrip("0").rstrip(".")
    return text


def write_csv(path, fieldnames, rows):
    with path.open("w", newline="", encoding="utf-8") as destination:
        writer = csv.DictWriter(destination, fieldnames=fieldnames)
        writer.writeheader()
        for row in rows:
            writer.writerow(
                {
                    column: (
                        format_decimal(value)
                        if isinstance(value, Decimal)
                        else value
                    )
                    for column, value in row.items()
                }
            )


def calculate_growth_ratios(median_results):
    results_by_case = {
        (row["Algoritmo"], row["TipoArreglo"], int(row["Tamano"])): row
        for row in median_results
    }
    ratios = []
    for (algorithm, array_type, size), previous in results_by_case.items():
        following = results_by_case.get((algorithm, array_type, size * 2))
        if following is None:
            continue
        previous_time = previous["TiempoMilisegundos"]
        current_time = following["TiempoMilisegundos"]
        ratios.append(
            {
                "Algoritmo": algorithm,
                "TipoArreglo": array_type,
                "TamanoAnterior": size,
                "TamanoActual": size * 2,
                "TiempoAnteriorMilisegundos": previous_time,
                "TiempoActualMilisegundos": current_time,
                "RazonT2nSobreTn": (
                    current_time / previous_time if previous_time > 0 else ""
                ),
            }
        )
    return ratios


def log_plot_time(value):
    if value < 0:
        raise ValueError("Los tiempos negativos no se pueden representar en el gráfico.")
    return value if value > 0 else Decimal("0.00005")


def save_graphs(output_dir, median_results):
    by_algorithm_size = defaultdict(list)
    for row in median_results:
        if row["TipoArreglo"] == "Aleatorio":
            by_algorithm_size[(row["Algoritmo"], int(row["Tamano"]))].append(
                row["TiempoMilisegundos"]
            )

    figure, axis = plt.subplots(figsize=(10, 6))
    has_algorithm_data = False
    for algorithm in ALGORITHMS:
        sizes = sorted(
            size
            for name, size in by_algorithm_size
            if name == algorithm
        )
        if not sizes:
            continue
        times = [
            log_plot_time(
                statistics.median(by_algorithm_size[(algorithm, size)])
            )
            for size in sizes
        ]
        if any(size <= 0 for size in sizes):
            raise ValueError("El gráfico log-log requiere tamaños positivos.")
        axis.plot(sizes, times, marker="o", label=algorithm)
        has_algorithm_data = True

    axis.set_xscale("log")
    axis.set_yscale("log")
    axis.set_title("Comparación de algoritmos (arreglos aleatorios)")
    axis.set_xlabel("Tamaño del arreglo")
    axis.set_ylabel("Tiempo mediano (ms; 0 se muestra como 0.00005)")
    axis.grid(True, which="both", linestyle="--", alpha=0.4)
    if has_algorithm_data:
        axis.legend()
    else:
        axis.text(
            0.5,
            0.5,
            "No hay resultados de entrada aleatoria.",
            ha="center",
            va="center",
            transform=axis.transAxes,
        )
    figure.tight_layout()
    figure.savefig(output_dir / "comparacion_algoritmos_loglog.png", dpi=160)
    plt.close(figure)

    figure, axis = plt.subplots(figsize=(12, 7))
    colors = {"Insertion Sort": "tab:blue", "Quick Sort": "tab:orange"}
    line_styles = {
        "Aleatorio": "-",
        "Ascendente": "--",
        "Descendente": ":",
        "Aleatorio_0_a_10": "-.",
    }
    has_comparison_data = False
    for algorithm in ("Insertion Sort", "Quick Sort"):
        for array_type in ARRAY_TYPES:
            points = sorted(
                (
                    int(row["Tamano"]),
                    row["TiempoMilisegundos"],
                )
                for row in median_results
                if row["Algoritmo"] == algorithm
                and row["TipoArreglo"] == array_type
            )
            if not points:
                continue
            sizes, times = zip(*points)
            if any(size <= 0 for size in sizes):
                raise ValueError("El gráfico log-log requiere tamaños positivos.")
            times = [log_plot_time(time) for time in times]
            axis.plot(
                sizes,
                times,
                color=colors[algorithm],
                linestyle=line_styles[array_type],
                marker="o",
                label=f"{algorithm} - {array_type}",
            )
            has_comparison_data = True

    axis.set_xscale("log")
    axis.set_yscale("log")
    axis.set_title("Insertion Sort y Quick Sort por tipo de arreglo")
    axis.set_xlabel("Tamaño del arreglo")
    axis.set_ylabel("Tiempo mediano (ms; 0 se muestra como 0.00005)")
    axis.grid(True, which="both", linestyle="--", alpha=0.4)
    if has_comparison_data:
        axis.legend(ncol=2)
    else:
        axis.text(
            0.5,
            0.5,
            "No hay resultados de Insertion Sort ni Quick Sort.",
            ha="center",
            va="center",
            transform=axis.transAxes,
        )
    figure.tight_layout()
    figure.savefig(output_dir / "insertion_quick_por_tipo.png", dpi=160)
    plt.close(figure)


def main():
    arguments = parse_arguments()
    csv_files = discover_csv_files(arguments.input_dir, arguments.output_dir)
    fieldnames, raw_rows = load_results(csv_files)
    median_results = median_rows(fieldnames, raw_rows)

    arguments.output_dir.mkdir(parents=True, exist_ok=True)
    write_csv(arguments.output_dir / "todos_resultados.csv", fieldnames, raw_rows)

    for algorithm in ALGORITHMS:
        safe_name = algorithm.replace(" ", "")
        algorithm_rows = [
            row for row in median_results if row["Algoritmo"] == algorithm
        ]
        write_csv(
            arguments.output_dir / f"{safe_name}_mediana.csv",
            fieldnames,
            algorithm_rows,
        )

    write_csv(
        arguments.output_dir / "medianas_algoritmos.csv",
        fieldnames,
        median_results,
    )
    write_csv(
        arguments.output_dir / "razones_t2n.csv",
        [
            "Algoritmo",
            "TipoArreglo",
            "TamanoAnterior",
            "TamanoActual",
            "TiempoAnteriorMilisegundos",
            "TiempoActualMilisegundos",
            "RazonT2nSobreTn",
        ],
        calculate_growth_ratios(median_results),
    )
    save_graphs(arguments.output_dir, median_results)

    print(f"CSV crudos combinados: {len(csv_files)} archivo(s), {len(raw_rows)} fila(s).")
    print(f"Resultados y gráficos guardados en: {arguments.output_dir}")


if __name__ == "__main__":
    try:
        main()
    except (OSError, ValueError, csv.Error) as error:
        print(f"Error: {error}", file=sys.stderr)
        raise SystemExit(1) from error
