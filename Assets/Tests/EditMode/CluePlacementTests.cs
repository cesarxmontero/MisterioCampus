using System.Collections.Generic;
using Misterio.Core;
using NUnit.Framework;

namespace Misterio.Tests
{
    public class CluePlacementTests
    {
        // Un caso de 4 sospechosos con tantas pistas como se quiera. Para el reparto da igual qué digan
        static CaseDefinition CaseWithClues(int count)
        {
            CaseDefinition c = ExampleCases.Empty(4, 4, 4);
            for (int i = 0; i < count; i++)
                c.Clues.Add(new SuspectClue(i % 4, Category.Place, 1, false));
            return c;
        }

        // Cuántas veces sale cada pista en el reparto
        static int[] TimesPlaced(CaseDefinition c, CluePlacement placement)
        {
            int[] times = new int[c.Clues.Count];
            foreach (List<int> clues in placement.BySuspect)
            {
                foreach (int clue in clues)
                    times[clue]++;
            }
            foreach (int clue in placement.BySlot)
            {
                if (clue != -1)
                    times[clue]++;
            }
            return times;
        }

        [TestCase(Difficulty.Easy)]
        [TestCase(Difficulty.Normal)]
        [TestCase(Difficulty.Hard)]
        public void GeneratedCases_EveryClueOnceAndOneOrTwoPerSuspect(Difficulty difficulty)
        {
            for (int seed = 0; seed < 200; seed++)
            {
                CaseDefinition c = CaseGenerator.Generate(difficulty, seed).Case;
                CluePlacement placement = CluePlacement.Place(c, seed);

                Assert.IsNotNull(placement, "semilla " + seed);
                foreach (int times in TimesPlaced(c, placement))
                    Assert.AreEqual(1, times, "semilla " + seed);
                foreach (List<int> clues in placement.BySuspect)
                    Assert.IsTrue(clues.Count == 1 || clues.Count == 2, "semilla " + seed);
            }
        }

        [Test]
        public void SecondClue_OnlyWhenTheSlotsAreFull()
        {
            // 18 pistas, 4 para los sospechosos y 12 para los huecos. Sobran 2
            CluePlacement placement = CluePlacement.Place(CaseWithClues(18), 7);

            int seconds = 0;
            foreach (List<int> clues in placement.BySuspect)
            {
                if (clues.Count == 2)
                    seconds++;
            }
            Assert.AreEqual(2, seconds);
            foreach (int clue in placement.BySlot)
                Assert.AreNotEqual(-1, clue);
        }

        [Test]
        public void TooManyClues_DoNotFit()
        {
            Assert.IsNotNull(CluePlacement.Place(CaseWithClues(20), 7)); // lo máximo, 4 x 2 + 12
            Assert.IsNull(CluePlacement.Place(CaseWithClues(21), 7));
        }

        [Test]
        public void FewerCluesThanSuspects_DoNotFit()
        {
            Assert.IsNull(CluePlacement.Place(CaseWithClues(3), 7)); // uno de los 4 se quedaría sin pista
        }

        [Test]
        public void Evidence_SpreadAcrossTheRooms()
        {
            // 10 pistas, 4 para los sospechosos y 6 indicios. Cada sala tiene que tener 1 o 2
            CluePlacement placement = CluePlacement.Place(CaseWithClues(10), 7);

            for (int r = 0; r < 4; r++)
            {
                int inRoom = 0;
                for (int i = 0; i < CluePlacement.SlotsPerRoom; i++)
                {
                    if (placement.BySlot[r, i] != -1)
                        inRoom++;
                }
                Assert.IsTrue(inRoom == 1 || inRoom == 2, "sala " + r);
            }
        }

        [Test]
        public void SameSeed_SamePlacement()
        {
            CaseDefinition c = CaseGenerator.Generate(Difficulty.Hard, 2026).Case;
            CluePlacement first = CluePlacement.Place(c, 2026);
            CluePlacement second = CluePlacement.Place(c, 2026);

            CollectionAssert.AreEqual(first.BySlot, second.BySlot);
            for (int s = 0; s < c.Suspects.Count; s++)
                CollectionAssert.AreEqual(first.BySuspect[s], second.BySuspect[s]);
        }

        [Test]
        public void KindOf_PlaceActivityAndRelation()
        {
            Assert.AreEqual(EvidenceKind.Place, CluePlacement.KindOf(new SuspectClue(0, Category.Place, 1, false)));
            Assert.AreEqual(EvidenceKind.Activity, CluePlacement.KindOf(new SuspectClue(0, Category.Activity, 2, true)));
            Assert.AreEqual(EvidenceKind.Relation, CluePlacement.KindOf(new RelationClue(1, 1, true)));
        }
    }
}