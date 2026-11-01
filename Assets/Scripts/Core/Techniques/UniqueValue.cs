namespace Misterio.Core
{
    // Valor unico. Si un lugar o una actividad solo lo puede tener un sospechoso, es suyo
    // Por ejemplo "solo Luna puede estar estudiando, así que Luna estaba estudiando"
    // Solo vale cuando ese valor lo tiene que tener alguien a la fuerza. Puede pasar cuando hay
    // tantos valores como sospechosos, con el lugar del robo, que nunca está vacío, y con los
    // valores que nombra una pista positiva de relación
    // Con actividades de más puede que nadie haga alguna y la deducción sería falsa
    public class UniqueValue : Technique
    {
        public UniqueValue() : base("Valor único", 2)
        {
        }

        public override Step TryApply(CaseDefinition c, CaseGrid grid)
        {
            foreach (Category category in Categories)
            {
                for (int v = 0; v < grid.ValueCount(category); v++)
                {
                    if (!SomeoneMustHave(c, grid, category, v))
                        continue;

                    int only = OnlySuspectFor(grid, category, v);
                    if (only != -1 && grid.CountOptions(only, category) > 1)
                    {
                        grid.KeepOnly(only, category, v);
                        string name = c.Suspects[only];
                        string value = ValueText(c, category, v);
                        return MakeStep("Solo " + name + " puede estar " + value + ", así que " + name + " estaba " + value + ".");
                    }
                }
            }
            return null;
        }

        // true si ese valor lo tiene que tener alguien sí o sí
        static bool SomeoneMustHave(CaseDefinition c, CaseGrid grid, Category category, int value)
        {
            if (grid.ValueCount(category) == grid.SuspectCount)
                return true;
            if (category == Category.Place && value == c.CrimePlace)
                return true;

            // "Quien estaba en la cafetería estaba cenando" significa que alguien estaba
            // en la cafetería y que alguien estaba cenando
            foreach (Clue clue in c.Clues)
            {
                RelationClue relation = clue as RelationClue;
                if (relation == null || !relation.Positive)
                    continue;
                if (category == Category.Place && relation.Place == value)
                    return true;
                if (category == Category.Activity && relation.Activity == value)
                    return true;
            }
            return false;
        }

        // El único sospechoso que todavía puede tener ese valor, o -1 si no hay ninguno o hay varios
        static int OnlySuspectFor(CaseGrid grid, Category category, int value)
        {
            int found = -1;
            for (int s = 0; s < grid.SuspectCount; s++)
            {
                if (!grid.IsPossible(s, category, value))
                    continue;
                if (found != -1)
                    return -1; // lo pueden tener varios
                found = s;
            }
            return found;
        }
    }
}