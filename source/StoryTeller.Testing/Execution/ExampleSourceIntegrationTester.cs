using System.Xml;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Examples;
using StoryTeller.Execution;
using StoryTeller.Model;
using StoryTeller.UserInterface;
using StructureMap;

namespace StoryTeller.Testing.Execution
{
    [TestFixture]
    public class ExampleSourceIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Bootstrapper.ForceRestart();

            library = DataMother.MathProject().LocalRunner().Library;
            ObjectFactory.Inject(library);
            source = ObjectFactory.With(library).GetInstance<ExampleSource>();
        }

        #endregion

        private ExampleSource source;
        private FixtureLibrary library;

        [Test]
        public void get_example_by_fixture()
        {
            Example example = source.BuildExample(new TPath("Math"));
            example.Title.ShouldEqual("Math");
            example.Description.ShouldEqual("The description of the MathFixture");

            // the xml is there
            var document = new XmlDocument();
            document.LoadXml(example.Xml);
            document.DocumentElement.Name.ShouldEqual("Test");
            document.DocumentElement.FirstChild.ChildNodes.Count.ShouldBeGreaterThan(1);

            example.Html.ShouldNotBeNull();
        }

        [Test]
        public void get_example_by_grammar_path()
        {
            Example example = source.BuildExample(new TPath("Math/MultiplyBy"));
            example.Title.ShouldEqual("*= {multiplier}");
            example.Description.ShouldEqual("This grammar multiplies two numbers together");

            // the xml is there
            var document = new XmlDocument();
            document.LoadXml(example.Xml);
            document.DocumentElement.Name.ShouldEqual("Test");
            document.SelectSingleNode("//Math/MultiplyBy").ShouldNotBeNull();

            // should only be showing one element within the first and only section
            // for the grammar in question
            document.DocumentElement.FirstChild.ChildNodes.Count.ShouldEqual(1);

            example.Html.ShouldNotBeNull();
        }

        [Test]
        public void get_example_by_hierarchy()
        {
            Example example = source.BuildExample(TPath.Empty);
            example.Title.ShouldEqual("All Fixtures");
            example.Description.ShouldBeEmpty();

            var document = new XmlDocument();
            document.LoadXml(example.Xml);

            // all Fixture nodes should be there
            document.DocumentElement.ChildNodes.Count.ShouldBeGreaterThan(1);
        }
    }
}