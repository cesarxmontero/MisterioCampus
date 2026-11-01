using NUnit.Framework;
using Misterio.Core;

namespace Misterio.Tests
{
    public class TechniqueTests
    {
        [Test]
        public void DirectClue_AppliesTheFirstClue()
        {
            CaseDefinition c = ExampleCases.Basic();
            CaseGrid grid = new CaseGrid(4, 4, 4);

            Step step = new DirectClue().TryApply(c, grid);

            Assert.AreEqual("Josep no estaba en la biblioteca.", step.Text);
            Assert.AreEqual(1, step.Cost);
            Assert.IsFalse(grid.IsPossible(0, Category.Place, 0));
        }

        [Test]
        public void DirectClue_NoClues_ReturnsNull()
        {
            CaseDefinition c = ExampleCases.Empty(3, 3, 3);

            Assert.IsNull(new DirectClue().TryApply(c, new CaseGrid(3, 3, 3)));
        }

        [Test]
        public void Exclusivity_RemovesTheValueFromTheOthers()
        {
            // Josep solo puede estar en una videollamada
            CaseDefinition c = ExampleCases.Basic();
            CaseGrid grid = new CaseGrid(4, 4, 4);
            grid.KeepOnly(0, Category.Activity, 2);

            Step step = new Exclusivity().TryApply(c, grid);

            Assert.AreEqual("Josep estaba en una videollamada, así que nadie más estaba en una videollamada.", step.Text);
            for (int s = 1; s < 4; s++)
                Assert.IsFalse(grid.IsPossible(s, Category.Activity, 2));
        }

        [Test]
        public void UniqueValue_OnlyOneCanStudy_Applies()
        {
            // 3 sospechosos y 3 actividades. Josep y Ari no estudiaban, así que estudiaba Luna
            CaseDefinition c = ThreeSuspects(false);
            CaseGrid grid = new CaseGrid(3, 3, 3);
            grid.Remove(0, Category.Activity, 0);
            grid.Remove(1, Category.Activity, 0);

            Step step = new UniqueValue().TryApply(c, grid);

            Assert.AreEqual("Solo Luna puede estar estudiando, así que Luna estaba estudiando.", step.Text);
            Assert.AreEqual(0, grid.OnlyOption(2, Category.Activity));
        }

        [Test]
        public void UniqueValue_ExtraActivity_DoesNotApply()
        {
            // Lo mismo pero con una actividad de más. Puede que nadie estudiara así que no se deduce nad
            CaseDefinition c = ThreeSuspects(true);
            CaseGrid grid = new CaseGrid(3, 3, 4);
            grid.Remove(0, Category.Activity, 0);
            grid.Remove(1, Category.Activity, 0);

            Assert.IsNull(new UniqueValue().TryApply(c, grid));
        }

        [Test]
        public void UniqueValue_RelationClueSaysSomeoneDidIt()
        {
            // Con una actividad de más, pero la pista dice que alguien cenaba en la biblioteca
            // Si Josep y Ari no cenaban, cenaba Luna
            CaseDefinition c = ThreeSuspects(true);
            c.Clues.Add(new RelationClue(0, 1, true));
            CaseGrid grid = new CaseGrid(3, 3, 4);
            grid.Remove(0, Category.Activity, 1);
            grid.Remove(1, Category.Activity, 1);

            Step step = new UniqueValue().TryApply(c, grid);

            Assert.AreEqual("Solo Luna puede estar cenando, así que Luna estaba cenando.", step.Text);
        }

        [Test]
        public void UniqueValue_LibraryAlwaysHasSomeone()
        {
            // 3 sospechosos y 4 lugares. Sobra un lugar pero la biblioteca nunca está vacía
            CaseDefinition c = ExampleCases.Empty(3, 4, 3);
            CaseGrid grid = new CaseGrid(3, 4, 3);
            grid.Remove(0, Category.Place, 0);
            grid.Remove(1, Category.Place, 0);

            Step step = new UniqueValue().TryApply(c, grid);

            Assert.IsNotNull(step);
            Assert.AreEqual(0, grid.OnlyOption(2, Category.Place));
        }

        [Test]
        public void CombineClues_Positive_JosepDidNotDine()
        {
            // El ejemplo de la memoria
            CaseDefinition c = ExampleCases.Basic();
            CaseGrid grid = new CaseGrid(4, 4, 4);
            grid.Remove(0, Category.Activity, 1);

            Step step = new CombineClues().TryApply(c, grid);

            Assert.AreEqual("Josep no estaba cenando y quien estaba en la cafetería estaba cenando, así que Josep no estaba en la cafetería.", step.Text);
            Assert.AreEqual(3, step.Cost);
            Assert.IsFalse(grid.IsPossible(0, Category.Place, 1));
        }

        [Test]
        public void CombineClues_Negative_AriWasNotReading()
        {
            // Nadie estaba leyendo en la biblioteca y Ari estaba en la biblioteca
            CaseDefinition c = ThreeSuspects(false);
            c.Clues.Add(new RelationClue(0, 2, false));
            CaseGrid grid = new CaseGrid(3, 3, 3);
            grid.KeepOnly(1, Category.Place, 0);

            Step step = new CombineClues().TryApply(c, grid);

            Assert.AreEqual("Ari estaba en la biblioteca y nadie estaba leyendo en la biblioteca, así que Ari no estaba leyendo.", step.Text);
            Assert.IsFalse(grid.IsPossible(1, Category.Activity, 2));
        }

        // Josep, Ari y Luna en 3 lugares, con una actividad de más por si la necesito
        static CaseDefinition ThreeSuspects(bool extraActivity)
        {
            CaseDefinition c = new CaseDefinition();
            c.Suspects.AddRange(new[] { "Josep", "Ari", "Luna" });
            c.Places.AddRange(new[] { "la biblioteca", "la cafetería", "el laboratorio" });
            c.Activities.AddRange(new[] { "estudiando", "cenando", "leyendo" });
            if (extraActivity)
                c.Activities.Add("en una videollamada");
            return c;
        }
    }
}
