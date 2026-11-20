namespace Misterio.Core
{
    // Un paso del razonamiento del solucionador humano que guarda la tecnica que se ha usado,
    // cuánto cuesta y qué se ha deducido (y lo explica con una frase)
    public class Step
    {
        public string TechniqueName;
        public int Cost;
        public string Text;
        public int Chain; // longitud de la cadena de deducciones que acaba en este paso


        public Step(string techniqueName, int cost, string text, int chain)
        {
            TechniqueName = techniqueName;
            Cost = cost;
            Text = text;
            Chain = chain;
        }

        public override string ToString()
        {
            return TechniqueName + " (" + Cost + "). " + Text;
        }
    }
}