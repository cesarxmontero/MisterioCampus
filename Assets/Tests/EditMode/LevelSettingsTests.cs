using NUnit.Framework;
using Misterio.Core;

namespace Misterio.Tests
{
    public class LevelSettingsTests
    {
        // Valors de la taula de la memoria
        [TestCase(Difficulty.Easy, 3, 0, 40)]
        [TestCase(Difficulty.Normal, 4, 1, 20)]
        [TestCase(Difficulty.Hard, 4, 2, 0)]
        public void Levels_MatchTheTable(Difficulty difficulty, int suspects, int extraActivities, int maxDirectPercent)
        {
            LevelSettings level = LevelSettings.For(difficulty);

            Assert.AreEqual(suspects, level.Suspects);
            Assert.AreEqual(extraActivities, level.ExtraActivities);
            Assert.AreEqual(maxDirectPercent, level.MaxDirectPercent);
        }

        [Test]
        public void Levels_NeverNeedMoreThanTheCampusHas()
        {
            // 4 sospechosos, 4 salas y 6 actividades
            foreach (Difficulty difficulty in new[] { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard })
            {
                LevelSettings level = LevelSettings.For(difficulty);
                Assert.IsTrue(level.Suspects <= 4, difficulty.ToString());
                Assert.IsTrue(level.Suspects + level.ExtraActivities <= 6, difficulty.ToString());
            }
        }
        
        [Test]
        public void Levels_RangesGoUpAndDoNotOverlap()
        {
            LevelSettings easy = LevelSettings.For(Difficulty.Easy);
            LevelSettings normal = LevelSettings.For(Difficulty.Normal);
            LevelSettings hard = LevelSettings.For(Difficulty.Hard);

            Assert.IsTrue(easy.MinCost <= easy.MaxCost);
            Assert.IsTrue(normal.MinCost <= normal.MaxCost);
            Assert.IsTrue(hard.MinCost <= hard.MaxCost);
            Assert.IsTrue(easy.MaxCost < normal.MinCost);
            Assert.IsTrue(normal.MaxCost < hard.MinCost);
        }
    }
}