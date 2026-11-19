using System.Collections.Generic;

namespace Misterio.Core
{
    // Generador de casos
    public static class CaseGenerator
    {
        static readonly string[] SuspectNames = { "Josep", "Ari", "Luna", "Lucía" };
        static readonly string[] PlaceNames = { "la biblioteca", "la cafetería", "el laboratorio", "la residencia" };
        static readonly string[] ActivityNames = { "estudiando", "cenando", "en una videollamada", "leyendo", "escuchando música", "dibujando" };

        // Genera un caso del nivel con esa semilla
        // De moment només afegeix pistes. Treuren les sobrants va més endevant
        public static CaseDefinition Generate(Difficulty difficulty, int seed)
        {
            LevelSettings level = LevelSettings.For(difficulty);
            System.Random random = new System.Random(seed);

            CaseDefinition c = EmptyCase(level, random);
            Solution solution = HiddenSolution(c, random);
            List<Clue> candidates = CandidateClues(c, solution, random);
            AddClues(c, candidates, level);
            return c;
        }

        // Caso sin pistas con los sospechosos y las actividades del nivel
        public static CaseDefinition EmptyCase(LevelSettings level, System.Random random)
        {
            CaseDefinition c = new CaseDefinition();
            c.Suspects.AddRange(Pick(SuspectNames, level.Suspects, random));
            c.Places.AddRange(PlaceNames);
            c.Activities.AddRange(Pick(ActivityNames, level.Suspects + level.ExtraActivities, random));
            c.CrimePlace = 0; // la biblioteca
            return c;
        }

        // Reparte un lugar y una actividad a cada sospechoso. Primero se elige al culpable y se
        // le pone en el lugar del robo, y los demás se reparten al azar el resto de lugares
        public static Solution HiddenSolution(CaseDefinition c, System.Random random)
        {
            int n = c.Suspects.Count;
            int[] placeOf = new int[n];
            int[] activityOf = new int[n];

            int culprit = random.Next(n);
            placeOf[culprit] = c.CrimePlace;

            List<int> places = new List<int>();
            for (int p = 0; p < c.Places.Count; p++)
            {
                if (p != c.CrimePlace)
                    places.Add(p);
            }
            Shuffle(places, random);
            int next = 0;
            for (int s = 0; s < n; s++)
            {
                if (s == culprit)
                    continue;
                placeOf[s] = places[next];
                next++;
            }

            // las actividades se barajan y cada sospechoso se queda una, las que sobran no las hace nadie
            List<int> activities = new List<int>();
            for (int a = 0; a < c.Activities.Count; a++)
                activities.Add(a);
            Shuffle(activities, random);
            for (int s = 0; s < n; s++)
                activityOf[s] = activities[s];

            return new Solution(placeOf, activityOf);
        }

        // Todas las pistas para la solución barajadas. Son las afirmaciones y negaciones
        // sobre cada sospechoso y las relaciones entre lugares y actividades, positivas y negativas
        // Falta la que diría quien estaba en la biblioteca porque resoldria el caso de golpe
        public static List<Clue> CandidateClues(CaseDefinition c, Solution solution, System.Random random)
        {
            List<Clue> clues = new List<Clue>();
            int culprit = solution.WhoWasIn(c.CrimePlace);

            for (int s = 0; s < c.Suspects.Count; s++)
            {
                for (int p = 0; p < c.Places.Count; p++)
                {
                    bool positive = solution.PlaceOf[s] == p;
                    if (positive && s == culprit)
                        continue;
                    clues.Add(new SuspectClue(s, Category.Place, p, positive));
                }
                for (int a = 0; a < c.Activities.Count; a++)
                    clues.Add(new SuspectClue(s, Category.Activity, a, solution.ActivityOf[s] == a));
            }

            for (int p = 0; p < c.Places.Count; p++)
            {
                int who = solution.WhoWasIn(p); // -1 si la sala está vacía
                for (int a = 0; a < c.Activities.Count; a++)
                    clues.Add(new RelationClue(p, a, who != -1 && solution.ActivityOf[who] == a));
            }

            Shuffle(clues, random);
            return clues;
        }

        // Afegeix pistes fins que el cas pasa sense ficar massa pistes directes
        public static bool AddClues(CaseDefinition c, List<Clue> candidates, LevelSettings level)
        {
            int direct = 0;
            foreach (Clue clue in candidates)
            {
                if (IsDirect(clue))
                {
                    if ((direct + 1) * 100 > level.MaxDirectPercent * (c.Clues.Count + 1))
                        continue;
                    direct++;
                }
                c.Clues.Add(clue);

                // si encara hi ha més d'una solució ja ni es prova
                if (CompleteSolver.CountSolutions(c, 2) == 1 && HumanSolver.Solve(c).Solved)
                    return true;
            }
            return false;
        }

        // Una afirmación directa dice el lugar o la actividad de un sospechoso sin tener que pensar
        // Las negaciones y las relaciones son indirectas
        public static bool IsDirect(Clue clue)
        {
            SuspectClue suspectClue = clue as SuspectClue;
            return suspectClue != null && suspectClue.Positive;
        }

        // Quita nombres al azar hasta que quedan los que hacen falta
        static List<string> Pick(string[] names, int count, System.Random random)
        {
            List<string> list = new List<string>(names);
            while (list.Count > count)
                list.RemoveAt(random.Next(list.Count));
            return list;
        }

        // Fisher-Yates
        static void Shuffle<T>(List<T> list, System.Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}