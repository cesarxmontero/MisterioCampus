namespace Misterio.Core
{
    // Descarte directo. Una pista sobre un sospechoso tacha opciones sin pensar nada más
    // Si niega tacha una opción ("Josep no estaba en la biblioteca") y si afirma tacha
    // todas las demás ("Luna estaba en la residencia")
    public class DirectClue : Technique
    {
        public DirectClue() : base("Descarte directo", 1)
        {
        }

        public override Step TryApply(CaseDefinition c, CaseGrid grid)
        {
            foreach (Clue clue in c.Clues)
            {
                // solo sirven las pistas sobre un sospechoso, las de relación van en CombineClues
                if (clue is SuspectClue && clue.Restrict(grid))
                    return MakeStep(clue.GetText(c));
            }
            return null;
        }
    }
}