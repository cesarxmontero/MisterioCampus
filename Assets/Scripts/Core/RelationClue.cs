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

        public override bool Restrict(CaseGrid grid)
        {
            bool changed = false;
            for (int s = 0; s < grid.SuspectCount; s++)
            {
                if (Positive)
                {
                    // Cada lugar y cada actividad son de una sola persona, así que quien estaba
                    // en el lugar es la misma persona que hacía la actividad. Si un sospechoso no pudo estar
                    // ahí tampoco pudo hacer la actividad y al revés
                    bool couldBeThere = grid.IsPossible(s, Category.Place, Place);
                    bool couldDoIt = grid.IsPossible(s, Category.Activity, Activity);
                    if (!couldBeThere && grid.Remove(s, Category.Activity, Activity))
                        changed = true;
                    if (!couldDoIt && grid.Remove(s, Category.Place, Place))
                        changed = true;
                }
                else
                {
                    // Si ya se sabe que un sospechoso estaba en el lugar, no hacía la actividad, y al revés
                    if (grid.OnlyOption(s, Category.Place) == Place && grid.Remove(s, Category.Activity, Activity))
                        changed = true;
                    if (grid.OnlyOption(s, Category.Activity) == Activity && grid.Remove(s, Category.Place, Place))
                        changed = true;
                }
            }
            return changed;
        }

        public override string GetText(CaseDefinition c)
        {
            if (Positive)
                return "Quien estaba en " + c.Places[Place] + " estaba " + c.Activities[Activity] + ".";
            return "Nadie estaba " + c.Activities[Activity] + " en " + c.Places[Place] + ".";
        }
    }
}