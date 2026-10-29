using NUnit.Framework;
using Misterio.Core;

namespace Misterio.Tests
{
    public class CaseGridTests
    {
        [Test]
        public void NewGrid_EverythingIsPossible()
        {
            CaseGrid grid = new CaseGrid(3, 4, 5);

            Assert.AreEqual(4, grid.CountOptions(0, Category.Place));
            Assert.AreEqual(5, grid.CountOptions(0, Category.Activity));
            Assert.AreEqual(-1, grid.OnlyOption(0, Category.Place));
        }

        [Test]
        public void Remove_OnlyReturnsTrueTheFirstTime()
        {
            CaseGrid grid = new CaseGrid(3, 4, 4);

            Assert.IsTrue(grid.Remove(0, Category.Place, 2));
            Assert.IsFalse(grid.Remove(0, Category.Place, 2));
            Assert.AreEqual(3, grid.CountOptions(0, Category.Place));
        }

        [Test]
        public void KeepOnly_LeavesOneOption()
        {
            CaseGrid grid = new CaseGrid(3, 4, 4);
            grid.KeepOnly(1, Category.Activity, 2);

            Assert.AreEqual(2, grid.OnlyOption(1, Category.Activity));
        }

        [Test]
        public void RemovingEverything_IsAContradiction()
        {
            CaseGrid grid = new CaseGrid(2, 2, 2);
            Assert.IsFalse(grid.HasEmptyRow());

            grid.Remove(0, Category.Place, 0);
            grid.Remove(0, Category.Place, 1);
            Assert.IsTrue(grid.HasEmptyRow());
        }

        [Test]
        public void Copy_DoesNotChangeTheOriginal()
        {
            CaseGrid grid = new CaseGrid(3, 4, 4);
            CaseGrid copy = grid.Copy();
            copy.KeepOnly(0, Category.Place, 1);

            Assert.AreEqual(4, grid.CountOptions(0, Category.Place));
            Assert.AreEqual(1, copy.CountOptions(0, Category.Place));
        }

        [Test]
        public void ToSolution_ReadsTheOnlyOptions()
        {
            CaseGrid grid = new CaseGrid(2, 2, 2);
            grid.KeepOnly(0, Category.Place, 1);
            grid.KeepOnly(0, Category.Activity, 0);
            grid.KeepOnly(1, Category.Place, 0);
            grid.KeepOnly(1, Category.Activity, 1);

            Solution solution = grid.ToSolution();
            CollectionAssert.AreEqual(new[] { 1, 0 }, solution.PlaceOf);
            CollectionAssert.AreEqual(new[] { 0, 1 }, solution.ActivityOf);
        }
    }
}