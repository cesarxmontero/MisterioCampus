namespace Misterio.Core
{
    // Pista sobre un sospechoso concreto. Puede afirmar o negar
    // Por ejemplo "Luna estaba en la residencia" o "Josep no estaba en la biblioteca"
    public class SuspectClue : Clue
    {
        public int Suspect;
        public Category Category;
        public int Value;
        public bool Positive;
        public SuspectClue(int suspect, Category category, int value, bool positive)
        {
            Suspect = suspect;
            Category = category;
            Value = value;
            Positive = positive;
        }
        public override bool IsTrue(Solution solution)
        {
            bool matches = solution.Get(Suspect, Category) == Value;
            return matches == Positive;
        }
        public override string GetText(CaseDefinition c)
        {
            string text = c.Suspects[Suspect] + (Positive ? " estaba " : " no estaba ");
            if (Category == Category.Place)
                text += "en " + c.Places[Value];
            else
                text += c.Activities[Value];
            return text + ".";
        }
    }
}