using System;
using System.Collections.Generic;
using System.Linq;
using StoryTeller.Execution;
using StoryTeller.Workspace;

namespace StoryTellerRunner
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("StoryTellerRunner.exe ProjectFile1 ProjectFile2 ResultsFile");

                return 1;
            }

            string resultsFile = args.Last();
            var projects = new List<IProject>();
            for (int i = 0; i < (args.Length - 1); i++)
            {
                Console.WriteLine("Loading Project file at " + args[i]);

                Project project = Project.LoadFromFile(args[i]);

                projects.Add(project);
            }


            var runner = new ProjectRunner(projects, resultsFile);

            return runner.Execute();
        }
    }
}