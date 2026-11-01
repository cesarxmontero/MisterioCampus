using System.Collections.Generic;

namespace Misterio.Core
{
    // Lo que devuelve el solucionador humano
    public class HumanResult
    {
        // true si ha llegado a la solución solo con deducciones
        public bool Solved;

        // La solución o null si no la ha encontrado
        public Solution Solution;

        // Todos los pasos en el orden en que se han hecho
        public List<Step> Steps = new List<Step>();

        // Dificultad del caso (la suma de todos los pasos)
        public int TotalCost()
        {
            int total = 0;
            foreach (Step step in Steps)
                total += step.Cost;
            return total;
        }

        // Coste del paso más difícil
        public int HardestStep()
        {
            int max = 0;
            foreach (Step step in Steps)
            {
                if (step.Cost > max)
                    max = step.Cost;
            }
            return max;
        }
    }
}