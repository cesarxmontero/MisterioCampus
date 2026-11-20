namespace Misterio.Core
{
    // Combinar pistas. Junta una pista de relación con algo que ya se sabe de un sospechoso
    // Por ejemplo "Josep no estaba cenando y quien estaba en la cafetería estaba cenando,
    // así que Josep no estaba en la cafetería"
    // Cada paso saca una sola conclusión porque cada una es un razonamiento distinto
    public class CombineClues : Technique
    {
        public CombineClues() : base("Combinar pistas", 3)
        {
        }

        public override Step TryApply(CaseDefinition c, CaseGrid grid)
        {
            foreach (Clue clue in c.Clues)
            {
                // solo sirven las pistas de relación
                RelationClue relation = clue as RelationClue;
                if (relation == null)
                    continue;

                for (int s = 0; s < grid.SuspectCount; s++)
                {
                    Step step = relation.Positive ? FromPositive(c, grid, relation, s) : FromNegative(c, grid, relation, s);
                    if (step != null)
                        return step;
                }
            }
            return null;
        }

        // "Quien estaba en el lugar estaba haciendo la actividad". Para cada sospechoso,
        // estar en ese lugar y hacer esa actividad van juntos
        Step FromPositive(CaseDefinition c, CaseGrid grid, RelationClue r, int s)
        {
            string name = c.Suspects[s];
            string place = ValueText(c, Category.Place, r.Place);
            string activity = ValueText(c, Category.Activity, r.Activity);
            string clueText = "quien estaba " + place + " estaba " + activity;

            // la cadena sigue desde la opción que ya estaba tachada
            int chain = grid.Chain(s, Category.Place, r.Place) + 1;
            if (!grid.IsPossible(s, Category.Place, r.Place) && grid.Remove(s, Category.Activity, r.Activity, chain))
                return MakeStep(name + " no estaba " + place + " y " + clueText + ", así que " + name + " no estaba " + activity + ".", chain);
            chain = grid.Chain(s, Category.Activity, r.Activity) + 1;
            if (!grid.IsPossible(s, Category.Activity, r.Activity) && grid.Remove(s, Category.Place, r.Place, chain))
                return MakeStep(name + " no estaba " + activity + " y " + clueText + ", así que " + name + " no estaba " + place + ".", chain);
            return null;
        }

        // "Nadie estaba haciendo la actividad en el lugar". Solo sirve cuando ya se sabe
        // dónde estaba el sospechoso o qué hacía
        Step FromNegative(CaseDefinition c, CaseGrid grid, RelationClue r, int s)
        {
            string name = c.Suspects[s];
            string place = ValueText(c, Category.Place, r.Place);
            string activity = ValueText(c, Category.Activity, r.Activity);
            string clueText = "nadie estaba " + activity + " " + place;

            // aquí sigue desde lo tachado en la fila
            int chain = grid.RowChain(s, Category.Place) + 1;
            if (grid.OnlyOption(s, Category.Place) == r.Place && grid.Remove(s, Category.Activity, r.Activity, chain))
                return MakeStep(name + " estaba " + place + " y " + clueText + ", así que " + name + " no estaba " + activity + ".", chain);
            chain = grid.RowChain(s, Category.Activity) + 1;
            if (grid.OnlyOption(s, Category.Activity) == r.Activity && grid.Remove(s, Category.Place, r.Place, chain))
                return MakeStep(name + " estaba " + activity + " y " + clueText + ", así que " + name + " no estaba " + place + ".", chain);
            return null;
        }
    }
}