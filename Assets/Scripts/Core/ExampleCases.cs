namespace Misterio.Core
{
    // Casos hechos a mano para las pruebas y para la primera versión del juego
    public static class ExampleCases
    {
        // El caso de ejemplo de la memoria. Tiene una sola solución, en la que
        // Josep estaba en el laboratorio en una videollamada, Ari en la biblioteca estudiando
        // Luna en la residencia leyendo y Nora en la cafetería cenando. Ari robó el libro
        public static CaseDefinition Basic()
        {
            CaseDefinition c = new CaseDefinition();
            c.Suspects.AddRange(new[] { "Josep", "Ari", "Luna", "Nora" });
            c.Places.AddRange(new[] { "la biblioteca", "la cafetería", "el laboratorio", "la residencia" });
            c.Activities.AddRange(new[] { "estudiando", "cenando", "en una videollamada", "leyendo" });
            c.CrimePlace = 0;

            c.Clues.Add(new SuspectClue(0, Category.Place, 0, false));   // Josep no estaba en la biblioteca
            c.Clues.Add(new SuspectClue(0, Category.Place, 3, false));   // Josep no estaba en la residencia
            c.Clues.Add(new RelationClue(1, 1, true));                   // Quien estaba en la cafetería estaba cenando
            c.Clues.Add(new SuspectClue(0, Category.Activity, 2, true)); // Josep estaba en una videollamada
            c.Clues.Add(new RelationClue(3, 3, true));                   // Quien estaba en la residencia estaba leyendo
            c.Clues.Add(new SuspectClue(2, Category.Place, 3, true));    // Luna estaba en la residencia
            c.Clues.Add(new SuspectClue(3, Category.Place, 0, false));   // Nora no estaba en la biblioteca
            return c;
        }

        // Caso sin pistas con nombres genéricos. Sirve para las pruebas y como punto de partida
        public static CaseDefinition Empty(int suspects, int places, int activities)
        {
            CaseDefinition c = new CaseDefinition();
            for (int i = 0; i < suspects; i++)
                c.Suspects.Add("Sospechoso " + i);
            for (int i = 0; i < places; i++)
                c.Places.Add("lugar " + i);
            for (int i = 0; i < activities; i++)
                c.Activities.Add("actividad " + i);
            return c;
        }
    }
}