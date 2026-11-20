using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Misterio.Core;

// Genera muchos casos de cada nivel y guarda sus datos en un CSV para analizarlos después
public static class ExperimentsMenu
{
    const int CasesPerLevel = 1000;

    // sin filtro de dificultad, para ver cómo se reparte y elegir los rangos
    [MenuItem("Misterio/Experimentos/Sin rango")]
    static void WithoutRange()
    {
        Run("sin_rango.csv", true);
    }

    // con el rango de cada nivel, para ver cuántos intentos y cuánto tiempo hacen falta
    [MenuItem("Misterio/Experimentos/Con rango")]
    static void WithRange()
    {
        Run("con_rango.csv", false);
    }

    static void Run(string file, bool withoutRange)
    {
        Difficulty[] difficulties = { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard };
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("nivel,semilla,intentos,pistas,directas,pasos,coste,paso_max,cadena,decisiones,microsegundos");

        CaseGenerator.Generate(Difficulty.Easy, 0); // la primera vez va más lenta y no se cuenta

        try
        {
            for (int d = 0; d < difficulties.Length; d++)
            {
                LevelSettings level = LevelSettings.For(difficulties[d]);
                if (withoutRange)
                {
                    level.MinCost = 0;
                    level.MaxCost = int.MaxValue;
                }

                for (int seed = 0; seed < CasesPerLevel; seed++)
                {
                    float progress = (d * CasesPerLevel + seed) / (float)(difficulties.Length * CasesPerLevel);
                    if (seed % 10 == 0 && EditorUtility.DisplayCancelableProgressBar("Experimentos", difficulties[d] + ", semilla " + seed, progress))
                        return;

                    // solo se mide lo que tarda el generador
                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
                    GeneratedCase generated = CaseGenerator.Generate(level, seed);
                    watch.Stop();
                    long micros = watch.ElapsedTicks * 1000000 / System.Diagnostics.Stopwatch.Frequency;

                    csv.Append(difficulties[d] + "," + seed + ",");
                    if (generated == null)
                    {
                        csv.AppendLine(",,,,,,,," + micros); // no ha salido ningún caso
                        continue;
                    }
                    CaseDefinition c = generated.Case;
                    HumanResult human = generated.Human;
                    csv.AppendLine(generated.Attempts + "," + c.Clues.Count + "," + CaseGenerator.CountDirect(c) + "," + human.Steps.Count + ","
                        + human.TotalCost() + "," + human.HardestStep() + "," + human.LongestChain() + "," + CompleteSolver.CountDecisions(c) + "," + micros);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // en la carpeta del proyecto, fuera de Assets
        string folder = Path.Combine(Application.dataPath, "..", "Experimentos");
        Directory.CreateDirectory(folder);
        string path = Path.GetFullPath(Path.Combine(folder, file));
        File.WriteAllText(path, csv.ToString());
        Debug.Log("Experimento guardado en " + path);
    }
}