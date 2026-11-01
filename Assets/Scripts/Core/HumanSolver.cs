namespace Misterio.Core
{
    // Solucionador "humano". Resuelve el caso como lo haría una persona con una deducción
    // cada vez y siempre la más fácil que encuentre. Nunca supone nada
    // Si llega a la solución el caso es deducible y la suma de los costes es su dificultad
    // Si se atasca, el caso no se puede resolver solo razonando con estas técnicas
    public static class HumanSolver
    {
        // De la más barata a la más cara
        static readonly Technique[] Techniques =
        {
            new DirectClue(),
            new Exclusivity(),
            new UniqueValue(),
            new CombineClues()
        };

        public static HumanResult Solve(CaseDefinition c)
        {
            HumanResult result = new HumanResult();
            if (!c.IsValid())
                return result;

            CaseGrid grid = new CaseGrid(c.Suspects.Count, c.Places.Count, c.Activities.Count);
            while (!IsSolved(grid))
            {
                Step step = NextStep(c, grid);
                if (step == null)
                    return result; // atascado, ninguna técnica sirve

                result.Steps.Add(step);
                if (grid.HasEmptyRow())
                    return result; // las pistas se contradicen
            }

            // se comprueba la solución entera por si acaso
            Solution solution = grid.ToSolution();
            if (c.IsSolution(solution))
            {
                result.Solved = true;
                result.Solution = solution;
            }
            return result;
        }

        // Prueba las técnicas en orden y devuelve el primer paso que encuentre
        static Step NextStep(CaseDefinition c, CaseGrid grid)
        {
            foreach (Technique technique in Techniques)
            {
                Step step = technique.TryApply(c, grid);
                if (step != null)
                    return step;
            }
            return null;
        }

        // true si a cada sospechoso le queda una sola opción de cada tipo
        static bool IsSolved(CaseGrid grid)
        {
            for (int s = 0; s < grid.SuspectCount; s++)
            {
                if (grid.OnlyOption(s, Category.Place) == -1 || grid.OnlyOption(s, Category.Activity) == -1)
                    return false;
            }
            return true;
        }
    }
}