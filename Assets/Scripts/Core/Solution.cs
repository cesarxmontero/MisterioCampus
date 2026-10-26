namespace Misterio.Core
{
// Solución de un caso, es decir, dónde estaba y qué hacía cada sospechoso
// PlaceOf[s] es el lugar del sospechoso y ActivityOf[s] su actividad
public class Solution
    {
        public int[] PlaceOf;
        public int[] ActivityOf;
        public Solution(int[] placeOf, int[] activityOf)
        {
            PlaceOf = placeOf;
            ActivityOf = activityOf;
        }
        public int Get(int suspect, Category category)
        {
            if (category == Category.Place)
                return PlaceOf[suspect];
            return ActivityOf[suspect];
        }
        // Devuelve quién estaba en ese lugar, o -1 si no había nadie
        public int WhoWasIn(int place)
        {
            for (int s = 0; s < PlaceOf.Length; s++)
            {
                if (PlaceOf[s] == place)
                    return s;
            }
            return -1;
        }
    }
}