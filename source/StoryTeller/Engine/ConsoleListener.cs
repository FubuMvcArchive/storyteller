using System;
using FubuCore;
using StoryTeller.Domain;

namespace StoryTeller.Engine
{
    public class ConsoleListener : MarshalByRefObject, ITestObserver
    {
        #region ITestObserver Members

        public virtual void StartTest(Test test, Counts counts)
        {
            writeDivider();
            write("Starting Test {0} / {1}", test.Name, test.SuiteName);
        }

        public void StartSection(Section section)
        {
            write("Starting Section {0}", section.GetName());
        }

        public void FinishSection(Section section)
        {
        }

        public void StartStep(IStep step)
        {
            write("Starting Grammar {0}", step.GrammarKey);
        }

        public void FinishStep(IStep step)
        {
        }

        public virtual void FinishTest(Test test)
        {
            write("Test {0} finished:  {1}", test.Name, test.GetStatus());

            writeDivider();
            writeSpacer();
            writeSpacer();
            writeSpacer();
        }

        public void Exception(string exceptionString)
        {
            write(exceptionString);
        }

        public bool CanContinue(Counts counts)
        {
            return true;
        }

        #endregion

        #region Overrides of MarshalByRefObject

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        protected void write(string format, params string[] args)
        {
            string message = format.ToFormat(args);
            Console.WriteLine(message);
        }

        protected void writeDivider()
        {
            write(
                "=========================================================================================================");
        }

        private void writeSpacer()
        {
            Console.WriteLine();
        }
    }
}