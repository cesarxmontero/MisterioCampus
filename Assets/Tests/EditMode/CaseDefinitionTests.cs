using NUnit.Framework;
using Misterio.Core;
namespace Misterio.Tests
{
    public class CaseDefinitionTests
        {
        [Test]
        public void IsValid_EnoughPlacesAndActivities_True()
        {
            Assert.IsTrue(ExampleCases.Empty(3, 4, 4).IsValid());
        }
        [Test]
        public void IsValid_FewerPlacesThanSuspects_False()
        {
            Assert.IsFalse(ExampleCases.Empty(4, 3, 4).IsValid());
        }
        [Test]
        public void IsSolution_EveryoneInDifferentPlace_True()
        {
            CaseDefinition c = ExampleCases.Empty(3, 4, 4);
            Solution solution = new Solution(new[] { 0, 1, 2 }, new[] { 2, 0, 1 });
            Assert.IsTrue(c.IsSolution(solution));
        }
        [Test]
        public void IsSolution_TwoSuspectsInSamePlace_False()
        {
            CaseDefinition c = ExampleCases.Empty(2, 2, 2);
            Solution solution = new Solution(new[] { 0, 0 }, new[] { 0, 1 });
            Assert.IsFalse(c.IsSolution(solution));
        }
        [Test]
        public void IsSolution_TwoSuspectsSameActivity_False()
        {
            CaseDefinition c = ExampleCases.Empty(2, 2, 2);
            Solution solution = new Solution(new[] { 0, 1 }, new[] { 1, 1 });
            Assert.IsFalse(c.IsSolution(solution));
        }
        [Test]
        public void IsSolution_NobodyInLibrary_False()
        {
            CaseDefinition c = ExampleCases.Empty(2, 3, 2);
            Solution solution = new Solution(new[] { 1, 2 }, new[] { 0, 1 });
            Assert.IsFalse(c.IsSolution(solution));
        }

        [Test]
        public void BasicCase_KnownSolutionIsValid()
        {
            // Josep laboratorio y videollamada, Ari biblioteca y estudiando,
            // Luna residencia y leyendo, Nora cafetería y cenando
            Solution solution = new Solution(new[] { 2, 0, 3, 1 }, new[] { 2,
            0, 3, 1 });
            Assert.IsTrue(ExampleCases.Basic().IsSolution(solution));
        }
        [Test]
        public void BasicCase_AriAndNoraSwapped_NotValid()
        {
            // Lo mismo pero intercambiando a Ari y Nora. Falla "Nora no estaba en la biblioteca".
            Solution solution = new Solution(new[] { 2, 1, 3, 0 }, new[] { 2, 1, 3, 0 });
            Assert.IsFalse(ExampleCases.Basic().IsSolution(solution));
        }



    }
}