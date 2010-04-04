using System.Collections.Generic;
using System.Windows.Controls;
using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Testing.Engine;
using StoryTeller.Testing.UserInterface;

namespace StoryTeller.Testing
{
    public class StepExecutionResult
    {
        public IStepResults Results { get; set; }
        public Counts Counts { get; set; }
    }

    /// <summary>
    /// All strictly for testing
    /// </summary>
    public static class TestingExtensions
    {
        public static StepExecutionResult Execute(this IGrammar grammar, IStep step)
        {
            var context = new TestContext();

            grammar.Execute(step, context);

            return new StepExecutionResult()
            {
                Counts = context.Counts,
                Results = context.ResultsFor(step)
            };
        }

        public static T Mock<T>(this TestContext context) where T : class
        {
            var mock = MockRepository.GenerateMock<T>();
            context.Store(mock);

            return mock;
        }

        public static IGrammar SetupMockGrammar(this TestContext context, string stepName)
        {
            var fixture = MockRepository.GenerateMock<IFixture>();
            var grammar = MockRepository.GenerateMock<IGrammar>();
            fixture.Stub(x => x[stepName]).Return(grammar);
            context.LoadFixture(fixture, new StubTestPart());

            return grammar;
        }

        public static void HandleMessage<T>(this object listener, T message)
        {
            ((IListener<T>)listener).Handle(message);
        }

        public static IEnumerable<string> GetMenus(this ContextMenu menu)
        {
            var list = new List<string>();
            foreach (MenuItem item in menu.Items)
            {
                list.Add(item.Header.As<string>());
            }

            return list;
        }

        public static void ClickMenu(this ContextMenu menu, string text)
        {
            foreach (MenuItem item in menu.Items)
            {
                if (item.Header.Equals(text))
                {
                    ControlDriver.ClickOn(item);
                    return;
                }
            }

            Assert.Fail("Did not find the menu item");
        }
    }
}