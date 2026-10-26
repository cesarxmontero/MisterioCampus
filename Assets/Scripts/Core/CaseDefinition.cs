using System.Collections.Generic;
namespace Misterio.Core
{
    // Un caso del juego con sus sospechosos, lugares, actividades y pistas
    // En el código cada sospechoso, lugar o actividad es su posición en la lista
    public class CaseDefinition
    {
        public List<string> Suspects = new List<string>();
        public List<string> Places = new List<string>();
        public List<string> Activities = new List<string>();
        public List<Clue> Clues = new List<Clue>();
        // Lugar del robo (la biblioteca) quien estaba ahí es el culpable
        public int CrimePlace = 0;
        // Cada lugar y cada actividad son de una sola persona, así que tiene que haber
        // al menos tantos lugares y actividades como sospechosos
        public bool IsValid()
        {
            if (Suspects.Count == 0)
                return false;
            if (Places.Count < Suspects.Count || Activities.Count < Suspects.Count)
                return false;
            return CrimePlace >= 0 && CrimePlace < Places.Count;
        }
        // Comprueba que una solución cumple las reglas del juego y todas las pistas
        public bool IsSolution(Solution solution)
        {
            if (solution.PlaceOf.Length != Suspects.Count || solution.ActivityOf.Length != Suspects.Count)
                return false;
            if (!AllDifferent(solution.PlaceOf, Places.Count))
                return false;
            if (!AllDifferent(solution.ActivityOf, Activities.Count))
                return false;
            // en el lugar del robo tiene que haber alguien
            if (solution.WhoWasIn(CrimePlace) == -1)
                return false;
            foreach (Clue clue in Clues)
            {
                if (!clue.IsTrue(solution))
                    return false;
            }
            return true;
        }
        // true si todos los valores están entre 0 y count - 1 y no se repite ninguno
        static bool AllDifferent(int[] values, int count)
        {
            bool[] used = new bool[count];
            foreach (int v in values)
            {
                if (v < 0 || v >= count || used[v])
                    return false;
                used[v] = true;
            }
            return true;
        }
    }
}