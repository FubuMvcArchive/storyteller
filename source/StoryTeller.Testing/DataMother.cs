using System;
using System.IO;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.UserInterface.Projects;
using StoryTeller.Workspace;
using StructureMap;

namespace StoryTeller.Testing
{
    public static class DataMother
    {
        private const string THE_GRAMMAR_FILE = @"..\..\..\..\samples\grammars.xml";
        private const string THE_MATH_FILE = @"..\..\..\..\samples\math.xml";

        public static Project MathProject()
        {
            return Project.LoadFromFile(THE_MATH_FILE);
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
    }
}