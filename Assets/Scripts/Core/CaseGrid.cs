namespace Misterio.Core
{
    // places[s, p] es true si el sospechoso todavía puede haber estado en el lugar
    // y activities[s, a] lo mismo con las actividades
    public class CaseGrid
    {
        bool[,] places;
        bool[,] activities;

        public CaseGrid(int suspectCount, int placeCount, int activityCount)
        {
            places = new bool[suspectCount, placeCount];
            activities = new bool[suspectCount, activityCount];

            // al principio todo es posible
            Fill(places);
            Fill(activities);
        }

        // solo para Copy()
        CaseGrid()
        {
        }

        public int SuspectCount
        {
            get { return places.GetLength(0); }
        }

        // Cuántos lugares o actividades hay
        public int ValueCount(Category category)
        {
            return Table(category).GetLength(1);
        }

        public bool IsPossible(int suspect, Category category, int value)
        {
            return Table(category)[suspect, value];
        }

        // Quita una opción y devuelve true si todavía era posible
        public bool Remove(int suspect, Category category, int value)
        {
            bool[,] table = Table(category);
            if (!table[suspect, value])
                return false;
            table[suspect, value] = false;
            return true;
        }

        // Deja al sospechoso solo con ese valor
        public void KeepOnly(int suspect, Category category, int value)
        {
            for (int v = 0; v < ValueCount(category); v++)
            {
                if (v != value)
                    Remove(suspect, category, v);
            }
        }

        public int CountOptions(int suspect, Category category)
        {
            int count = 0;
            for (int v = 0; v < ValueCount(category); v++)
            {
                if (IsPossible(suspect, category, v))
                    count++;
            }
            return count;
        }

        // Si al sospechoso solo le queda una opción la devuelve. Si no, devuelve -1
        public int OnlyOption(int suspect, Category category)
        {
            int found = -1;
            for (int v = 0; v < ValueCount(category); v++)
            {
                if (!IsPossible(suspect, category, v))
                    continue;
                if (found != -1)
                    return -1; // le queda más de una
                found = v;
            }
            return found;
        }

        // true si algún sospechoso se ha quedado sin opciones
        public bool HasEmptyRow()
        {
            for (int s = 0; s < SuspectCount; s++)
            {
                if (CountOptions(s, Category.Place) == 0 || CountOptions(s, Category.Activity) == 0)
                    return true;
            }
            return false;
        }

        // Solo tiene sentido cuando a cada sospechoso le queda una opción de cada tipo
        public Solution ToSolution()
        {
            int[] placeOf = new int[SuspectCount];
            int[] activityOf = new int[SuspectCount];
            for (int s = 0; s < SuspectCount; s++)
            {
                placeOf[s] = OnlyOption(s, Category.Place);
                activityOf[s] = OnlyOption(s, Category.Activity);
            }
            return new Solution(placeOf, activityOf);
        }

        public CaseGrid Copy()
        {
            CaseGrid copy = new CaseGrid();
            copy.places = (bool[,])places.Clone();
            copy.activities = (bool[,])activities.Clone();
            return copy;
        }

        bool[,] Table(Category category)
        {
            if (category == Category.Place)
                return places;
            return activities;
        }

        static void Fill(bool[,] table)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                    table[i, j] = true;
            }
        }
    }
}