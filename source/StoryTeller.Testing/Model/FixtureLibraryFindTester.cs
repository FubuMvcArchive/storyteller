using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Model;
using StoryTeller.Samples;

namespace StoryTeller.Testing.Model
{
    [TestFixture]
    public class FixtureLibraryFindTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var runner = new MathTestRunner();
            library = runner.Library;
        }

        #endregion

        private FixtureLibrary library;

        [Test]
        public void find_fixturegraph_by_path()
        {
            library.Find(new TPath("Math")).ShouldBeOfType<FixtureGraph>().Name.ShouldEqual("Math");
        }

        [Test]
        public void find_grammar_by_path()
        {
            library.Find(new TPath("Math/MultiplyBy")).ShouldBeOfType<GrammarStructure>().Name.ShouldEqual(
                "MultiplyBy");
        }

        [Test]
        public void find_the_library_with_an_empty_path()
        {
            library.Find(TPath.Empty).ShouldBeTheSameAs(library);
        }
    }
}