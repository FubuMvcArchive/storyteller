using System;
using System.IO;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Execution;
using StoryTeller.UserInterface;
using StoryTeller.UserInterface.Exploring;
using StoryTeller.UserInterface.Projects;
using StoryTeller.Workspace;
using StructureMap;

namespace StoryTeller.AcceptanceTesting
{
    public class FakeRunner : TestRunner
    {
        public FakeRunner()
            : base(x => { })
        {
        }
    }

    public class StoryTellerRunner : TestRunner
    {
        public const string PROJECT_FILE = "StoryTellerInternal.xml";
        public const string PROJECT_FOLDER = "Project";
        public const string PROJECT_NAME = "StoryTellerInternal";
        public const string TEST_FOLDER = "Project/TestFolder";

        public StoryTellerRunner()
            : base(x => x.AddFixturesFromThisAssembly())
        {
        }

        protected override void setUp(ITestContext context)
        {
            var container = new Container(new UserInterfaceRegistry());
            container.Configure(x => { x.For<ITestEngine>().Use<InProcessTestEngine>(); });

            Bootstrapper.StartupShell(container);

            createProjectFolders();

            var project = new Project
            {
                BinaryFolder = "",
                FileName = PROJECT_FILE,
                ProjectFolder = PROJECT_FOLDER,
                Name = PROJECT_NAME,
                TestRunnerTypeName = typeof(FakeRunner).AssemblyQualifiedName
            };

            Hierarchy hierarchy = project.LoadTests();

            context.Store(new SystemUnderTest(ObjectFactory.Container, project, hierarchy));
        }

        private void createProjectFolders()
        {
            if (Directory.Exists(PROJECT_FOLDER))
            {
                Directory.Delete(PROJECT_FOLDER, true);
            }
            Directory.CreateDirectory(PROJECT_FOLDER);
            Directory.CreateDirectory(TEST_FOLDER);
        }

        protected override void tearDown(ITestContext context)
        {
        }
    }

    public class SystemUnderTest
    {
        private readonly IContainer _container;
        private readonly Hierarchy _hierarchy;
        private readonly Project _project;

        public SystemUnderTest(IContainer container, Project project, Hierarchy hierarchy)
        {
            _container = container;
            _project = project;
            _hierarchy = hierarchy;
        }

        public Hierarchy Hierarchy { get { return _container.GetInstance<ITestExplorer>().CurrentHierarchy; } }

        public void Do<T>(Action<T> action)
        {
            action(_container.GetInstance<T>());
        }

        public void AddTest(string path)
        {
            Test test = _hierarchy.AddTest(path);
            _project.Save(test);
        }

        public void ReloadProject()
        {
            Do<IProjectController>(x => x.LoadProject(_project.ToProjectToken()));
        }
    }
}