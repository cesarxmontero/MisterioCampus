namespace Misterio.Core
{
    // Parámetros de cada nivel de dificultad
    public class LevelSettings
    {
        public int Suspects;         // cuántos sospechosos hay
        public int ExtraActivities;  // actividades de más, que no hace nadie
        public int MaxDirectPercent; // maximo de afirmaciones directas ("Luna estaba en la residencia") en % de las pistas
        public int MinCost;          // rango de dificultad, la suma de costes del solucionador humano
        public int MaxCost;          // los rangos salen del experimento sin rango (analisis.py)

        public LevelSettings(int suspects, int extraActivities, int maxDirectPercent, int minCost, int maxCost)
        {
            Suspects = suspects;
            ExtraActivities = extraActivities;
            MaxDirectPercent = maxDirectPercent;
            MinCost = minCost;
            MaxCost = maxCost;
        }

        public static LevelSettings For(Difficulty difficulty)
        {
            if (difficulty == Difficulty.Easy)
                return new LevelSettings(3, 0, 40, 0, 24);
            if (difficulty == Difficulty.Normal)
                return new LevelSettings(4, 1, 20, 25, 38);
            return new LevelSettings(4, 2, 0, 39, 99);
        }
    }
}