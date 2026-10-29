using NUnit.Framework;
using Misterio.Core;
namespace Misterio.Tests
{
    public class ClueTests
    {
        [Test]
        public void Texts()
        {
            CaseDefinition c = ExampleCases.Basic();
            Assert.AreEqual("Josep no estaba en la biblioteca.", c.Clues[0].GetText(c));
            Assert.AreEqual("Quien estaba en la cafetería estaba cenando.", c.Clues[2].GetText(c));
            Assert.AreEqual("Josep estaba en una videollamada.", c.Clues[3].GetText(c));
            Assert.AreEqual("Luna estaba en la residencia.", c.Clues[5].GetText(c));
            Assert.AreEqual("Nadie estaba leyendo en la biblioteca.", new RelationClue(0, 3, false).GetText(c));
        }
        
        [Test]
        public void SuspectClue_PositiveAndNegative()
        {
            // El sospechoso 0 en el lugar 0 con la actividad 0, y el 1 en el lugar 1 con la actividad 1
            Solution solution = new Solution(new[] { 0, 1 }, new[] { 0, 1 });
            Assert.IsTrue(new SuspectClue(0, Category.Place, 0, true).IsTrue(solution));
            Assert.IsFalse(new SuspectClue(0, Category.Place, 0, false).IsTrue(solution));
            Assert.IsTrue(new SuspectClue(1, Category.Activity, 0, false).IsTrue(solution));
        }
        [Test]
        public void RelationClue_NeedsSomeoneInThePlace()
        {
        // 3 sospechosos en los lugares 0, 1 y 2. En el lugar 3 no hay nadie
        Solution solution = new Solution(new[] { 0, 1, 2 }, new[] { 0, 1, 2 });
        Assert.IsFalse(new RelationClue(3, 3, true).IsTrue(solution));
        Assert.IsTrue(new RelationClue(3, 3, false).IsTrue(solution));
        Assert.IsTrue(new RelationClue(1, 1, true).IsTrue(solution));
        Assert.IsFalse(new RelationClue(1, 2, true).IsTrue(solution));
        }

        [Test]
        public void SuspectClue_Negative_RemovesOneOption()
        {
            // Josep no estaba en la biblioteca
            CaseGrid grid = new CaseGrid(4, 4, 4);
            new SuspectClue(0, Category.Place, 0, false).Restrict(grid);

            Assert.IsFalse(grid.IsPossible(0, Category.Place, 0));
            Assert.AreEqual(3, grid.CountOptions(0, Category.Place));
        }

        [Test]
        public void SuspectClue_Positive_LeavesOnlyThatOption()
        {
            // Luna estaba en la residencia
            CaseGrid grid = new CaseGrid(4, 4, 4);
            new SuspectClue(2, Category.Place, 3, true).Restrict(grid);

            Assert.AreEqual(3, grid.OnlyOption(2, Category.Place));
        }

        [Test]
        public void RelationClue_Positive_LinksPlaceAndActivity()
        {
            // Josep no cenó y quien estaba en la cafetería estaba cenando,
            // así que Josep no estaba en la cafetería
            CaseGrid grid = new CaseGrid(4, 4, 4);
            grid.Remove(0, Category.Activity, 1);
            new RelationClue(1, 1, true).Restrict(grid);

            Assert.IsFalse(grid.IsPossible(0, Category.Place, 1));
        }

        [Test]
        public void RelationClue_Negative_OnlyActsWhenThePlaceIsKnown()
        {
            // Nadie estaba leyendo en la biblioteca
            CaseGrid grid = new CaseGrid(4, 4, 4);
            RelationClue clue = new RelationClue(0, 3, false);

            // mientras no se sabe dónde estaba nadie, la pista no descarta nada
            Assert.IsFalse(clue.Restrict(grid));

            // si Ari estaba en la biblioteca, Ari no estaba leyendo
            grid.KeepOnly(1, Category.Place, 0);
            Assert.IsTrue(clue.Restrict(grid));
            Assert.IsFalse(grid.IsPossible(1, Category.Activity, 3));
        }
    }
}