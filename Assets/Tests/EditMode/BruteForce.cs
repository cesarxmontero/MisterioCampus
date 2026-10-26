using System.Collections.Generic;
using Misterio.Core;

namespace Misterio.Tests
{
    // Cuenta las soluciones probando todas las combinaciones posibles, sin propagar nada
    // Es simple pero sirve para comprobar el solucionador en casos pequeños
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
                    if (c.IsSolution(new Solution(placeOf, activityOf)))
                        count++;
                }
            }
            return count;
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