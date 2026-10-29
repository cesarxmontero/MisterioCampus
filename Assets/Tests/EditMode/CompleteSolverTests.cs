using System.Collections.Generic;
using NUnit.Framework;
using Misterio.Core;

namespace Misterio.Tests
{
    public class CompleteSolverTests
    {
        [Test]
        public void BasicCase_HasOneSolution()
        {
            List<Solution> solutions = CompleteSolver.FindSolutions(ExampleCases.Basic(), 10);

            Assert.AreEqual(1, solutions.Count);
            // Josep laboratorio, Ari biblioteca, Luna residencia, Nora cafetería
            CollectionAssert.AreEqual(new[] { 2, 0, 3, 1 }, solutions[0].PlaceOf);
            // Josep videollamada, Ari estudiando, Luna leyendo, Nora cenando
            CollectionAssert.AreEqual(new[] { 2, 0, 3, 1 }, solutions[0].ActivityOf);
        }

        [Test]
        public void BasicCase_CulpritIsAri()
        {
            CaseDefinition c = ExampleCases.Basic();
            Solution solution = CompleteSolver.FindSolutions(c, 2)[0];

            Assert.AreEqual(1, solution.WhoWasIn(c.CrimePlace));
        }

        [Test]
        public void BasicCase_SolutionMeetsAllClues()
        {
            CaseDefinition c = ExampleCases.Basic();
            Solution solution = CompleteSolver.FindSolutions(c, 2)[0];

            foreach (Clue clue in c.Clues)
                Assert.IsTrue(clue.IsTrue(solution), clue.GetText(c));
        }

        [Test]
        public void BasicCase_WithoutLastClue_HasTwoSolutions()
        {
            // Sin la pista de que Nora no estaba en la biblioteca, el libro lo pudo robar Ari o Nora
            CaseDefinition c = ExampleCases.Basic();
            c.Clues.RemoveAt(c.Clues.Count - 1);

            Assert.AreEqual(2, CompleteSolver.CountSolutions(c));
        }

        // Sin pistas tienen que salir todas las combinaciones posibles
        [TestCase(4, 4, 4, 576)]  // 4! * 4!
        [TestCase(4, 4, 5, 2880)] // 4! * (5 * 4 * 3 * 2)
        [TestCase(3, 4, 4, 432)]  // 18 formas de repartir los lugares con la biblioteca ocupada * 24
        public void NoClues_CountsAllCombinations(int suspects, int places, int activities, int expected)
        {
            CaseDefinition c = ExampleCases.Empty(suspects, places, activities);

            Assert.AreEqual(expected, CompleteSolver.CountSolutions(c));
        }

        [Test]
        public void ContradictoryClues_NoSolution()
        {
            CaseDefinition c = ExampleCases.Empty(4, 4, 4);
            c.Clues.Add(new SuspectClue(0, Category.Place, 1, true));
            c.Clues.Add(new SuspectClue(0, Category.Place, 1, false));

            Assert.AreEqual(0, CompleteSolver.CountSolutions(c));
        }

        [Test]
        public void Limit_StopsTheSearch()
        {
            CaseDefinition c = ExampleCases.Empty(4, 4, 4);

            Assert.AreEqual(2, CompleteSolver.CountSolutions(c, 2));
        }

        [Test]
        public void MorePlacesThanSuspects_RelationNeedsThePlaceOccupied()
        {
            // 3 sospechosos y 4 lugares. Si quien estaba en el lugar 3 hacía la actividad 0,
            // el lugar 3 no puede quedar vacío
            CaseDefinition c = ExampleCases.Empty(3, 4, 4);
            c.Clues.Add(new RelationClue(3, 0, true));

            Assert.AreEqual(BruteForce.Count(c), CompleteSolver.CountSolutions(c));
            foreach (Solution solution in CompleteSolver.FindSolutions(c))
                Assert.AreNotEqual(-1, solution.WhoWasIn(3));
        }

        [Test]
        public void RandomCases_SameResultAsBruteForce()
        {
            // Casos pequeños al azar comparando con probar todas las combinaciones una a una
            // La seed es fija para que la prueba salga siempre igual
            System.Random random = new System.Random(2000);
            for (int i = 0; i < 100; i++)
            {
                CaseDefinition c = RandomCase(random);
                Assert.AreEqual(BruteForce.Count(c), CompleteSolver.CountSolutions(c), "caso " + i);
            }
        }

        static CaseDefinition RandomCase(System.Random random)
        {
            int suspects = random.Next(3, 5);           // 3 o 4
            int places = suspects + random.Next(0, 2);  // puede sobrar un lugar
            int activities = suspects + random.Next(0, 2);  // y una actividad
            CaseDefinition c = ExampleCases.Empty(suspects, places, activities);

            int clueCount = random.Next(0, 8);
            for (int i = 0; i < clueCount; i++)
            {
                // más negaciones que afirmaciones, si no curiosamente casi todos los casos salen sin solución
                bool positive = random.Next(3) == 0;
                if (random.Next(2) == 0)
                {
                    Category category = random.Next(2) == 0 ? Category.Place : Category.Activity;
                    int max = category == Category.Place ? places : activities;
                    c.Clues.Add(new SuspectClue(random.Next(suspects), category, random.Next(max), positive));
                }
                else
                {
                    c.Clues.Add(new RelationClue(random.Next(places), random.Next(activities), positive));
                }
            }
            return c;
        }
    }
}