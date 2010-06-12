using System;
using System.IO;
using System.Windows;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Execution;
using StoryTeller.Model;
using StoryTeller.UserInterface;
using StoryTeller.UserInterface.Actions;
using StoryTeller.UserInterface.Projects;
using StoryTeller.Workspace;
using StructureMap;

namespace StoryTeller.Testing
{
    public static class DataMother
    {
        public const string THE_GRAMMAR_FILE = @"..\..\..\..\samples\grammars.xml";
        public const string THE_MATH_FILE = @"..\..\..\..\samples\math.xml";

        public static IScreenObjectRegistry ScreenObjectRegistry()
        {
            return new ScreenObjectRegistry(new Window(), new Container(), new Shell());
        }

        public static Project MathProject()
        {
            var directory = Path.GetDirectoryName(THE_MATH_FILE);
            var empty = Path.Combine(directory, "EmptySuite");
            if (!Directory.Exists(empty))
            {
                Directory.CreateDirectory(empty);
            }

            return Project.LoadFromFile(THE_MATH_FILE);
        }

        public static string MathProjectFile()
        {
            return THE_MATH_FILE;
        }

        public static FixtureLibrary MathLibrary()
        {
            return MathProject().LocalRunner().Library;
        }

        public static Project GrammarProject()
        {
            return Project.LoadFromFile(THE_GRAMMAR_FILE);
        }

        public static ProjectHistory HistoryPointingToMathProject()
        {
            var history = new ProjectHistory();
            history.Store(new ProjectToken
            {
                Name = "Math",
                Filename = THE_MATH_FILE
            });

            return history;
        }

        public static Test FailedTest()
        {
            var test = new Test(Guid.NewGuid().ToString())
            {
                LastResult = new TestResult()
            };
            test.LastResult.ExecutionTime = 2.34;
            test.LastResult.Counts.IncrementWrongs();

            return test;
        }

        public static Test ExceptionTest()
        {
            var test = new Test(Guid.NewGuid().ToString())
            {
                LastResult = new TestResult()
            };
            test.LastResult.ExecutionTime = 2.34;
            test.LastResult.Counts.IncrementExceptions();

            return test;
        }

        public static Test SuccessfulTest()
        {
            var test = new Test(Guid.NewGuid().ToString())
            {
                LastResult = new TestResult(),

            };
            test.LastResult.ExecutionTime = 2.34;
            test.LastResult.Counts.IncrementRights();

            return test;
        }

        public static Test TestWithNoResults()
        {
            return new Test(Guid.NewGuid().ToString());
        }

        public static Hierarchy BuildHierarchy(string text)
        {
            var hierarchy = new Hierarchy("new hierarchy");

            var reader = new StringReader(text);
            string lineText;
            while ((lineText = reader.ReadLine()) != null)
            {
                if (lineText.Trim() == string.Empty) continue;

                buildTest(lineText, hierarchy);
            }


            return hierarchy;
        }

        private static void buildTest(string lineText, Hierarchy hierarchy)
        {
            string[] parts = lineText.Split(',');
            Test test = hierarchy.AddTest(parts[0]);

            if (test.LastResult == null)
            {
                test.LastResult = new TestResult();
            }

            switch (parts[1])
            {
                case "Success":
                    test.LastResult.Counts.IncrementRights();
                    break;

                case "Failure":
                    test.LastResult.Counts.IncrementWrongs();
                    break;

                case "Unknown":
                    test.LastResult = null;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (parts.Length >= 3)
            {
                test.Lifecycle = (Lifecycle)Enum.Parse(typeof(Lifecycle), parts[2]);
            }
        }

        public static void LoadMathProject()
        {
            ObjectFactory.GetInstance<IProjectController>().LoadProject(new ProjectToken { Filename = THE_MATH_FILE });
        }

        public static Test[] TestArray(int count)
        {
            var returnValue = new Test[count];
            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = SuccessfulTest();
            }

            return returnValue;
        }

        public static ProjectTestRunner MathProjectRunner()
        {
            return new ProjectTestRunner(THE_MATH_FILE);
        }

        public static ProjectTestRunner GrammarsProjectRunner()
        {
            return new ProjectTestRunner(THE_GRAMMAR_FILE);
        }
    }
}