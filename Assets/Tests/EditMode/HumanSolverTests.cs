using System.Collections.Generic;
using NUnit.Framework;
using Misterio.Core;

namespace Misterio.Tests
{
    public class HumanSolverTests
    {
        [Test]
        public void BasicCase_IsDeducible()
        {
            HumanResult result = HumanSolver.Solve(ExampleCases.Basic());

            Assert.IsTrue(result.Solved);
            CollectionAssert.AreEqual(new[] { 2, 0, 3, 1 }, result.Solution.PlaceOf);
            CollectionAssert.AreEqual(new[] { 2, 0, 3, 1 }, result.Solution.ActivityOf);
        }

        [Test]
        public void BasicCase_StepsAndDifficulty()
        {
            HumanResult result = HumanSolver.Solve(ExampleCases.Basic());

            Assert.AreEqual(15, result.Steps.Count);
            Assert.AreEqual(25, result.TotalCost());
            Assert.AreEqual(3, result.HardestStep());
            Assert.AreEqual("Josep no estaba en la biblioteca.", result.Steps[0].Text);
        }

        [Test]
        public void BasicCase_WriteStepsInConsole()
        {
            // Razonamiento en la consola de Unity
            HumanResult result = HumanSolver.Solve(ExampleCases.Basic());
            foreach (Step step in result.Steps)
                UnityEngine.Debug.Log(step.ToString());
            UnityEngine.Debug.Log("Dificultad total " + result.TotalCost());

            Assert.IsTrue(result.Solved);
        }

        [Test]
        public void CaseThatNeedsColumns_IsDeducible()
        {
            // El caso de Josep, Ari y Luna en el que el solucionador completo tenía que suponer
            // Una persona lo resuelve mirando columnas, por ejemplo viendo que solo Luna puede estar estudiando.
            CaseDefinition c = new CaseDefinition();
            c.Suspects.AddRange(new[] { "Josep", "Ari", "Luna" });
            c.Places.AddRange(new[] { "la biblioteca", "la cafetería", "el laboratorio" });
            c.Activities.AddRange(new[] { "estudiando", "cenando", "leyendo" });
            c.Clues.Add(new SuspectClue(0, Category.Activity, 0, false)); // Josep no estaba estudiando
            c.Clues.Add(new SuspectClue(1, Category.Activity, 0, false)); // Ari no estaba estudiando
            c.Clues.Add(new SuspectClue(2, Category.Place, 1, false));    // Luna no estaba en la cafetería
            c.Clues.Add(new SuspectClue(1, Category.Place, 1, false));    // Ari no estaba en la cafetería
            c.Clues.Add(new RelationClue(0, 1, true));                    // Quien estaba en la biblioteca estaba cenando

            HumanResult result = HumanSolver.Solve(c);

            Assert.IsTrue(result.Solved);
            Assert.IsTrue(UsesTechnique(result, "Valor único"));
        }

        [Test]
        public void NoClues_GetsStuck()
        {
            HumanResult result = HumanSolver.Solve(ExampleCases.Empty(3, 3, 3));

            Assert.IsFalse(result.Solved);
            Assert.AreEqual(0, result.Steps.Count);
        }

        [Test]
        public void TwoSolutions_GetsStuck()
        {
            // Sin la última pista hay dos soluciones por lo que no se puede razonar una solucion
            CaseDefinition c = ExampleCases.Basic();
            c.Clues.RemoveAt(c.Clues.Count - 1);

            Assert.IsFalse(HumanSolver.Solve(c).Solved);
        }

        [Test]
        public void RandomCases_WhenSolvedItIsTheRealSolution()
        {
            // Casos al azar con pistas siempre verdaderas. Si el humano llega a una solución tiene que ser
            // la de verdad y además la única, porque nunca tacha nada que sea posible
            System.Random random = new System.Random(2000);
            int solved = 0;
            for (int i = 0; i < 200; i++)
            {
                int suspects = random.Next(3, 5);               // 3 o 4
                int places = suspects + random.Next(0, 2);      // puede sobrar un lugar
                int activities = suspects + random.Next(0, 2);  // y una actividad
                Solution truth = new Solution(RandomValues(random, suspects, places, true), RandomValues(random, suspects, activities, false));
                CaseDefinition c = CaseWithTrueClues(random, truth, places, activities);

                HumanResult result = HumanSolver.Solve(c);
                if (!result.Solved)
                    continue;

                solved++;
                CollectionAssert.AreEqual(truth.PlaceOf, result.Solution.PlaceOf, "caso " + i);
                CollectionAssert.AreEqual(truth.ActivityOf, result.Solution.ActivityOf, "caso " + i);
                Assert.AreEqual(1, CompleteSolver.CountSolutions(c, 2), "caso " + i);
            }

            // si casi no resolviera ninguno, la prueba no estaría comprobando nada
            Assert.IsTrue(solved >= 20, "solo ha resuelto " + solved);
        }

        static bool UsesTechnique(HumanResult result, string name)
        {
            foreach (Step step in result.Steps)
            {
                if (step.TechniqueName == name)
                    return true;
            }
            return false;
        }

        // Valores distintos al azar para n sospechosos. Si needsZero es true, alguien tiene el 0,
        // que en los lugares es la biblioteca
        static int[] RandomValues(System.Random random, int n, int count, bool needsZero)
        {
            List<int> values = new List<int>();
            for (int v = 0; v < count; v++)
                values.Add(v);

            // se barajan y se cogen los n primeros
            for (int i = count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
            int[] result = values.GetRange(0, n).ToArray();

            if (needsZero && System.Array.IndexOf(result, 0) == -1)
                result[random.Next(n)] = 0;
            return result;
        }

        // Un caso con pistas al azar pero siempre verdaderas para la solución que se pasa
        static CaseDefinition CaseWithTrueClues(System.Random random, Solution truth, int places, int activities)
        {
            int suspects = truth.PlaceOf.Length;
            CaseDefinition c = ExampleCases.Empty(suspects, places, activities);

            int clueCount = random.Next(8, 18);
            for (int i = 0; i < clueCount; i++)
            {
                if (random.Next(2) == 0)
                {
                    int s = random.Next(suspects);
                    Category category = random.Next(2) == 0 ? Category.Place : Category.Activity;
                    int value = random.Next(category == Category.Place ? places : activities);
                    // afirma si ese es su valor y niega si no, nunca miente
                    c.Clues.Add(new SuspectClue(s, category, value, truth.Get(s, category) == value));
                }
                else
                {
                    int place = random.Next(places);
                    int activity = random.Next(activities);
                    int who = truth.WhoWasIn(place);
                    c.Clues.Add(new RelationClue(place, activity, who != -1 && truth.ActivityOf[who] == activity));
                }
            }
            return c;
        }
    }
}