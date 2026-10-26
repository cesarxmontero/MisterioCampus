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
    }
}