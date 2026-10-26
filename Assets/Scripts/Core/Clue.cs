namespace Misterio.Core
{
    // Clase base de todas las pistas
    public abstract class Clue
    {
        // true si la pista se cumple en esa solución
        public abstract bool IsTrue(Solution solution);
        // Frase que lee el jugador
        public abstract string GetText(CaseDefinition c);
    }
}