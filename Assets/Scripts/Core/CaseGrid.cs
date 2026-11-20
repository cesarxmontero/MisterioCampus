namespace Misterio.Core
{
    // places[s, p] es true si el sospechoso todavía puede haber estado en el lugar
    // y activities[s, a] lo mismo con las actividades
    public class CaseGrid
    {
        bool[,] places;
        bool[,] activities;
        // Longitud de la cadena amb la que es tacha cada opcio
        int[,] placeChains;
        int[,] activityChains;
        public CaseGrid(int suspectCount, int placeCount, int activityCount)
        {
            places = new bool[suspectCount, placeCount];
            activities = new bool[suspectCount, activityCount];
            placeChains = new int[suspectCount, placeCount];
            activityChains = new int[suspectCount, activityCount];

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
        // chain es la longitud de la cadena con la que se tacha. 1 si sale directamente de una pista
        public bool Remove(int suspect, Category category, int value, int chain = 1)
        {
            bool[,] table = Table(category);
            if (!table[suspect, value])
                return false;
            table[suspect, value] = false;
            Chains(category)[suspect, value] = chain;
            return true;
        }

        // Deja al sospechoso solo con ese valor
        public void KeepOnly(int suspect, Category category, int value, int chain = 1)
        {
            for (int v = 0; v < ValueCount(category); v++)
            {
                if (v != value)
                    Remove(suspect, category, v, chain);
            }
        }

        // Longitud de la cadena con la que se tachó la opción. 0 si no está tachada
        public int Chain(int suspect, Category category, int value)
        {
            return Chains(category)[suspect, value];
        }

        // La cadena más larga de lo que está tachado en la fila de un sospechoso
        public int RowChain(int suspect, Category category)
        {
            int max = 0;
            for (int v = 0; v < ValueCount(category); v++)
            {
                if (Chain(suspect, category, v) > max)
                    max = Chain(suspect, category, v);
            }
            return max;
        }

        // Lo mismo en la columna de un valor
        public int ColumnChain(Category category, int value)
        {
            int max = 0;
            for (int s = 0; s < SuspectCount; s++)
            {
                if (Chain(s, category, value) > max)
                    max = Chain(s, category, value);
            }
            return max;
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
            copy.placeChains = (int[,])placeChains.Clone();
            copy.activityChains = (int[,])activityChains.Clone();
            return copy;
        }

        bool[,] Table(Category category)
        {
            if (category == Category.Place)
                return places;
            return activities;
        }

        int[,] Chains(Category category)
        {
            if (category == Category.Place)
                return placeChains;
            return activityChains;
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