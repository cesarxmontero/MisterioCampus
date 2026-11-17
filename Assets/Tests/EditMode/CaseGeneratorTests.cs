using System.Collections.Generic;
using NUnit.Framework;
using Misterio.Core;

namespace Misterio.Tests
{
    public class CaseGeneratorTests
    {
        [TestCase(Difficulty.Easy)]
        [TestCase(Difficulty.Normal)]
        [TestCase(Difficulty.Hard)]
        public void EmptyCase_HasTheSizesOfTheLevel(Difficulty difficulty)
        {
            LevelSettings level = LevelSettings.For(difficulty);
            CaseDefinition c = CaseGenerator.EmptyCase(level, new System.Random(1));

            Assert.AreEqual(level.Suspects, c.Suspects.Count);
            Assert.AreEqual(4, c.Places.Count);
            Assert.AreEqual(level.Suspects + level.ExtraActivities, c.Activities.Count);
            Assert.AreEqual(0, c.Clues.Count);
            Assert.IsTrue(c.IsValid());
        }

        [Test]
        public void HiddenSolution_FollowsTheRules()
        {
            // Sin pistas IsSolution solo mira las reglas, que nadie repita lugar ni actividad
            // y que haya alguien en la biblioteca
            for (int seed = 0; seed < 300; seed++)
            {
                System.Random random = new System.Random(seed);
                LevelSettings level = LevelSettings.For((Difficulty)(seed % 3));
                CaseDefinition c = CaseGenerator.EmptyCase(level, random);
                Solution solution = CaseGenerator.HiddenSolution(c, random);

                Assert.IsTrue(c.IsSolution(solution), "semilla " + seed);
            }
        }

        [Test]
        public void SameSeed_SameCase()
        {
            LevelSettings level = LevelSettings.For(Difficulty.Easy);
            System.Random first = new System.Random(42);
            System.Random second = new System.Random(42);

            CaseDefinition a = CaseGenerator.EmptyCase(level, first);
            CaseDefinition b = CaseGenerator.EmptyCase(level, second);
            Solution solutionA = CaseGenerator.HiddenSolution(a, first);
            Solution solutionB = CaseGenerator.HiddenSolution(b, second);

            CollectionAssert.AreEqual(a.Suspects, b.Suspects);
            CollectionAssert.AreEqual(a.Activities, b.Activities);
            CollectionAssert.AreEqual(solutionA.PlaceOf, solutionB.PlaceOf);
            CollectionAssert.AreEqual(solutionA.ActivityOf, solutionB.ActivityOf);
        }

        [Test]
        public void ManySeeds_EverySuspectIsSometimesTheCulprit()
        {
            // Si el reparto fuera siempre igual saldría siempre el mismo culpable
            LevelSettings level = LevelSettings.For(Difficulty.Normal);
            List<string> culprits = new List<string>();
            for (int seed = 0; seed < 50; seed++)
            {
                System.Random random = new System.Random(seed);
                CaseDefinition c = CaseGenerator.EmptyCase(level, random);
                Solution solution = CaseGenerator.HiddenSolution(c, random);

                string culprit = c.Suspects[solution.WhoWasIn(c.CrimePlace)];
                if (!culprits.Contains(culprit))
                    culprits.Add(culprit);
            }
            Assert.AreEqual(4, culprits.Count);
        }
                [Test]
        public void CandidateClues_AreAllTrue()
        {
            for (int seed = 0; seed < 100; seed++)
            {
                System.Random random = new System.Random(seed);
                CaseDefinition c = CaseGenerator.EmptyCase(LevelSettings.For((Difficulty)(seed % 3)), random);
                Solution solution = CaseGenerator.HiddenSolution(c, random);

                foreach (Clue clue in CaseGenerator.CandidateClues(c, solution, random))
                    Assert.IsTrue(clue.IsTrue(solution), clue.GetText(c) + " (semilla " + seed + ")");
            }
        }

        [Test]
        public void CandidateClues_CountAndNoRepeats()
        {
            // Nivel normal, 4 sospechosos, 4 lugares y 5 actividades
            // 4 * (4 + 5) pistas de sospechoso y 4 * 5 de relación, menos la del culpable, son 55
            System.Random random = new System.Random(7);
            CaseDefinition c = CaseGenerator.EmptyCase(LevelSettings.For(Difficulty.Normal), random);
            Solution solution = CaseGenerator.HiddenSolution(c, random);
            List<Clue> clues = CaseGenerator.CandidateClues(c, solution, random);

            Assert.AreEqual(55, clues.Count);
            List<string> texts = new List<string>();
            foreach (Clue clue in clues)
            {
                string text = clue.GetText(c);
                Assert.IsFalse(texts.Contains(text), "repetida: " + text);
                texts.Add(text);
            }
        }

        [Test]
        public void CandidateClues_NeverSayWhoWasInTheLibrary()
        {
            for (int seed = 0; seed < 100; seed++)
            {
                System.Random random = new System.Random(seed);
                CaseDefinition c = CaseGenerator.EmptyCase(LevelSettings.For((Difficulty)(seed % 3)), random);
                Solution solution = CaseGenerator.HiddenSolution(c, random);

                foreach (Clue clue in CaseGenerator.CandidateClues(c, solution, random))
                {
                    SuspectClue suspectClue = clue as SuspectClue;
                    if (suspectClue == null)
                        continue;
                    bool saysCulprit = suspectClue.Positive && suspectClue.Category == Category.Place && suspectClue.Value == c.CrimePlace;
                    Assert.IsFalse(saysCulprit, clue.GetText(c));
                }
            }
        }

        [Test]
        public void AllCandidates_OnlyAllowTheHiddenSolution()
        {
            // Con todas las pistas juntas solo puede quedar la solución que se repartió
            for (int seed = 0; seed < 30; seed++)
            {
                System.Random random = new System.Random(seed);
                CaseDefinition c = CaseGenerator.EmptyCase(LevelSettings.For((Difficulty)(seed % 3)), random);
                Solution solution = CaseGenerator.HiddenSolution(c, random);
                c.Clues.AddRange(CaseGenerator.CandidateClues(c, solution, random));

                List<Solution> solutions = CompleteSolver.FindSolutions(c);
                Assert.AreEqual(1, solutions.Count, "semilla " + seed);
                CollectionAssert.AreEqual(solution.PlaceOf, solutions[0].PlaceOf);
                CollectionAssert.AreEqual(solution.ActivityOf, solutions[0].ActivityOf);
            }
        }
    }
}