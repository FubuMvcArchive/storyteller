using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class ReflectionAssertionTester
    {
        [Test,
         ExpectedException(typeof (ApplicationException), ExpectedMessage = "'NotAMethod' cannot be marked as an Assertion.  Only methods that return a boolean can be assertions")]
        public void cannot_make_an_assertion_out_of_a_method_that_does_not_return_bool()
        {
            MethodInfo method = typeof (AssertionFixture).GetMethod("NotAMethod");
            new ReflectionAssertion(method, new AssertionFixture());
        }

        [Test]
        public void execute_successfully()
        {
            ReflectionAssertion assertion = ReflectionAssertion.Create(x => x.Success(), new AssertionFixture());
            var step = new Step();

            var context = new TestContext();

            assertion.Execute(step, context);
            context.Counts.ShouldEqual(1, 0, 0, 0);

            step.Get("returnValue").Parse<bool>().ShouldBeTrue();
            context.ResultsFor(step).ActualDisplay<bool>("returnValue").ShouldBeTrue();
        }

        [Test]
        public void execute_with_exception()
        {
            ReflectionAssertion assertion = ReflectionAssertion.Create(x => x.Exception(), new AssertionFixture());
            var step = new Step();

            var context = new TestContext();

            assertion.Execute(step, context);
            context.Counts.ShouldEqual(0, 0, 1, 0);

            step.Get("returnValue").Parse<bool>().ShouldBeTrue();
            context.ResultsFor(step).ActualDisplay<bool>("returnValue").ShouldBeFalse();
            context.ResultsFor(step).ExceptionText.ShouldContain("NotImplementedException");
        }

        [Test]
        public void execute_with_false()
        {
            ReflectionAssertion assertion = ReflectionAssertion.Create(x => x.Failure(), new AssertionFixture());
            var step = new Step();
            var context = new TestContext();

            assertion.Execute(step, context);
            context.Counts.ShouldEqual(0, 1, 0, 0);

            step.Get("returnValue").Parse<bool>().ShouldBeTrue();
            context.ResultsFor(step).ActualDisplay<bool>("returnValue").ShouldBeFalse();
        }

        [Test]
        public void get_the_cells()
        {
            ReflectionAssertion assertion = ReflectionAssertion.Create(x => x.MethodWithArgs(null, 0),
                                                                       new AssertionFixture());
            IList<Cell> actual = assertion.GetCells();
            var expected = new List<Cell>
            {
                Cell.For<string>("name"),
                Cell.For<int>("age"),
                Cell.For<bool>("returnValue")
            };

            TestUtility.AssertListMatches((IList) actual, expected);
        }
    }

    public class AssertionFixture
    {
        public string NotAMethod()
        {
            return null;
        }

        [FormatAs("Success")]
        public bool Success()
        {
            return true;
        }

        [FormatAs("Failure")]
        public bool Failure()
        {
            return false;
        }

        [FormatAs("Exception")]
        public bool Exception()
        {
            throw new NotImplementedException();
        }

        public bool MethodWithArgs(string name, int age)
        {
            return true;
        }
    }
}