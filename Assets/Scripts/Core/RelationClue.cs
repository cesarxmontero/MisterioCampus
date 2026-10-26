namespace Misterio.Core
{
    // Pista que relaciona un lugar con una actividad, sin decir quién era
    // Positiva, "Quien estaba en la cafetería estaba cenando" (alguien estaba ahí haciendo eso)
    // Negativa, "Nadie estaba leyendo en la biblioteca"
    public class RelationClue : Clue
    {
        public int Place;
        public int Activity;
        public bool Positive;
        public RelationClue(int place, int activity, bool positive)
        {
            Place = place;
            Activity = activity;
            Positive = positive;
        }
        public override bool IsTrue(Solution solution)
        {
            int s = solution.WhoWasIn(Place);
            bool happened = s != -1 && solution.ActivityOf[s] == Activity;
            return happened == Positive;
        }
        public override string GetText(CaseDefinition c)
        {
            if (Positive)
                return "Quien estaba en " + c.Places[Place] + " estaba " + c.Activities[Activity] + ".";
            return "Nadie estaba " + c.Activities[Activity] + " en " + c.Places[Place] + ".";
        }
    }
}
