# Análisis de los experimentos. Saca las tablas y la gráfica para la memoria

from pathlib import Path

import matplotlib.pyplot as plt
import pandas as pd

CARPETA = Path(__file__).parent
NIVELES = {"Easy": "Fácil", "Normal": "Normal", "Hard": "Difícil"}


def leer(nombre):
    datos = pd.read_csv(CARPETA / nombre)
    datos["nivel"] = datos["nivel"].map(NIVELES)
    return datos


def por_nivel(datos):
    return datos.groupby("nivel", sort=False)


def resumen(datos):
    filas = {}
    for nivel, d in por_nivel(datos):
        filas[nivel] = {
            "Válidos (%)": round(100 * len(d) / d["intentos"].sum(), 1),
            "Pistas": round(d["pistas"].mean(), 1),
            "Directas": round(d["directas"].mean(), 1),
            "Pasos": round(d["pasos"].mean(), 1),
            "Dificultad": f"{d['coste'].min():.0f} a {d['coste'].max():.0f}",
            "Mediana": round(d["coste"].median()),
            "Paso más difícil": round(d["paso_max"].mean(), 2),
            "Cadena": round(d["cadena"].mean(), 1),
            "Solo propagando (%)": round(100 * (d["decisiones"] == 0).mean(), 1),
            "Tiempo (ms)": round(d["microsegundos"].mean() / 1000, 2),
        }
    return pd.DataFrame(filas).T


# Spearman 
def correlaciones(datos):
    medidas = ["coste", "paso_max", "cadena", "decisiones"]
    grupos = [("Todos", datos)] + list(por_nivel(datos))
    filas = {nivel: d[medidas].corr(method="spearman")["coste"].drop("coste") for nivel, d in grupos}
    return pd.DataFrame(filas).T.round(2)


# Prueba todos los cortes entre niveles y se queda con el que hace que el nivel que más
# casos descarta descarte lomenos posible. Si empatan, el que menos descarta en total
def rangos(datos):
    coste = {nivel: d["coste"] for nivel, d in por_nivel(datos)}
    minimo, maximo = int(datos["coste"].min()), int(datos["coste"].max())
    mejor = None
    for a in range(minimo, maximo + 1):
        for b in range(a + 1, maximo + 2):
            acepta = [(coste["Fácil"] < a).mean(), coste["Normal"].between(a, b - 1).mean(), (coste["Difícil"] >= b).mean()]
            clave = (min(acepta), sum(acepta))
            if mejor is None or clave > mejor[0]:
                mejor = (clave, a, b, acepta)
    return mejor[1], mejor[2], mejor[3]


def grafica(datos, a, b, fichero):
    plt.rcParams["font.sans-serif"] = ["Arial", "Liberation Sans", "DejaVu Sans"]
    rango = {"Fácil": (0, a - 1), "Normal": (a, b - 1), "Difícil": (b, 99)}
    inicio, fin = datos["coste"].min() - 1, datos["coste"].max() + 1

    figura, ejes = plt.subplots(3, 1, figsize=(6.3, 5.6), sharex=True, sharey=True)
    for eje, (nivel, d) in zip(ejes, por_nivel(datos)):
        cuenta = d["coste"].value_counts().sort_index()
        desde, hasta = rango[nivel]
        eje.bar(cuenta.index, cuenta.values, width=0.75, color="#2a78d6", zorder=2, label="Casos")
        eje.axvspan(max(desde, inicio) - 0.5, min(hasta, fin) + 0.5, color="#e9e8e4", zorder=0, label="Rango del nivel")
        eje.set_title(nivel, loc="left", fontsize=10, color="#0b0b0b")
        eje.set_ylabel("Casos", color="#52514e")
        eje.grid(axis="y", color="#dddcd7", linewidth=0.6, zorder=1)
        eje.tick_params(colors="#52514e", length=0)
        for lado in ["top", "right", "left"]:
            eje.spines[lado].set_visible(False)
        eje.spines["bottom"].set_color("#b5b4ae")
    ejes[0].legend(frameon=False, fontsize=8, loc="upper right", labelcolor="#52514e")
    ejes[-1].set_xlim(inicio, fin)
    ejes[-1].set_xlabel("Dificultad (suma de costes del solucionador humano)", color="#52514e")
    figura.tight_layout()
    figura.savefig(fichero, dpi=200)


pd.set_option("display.width", 200)
sin_rango = leer("sin_rango.csv")
print("Sin rango, 1.000 casos por nivel\n")
print(resumen(sin_rango).to_string(), "\n")
print("Correlación de Spearman con la dificultad\n")
print(correlaciones(sin_rango).to_string(), "\n")

a, b, acepta = rangos(sin_rango)
print(f"Rangos propuestos. Fácil 0 a {a - 1}, Normal {a} a {b - 1}, Difícil {b} a 99")
print("Casos que acepta cada nivel (%)", [round(100 * float(x), 1) for x in acepta], "\n")
grafica(sin_rango, a, b, CARPETA / "dificultad.png")

if (CARPETA / "con_rango.csv").exists():
    con_rango = leer("con_rango.csv")
    print("Con rango\n")
    filas = {}
    for nivel, d in por_nivel(con_rango):
        filas[nivel] = {
            "Sin caso": int(d["coste"].isna().sum()),
            "Dificultad": f"{d['coste'].min():.0f} a {d['coste'].max():.0f}",
            "Intentos": round(d["intentos"].mean(), 2),
            "Intentos máx.": int(d["intentos"].max()),
            "Tiempo (ms)": round(d["microsegundos"].mean() / 1000, 2),
            "Tiempo máx. (ms)": round(d["microsegundos"].max() / 1000, 1),
        }
    print(pd.DataFrame(filas).T.to_string())