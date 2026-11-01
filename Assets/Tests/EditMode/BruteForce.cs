using System.Collections.Generic;
using Misterio.Core;

namespace Misterio.Tests
{
    // Cuenta las soluciones probando todas las combinaciones posibles, sin propagar nada
    // Es simple pero sirve para comprobar el solucionador en casos pequeños
    // Comprueba las pistas por su cuenta
    public static class BruteForce
    {
        public static int Count(CaseDefinition c)
        {
            int n = c.Suspects.Count;
            List<int[]> placeOptions = Arrangements(n, c.Places.Count);
            List<int[]> activityOptions = Arrangements(n, c.Activities.Count);

            int count = 0;
            foreach (int[] placeOf in placeOptions)
            {
                foreach (int[] activityOf in activityOptions)
                {
                    if (Fits(c, placeOf, activityOf))
                        count++;
                }
            }
            return count;
        }

        // Mira si una combinación cumple las reglas y todas las pistas
        // Lo de que nadie repita lugar ni actividad ya lo hace Arrangements
        static bool Fits(CaseDefinition c, int[] placeOf, int[] activityOf)
        {
            // en la biblioteca tiene que haber alguien
            if (System.Array.IndexOf(placeOf, c.CrimePlace) == -1)
                return false;

            foreach (Clue clue in c.Clues)
            {
                SuspectClue suspectClue = clue as SuspectClue;
                if (suspectClue != null)
                {
                    int value = suspectClue.Category == Category.Place ? placeOf[suspectClue.Suspect] : activityOf[suspectClue.Suspect];
                    if ((value == suspectClue.Value) != suspectClue.Positive)
                        return false;
                }

                RelationClue relationClue = clue as RelationClue;
                if (relationClue != null)
                {
                    // miramos si alguien estaba en ese lugar haciendo esa actividad
                    bool happened = false;
                    for (int s = 0; s < placeOf.Length; s++)
                    {
                        if (placeOf[s] == relationClue.Place && activityOf[s] == relationClue.Activity)
                            happened = true;
                    }
                    if (happened != relationClue.Positive)
                        return false;
                }
            }
            return true;
        }

        // Todas las formas de dar a n sospechosos valores distintos entre 0 y m - 1
        static List<int[]> Arrangements(int n, int m)
        {
            List<int[]> result = new List<int[]>();
            Build(new int[n], 0, new bool[m], result);
            return result;
        }

        static void Build(int[] current, int index, bool[] used, List<int[]> result)
        {
            if (index == current.Length)
            {
                result.Add((int[])current.Clone());
                return;
            }
            for (int v = 0; v < used.Length; v++)
            {
                if (used[v])
                    continue;
                used[v] = true;
                current[index] = v;
                Build(current, index + 1, used, result);
                used[v] = false;
            }
        }
    }
}