using NUnit.Framework;
using Misterio.Core;

namespace Misterio.Tests
{
    public class BruteForceTests
    {
        [Test]
        public void BasicCase_HasOneSolution()
        {
            Assert.AreEqual(1, BruteForce.Count(ExampleCases.Basic()));
        }

        [Test]
        public void BasicCase_WithoutLastClue_HasTwoSolutions()
        {
            // Sin la pista de que Nora no estaba en la biblioteca, el libro lo pudo robar Ari o Nora
            CaseDefinition c = ExampleCases.Basic();
            c.Clues.RemoveAt(c.Clues.Count - 1);

            Assert.AreEqual(2, BruteForce.Count(c));
        }

        // Sin pistas salen todas las combinaciones, como en el párrafo del azar de la memoria
        [TestCase(4, 4, 4, 576)]  // 4! * 4!
        [TestCase(4, 4, 5, 2880)] // 4! * (5 * 4 * 3 * 2) (Esto en realidad es actividades factorial entre actividades - sospechosos y se hace factorial)
        [TestCase(3, 4, 4, 432)]  // 18 formas de repartir los lugares con la biblioteca ocupada * 24
        public void NoClues_CountsAllCombinations(int suspects, int places, int activities, int expected)
        {
            CaseDefinition c = ExampleCases.Empty(suspects, places, activities);

            Assert.AreEqual(expected, BruteForce.Count(c));
        }
    }
}