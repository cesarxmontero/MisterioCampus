namespace Misterio.Core
{
    // Exclusividad. Si a un sospechoso solo le queda un valor ese valor es suyo y se les
    // tacha a todos los demas. Por ejemplo "Josep estaba en una videollamada, así que
    // nadie más estaba en una videollamada".
    public class Exclusivity : Technique
    {
        public Exclusivity() : base("Exclusividad", 1)
        {
        }

        public override Step TryApply(CaseDefinition c, CaseGrid grid)
        {
            for (int s = 0; s < grid.SuspectCount; s++)
            {
                foreach (Category category in Categories)
                {
                    int v = grid.OnlyOption(s, category);
                    if (v == -1)
                        continue;

                    bool changed = false;
                    for (int other = 0; other < grid.SuspectCount; other++)
                    {
                        if (other != s && grid.Remove(other, category, v))
                            changed = true;
                    }

                    if (changed)
                    {
                        string value = ValueText(c, category, v);
                        string rest = category == Category.Place ? "allí" : value;
                        return MakeStep(c.Suspects[s] + " estaba " + value + ", así que nadie más estaba " + rest + ".");
                    }
                }
            }
            return null;
        }
    }
}