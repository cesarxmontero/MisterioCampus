namespace Misterio.Core
{
    // Lo que devuelve el generador. Además del caso guarda la solución para que el juego sepa
    // quién fue y la traza del humano, que da la dificultad y sirve par a la pantalla final
    public class GeneratedCase
    {
        public CaseDefinition Case;
        public Solution Solution;
        public int Seed;
        public int Attempts;      // cuántos casos se han generado hasta que uno ha encajado en el nivel
        public HumanResult Human; // con Human.TotalCost() se tiene la dificultad del caso

        public GeneratedCase(CaseDefinition c, Solution solution, int seed, int attempts, HumanResult human)
        {
            Case = c;
            Solution = solution;
            Seed = seed;
            Attempts = attempts;
            Human = human;
        }
    }
}