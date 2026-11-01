namespace Misterio.Core
{
    // Clase base de las técnicas de deducción del solucionador humano
    // Cada técnica tiene un nombre y un coste
    public abstract class Technique
    {
        public string Name;
        public int Cost;

        protected static readonly Category[] Categories = { Category.Place, Category.Activity };

        protected Technique(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }

        // Busca una deducción que se pueda hacer ahora y si la encuentra la aplica a la
        // cuadrícula y devuelve el paso. Si no encuentra ninguna devuelve null
        public abstract Step TryApply(CaseDefinition c, CaseGrid grid);

        protected Step MakeStep(string text)
        {
            return new Step(Name, Cost, text);
        }

        // A los lugares les pone "en" delante y las actividades las deja igual
        protected static string ValueText(CaseDefinition c, Category category, int value)
        {
            if (category == Category.Place)
                return "en " + c.Places[value];
            return c.Activities[value];
        }
    }
}