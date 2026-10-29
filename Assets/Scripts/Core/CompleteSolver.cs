using System.Collections.Generic;

namespace Misterio.Core
{
    // Solucionador completo. Busca las soluciones de un caso con búsqueda con vuelta atrás
    // Antes de cada decisión se propagan las reglas y las pistas que quitan de la
    // cuadrícula las opciones imposibles, y así hay mucho menos que probar
    // El generador lo usará para comprobar que un caso tiene una única solución
    public static class CompleteSolver
    {
        static readonly Category[] Categories = { Category.Place, Category.Activity };

        // Con limit = 2 ya sirve para saber si la solución es única
        public static int CountSolutions(CaseDefinition c, int limit = int.MaxValue)
        {
            return FindSolutions(c, limit).Count;
        }

        public static List<Solution> FindSolutions(CaseDefinition c, int limit = int.MaxValue)
        {
            List<Solution> found = new List<Solution>();
            if (!c.IsValid())
                return found;

            CaseGrid grid = new CaseGrid(c.Suspects.Count, c.Places.Count, c.Activities.Count);
            Search(c, grid, limit, found);
            return found;
        }

        static void Search(CaseDefinition c, CaseGrid grid, int limit, List<Solution> found)
        {
            if (found.Count >= limit)
                return;
            if (!Propagate(c, grid))
                return; // contradicción, aqui no hay solución

            // Se elige el sospechoso y la categoría con menos opciones 
            int bestSuspect = -1;
            Category bestCategory = Category.Place;
            int bestCount = int.MaxValue;
            for (int s = 0; s < grid.SuspectCount; s++)
            {
                foreach (Category category in Categories)
                {
                    int n = grid.CountOptions(s, category);
                    if (n > 1 && n < bestCount)
                    {
                        bestSuspect = s;
                        bestCategory = category;
                        bestCount = n;
                    }
                }
            }

            if (bestSuspect == -1)
            {
                // Ya no queda nada por decidir y se comprueba la solución entera por si acaso
                Solution solution = grid.ToSolution();
                if (c.IsSolution(solution))
                    found.Add(solution);
                return;
            }

            // Se prueba cada opción en una copia de la cuadrícula. Si no lleva a nada se vuelve atrás 
            // y se prueba la siguiente
            for (int v = 0; v < grid.ValueCount(bestCategory); v++)
            {
                if (!grid.IsPossible(bestSuspect, bestCategory, v))
                    continue;

                CaseGrid next = grid.Copy();
                next.KeepOnly(bestSuspect, bestCategory, v);
                Search(c, next, limit, found);
                if (found.Count >= limit)
                    return;
            }
        }

        // Aplica las reglas y las pistas una y otra vez hasta que ya no cambia nada
        // Devuelve false si se llega a una contradicción
        static bool Propagate(CaseDefinition c, CaseGrid grid)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;

                foreach (Clue clue in c.Clues)
                {
                    if (clue.Restrict(grid))
                        changed = true;
                }

                // lo que ya es de un sospechoso no puede ser de otro
                foreach (Category category in Categories)
                {
                    if (RemoveTaken(grid, category))
                        changed = true;
                }

                // En el lugar del robo tiene que haber alguien
                // Si solo un sospechoso puede estar ahi, tiene que ser él
                int candidates = 0;
                int last = -1;
                for (int s = 0; s < grid.SuspectCount; s++)
                {
                    if (grid.IsPossible(s, Category.Place, c.CrimePlace))
                    {
                        candidates++;
                        last = s;
                    }
                }
                if (candidates == 0)
                    return false;
                if (candidates == 1 && grid.CountOptions(last, Category.Place) > 1)
                {
                    grid.KeepOnly(last, Category.Place, c.CrimePlace);
                    changed = true;
                }

                if (grid.HasEmptyRow())
                    return false;
            }
            return true;
        }

        // Si a un sospechoso solo le queda un valor, se les quita a los demás
        static bool RemoveTaken(CaseGrid grid, Category category)
        {
            bool changed = false;
            for (int s = 0; s < grid.SuspectCount; s++)
            {
                int v = grid.OnlyOption(s, category);
                if (v == -1)
                    continue;

                for (int other = 0; other < grid.SuspectCount; other++)
                {
                    if (other != s && grid.Remove(other, category, v))
                        changed = true;
                }
            }
            return changed;
        }
    }
}