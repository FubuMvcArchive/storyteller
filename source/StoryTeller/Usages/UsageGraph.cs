using System;
using System.Collections.Generic;
using FubuCore.Util;
using StoryTeller.Domain;
using StoryTeller.Model;
using System.Linq;

namespace StoryTeller.Usages
{
    public class UsageGraph : ITestStream
    {
        private readonly FixtureLibrary _library;
        private readonly IUsageGraphListener _listener;
        private readonly Cache<string, FixtureUsage> _fixtures = new Cache<string,FixtureUsage>();
        private Test _currentTest;

        public UsageGraph(FixtureLibrary library, IUsageGraphListener listener)
        {
            _library = library;
            _listener = listener;

            _fixtures.OnMissing = name =>
            {
                var fixture = library.FixtureFor(name);
                return new FixtureUsage(fixture);
            };
        }

        public void Rebuild(IEnumerable<Test> tests)
        {
            _fixtures.ClearAll();

            _listener.Start(tests.Count());

            tests.Each(x =>
            {
                _listener.ReadingTest(x.LocatorPath());

                var parser = new TestParser(x, this, _library);
                parser.Parse();
            });
        }

        public FixtureUsage ForFixture(string name)
        {
            return _fixtures[name];
        }

        public IEnumerable<FixtureGraph> FixturesFor(string workspaceName)
        {
            return _fixtures.GetAll().Where(x => x.HasUsageInWorkspace(workspaceName)).Distinct().Select(x => x.Fixture);
        }

        public IEnumerable<Test> TestsFor(string fixtureName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Test> TestsFor(string fixtureName, string grammarKey)
        {
            throw new NotImplementedException();
        }

        void ITestStream.Comment(Comment comment)
        {
        }

        void ITestStream.InvalidSection(Section section)
        {
            ForFixture(section.FixtureName).Mark(_currentTest);
        }

        private readonly Stack<FixtureGraph> _fixtureStack = new Stack<FixtureGraph>();

        void ITestStream.StartSection(Section section, FixtureGraph fixture)
        {
            ForFixture(section.FixtureName).Mark(_currentTest);
            _fixtureStack.Push(fixture);
        }

        void ITestStream.EndSection(Section section)
        {
            _fixtureStack.Pop();
        }

        void ITestStream.Sentence(Sentence sentence, IStep step)
        {
            markGrammar(sentence.Name);
        }

        private void markGrammar(string name)
        {
            ForFixture(_fixtureStack.Peek().Name).MarkGrammar(_currentTest, name);
        }

        void ITestStream.InvalidGrammar(string grammarKey, IStep step)
        {
            markGrammar(grammarKey);
        }

        void ITestStream.Table(Table table, IStep step)
        {
            markGrammar(table.Name);
        }

        void ITestStream.SetVerification(SetVerification verification, IStep step)
        {
            markGrammar(verification.Name);
        }

        void ITestStream.StartParagraph(Paragraph paragraph, IStep step)
        {
            markGrammar(paragraph.Name);
        }

        void ITestStream.EndParagraph(Paragraph paragraph, IStep step)
        {
            
        }

        void ITestStream.StartEmbeddedSection(EmbeddedSection section, IStep step)
        {
            markGrammar(section.Name);
            ForFixture(section.Fixture.Name).Mark(_currentTest);
            _fixtureStack.Push(section.Fixture);
        }

        void ITestStream.EndEmbeddedSection(EmbeddedSection section, IStep step)
        {
            _fixtureStack.Pop();
        }

        void ITestStream.StartTest(Test test)
        {
            _currentTest = test;
        }

        void ITestStream.EndTest(Test test)
        {
        }

        void ITestStream.IncrementParagraphGrammar()
        {
        }

        void ITestStream.Do(DoGrammarStructure structure, IStep step)
        {
        }

        public void Rebuild(Hierarchy hierarchy)
        {
            Rebuild(hierarchy.GetAllTests());
        }

        public IEnumerable<FixtureGraph> AllFixtures()
        {
            return _fixtures.GetAll().Select(x => x.Fixture);
        }
    }
}