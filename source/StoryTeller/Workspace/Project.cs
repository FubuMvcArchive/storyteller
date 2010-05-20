using System;
using System.IO;
using System.Xml.Serialization;
using FubuCore;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Persistence;

namespace StoryTeller.Workspace
{
    public interface IProject
    {
        string Name { get; set; }
        string FileName { get; }
        string ConfigurationFileName { get; }
        string SystemTypeName { get; }
        int TimeoutInSeconds { get; set; }
        string FixtureAssembly { get; set; }
        string GetBinaryFolder();
        Hierarchy LoadTests();
        void Save(Test test);
        void DeleteFile(Test test);
        void RenameTest(Test test, string name);

        ITestRunner LocalRunner();
        void CreateDirectory(Suite suite);
    }

    public class Project : IProject
    {
        private string _fileName;
        private string _projectFolder;
        private int _timeoutInSeconds;

        public Project()
        {
            TimeoutInSeconds = 5;
        }

        public Project(string filename)
            : this()
        {
            FileName = filename;
        }


        public string FixtureAssembly { get; set; }
        public string BinaryFolder { get; set; }

        public string TestFolder { get; set; }

        [XmlIgnore]
        public string ProjectFolder
        {
            get
            {
                if (_projectFolder.IsEmpty()) return Path.GetFullPath(".");

                return _projectFolder;
            }
            set
            {
                _projectFolder = value;
                if (!Path.IsPathRooted(_projectFolder) && _projectFolder.IsNotEmpty())
                {
                    _projectFolder = Path.GetFullPath(_projectFolder);
                }

                if (_projectFolder.IsEmpty())
                {
                    _projectFolder = Path.GetFullPath(".");
                }

            }
        }

        #region IProject Members

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                _projectFolder = Path.GetDirectoryName(_fileName);
            }
        }

        public int TimeoutInSeconds { get { return _timeoutInSeconds > 0 ? _timeoutInSeconds : 5; } set { _timeoutInSeconds = value; } }

        public string SystemTypeName { get; set; }
        public string Name { get; set; }

        public string ConfigurationFileName { get; set; }


        public string GetBinaryFolder()
        {
            return getCorrectPath(BinaryFolder);
        }

        public Hierarchy LoadTests()
        {
            var hierarchy = new Hierarchy(this);

            new HierarchyLoader(GetTestFolder(), hierarchy).Load();

            return hierarchy;
        }

        public void Save(Test test)
        {
            string path = GetTestPath(test);
            new TestWriter().WriteToFile(test, path);
        }

        public void DeleteFile(Test test)
        {
            string path = GetTestPath(test);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void RenameTest(Test test, string name)
        {
            DeleteFile(test);
            test.Name = name;
            Save(test);
        }

        public ITestRunner LocalRunner()
        {
            Type type = Type.GetType(SystemTypeName);
            var runner = Activator.CreateInstance(type).As<ITestRunner>();

            return runner;
        }

        public void CreateDirectory(Suite suite)
        {
            string path = Path.Combine(GetTestFolder(), suite.GetFolder());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        #endregion

        public static Project LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var fileSystem = new FileSystem();
            var project = fileSystem.LoadFromFile<Project>(filename);
            project.ProjectFolder = Path.GetDirectoryName(filename);
            project.FileName = filename;

            return project;
        }

        public string GetTestFolder()
        {
            return getCorrectPath(TestFolder);
        }

        public string GetBaseProjectFolder()
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(_fileName);
        }

        private string getCorrectPath(string folder)
        {

            if (folder.IsEmpty()) return string.Empty;

            if (Path.IsPathRooted(folder))
            {
                return folder;
            }


            string projectFolder = Path.IsPathRooted(ProjectFolder)
                                       ? ProjectFolder
                                       : Path.GetFullPath(ProjectFolder);


            string path = Path.Combine(projectFolder, folder);
            return Path.GetFullPath(path);
        }

        public void Save(string filename)
        {
            FileName = filename;
            new FileSystem().PersistToFile(this, filename);
        }

        public string GetTestPath(Test test)
        {
            string fileName = test.FileName;
            return Path.Combine(GetTestFolder(), fileName);
        }

        #region Nested type: HierarchyLoader

        public class HierarchyLoader
        {
            private readonly Hierarchy _hierarchy;
            private readonly ITestReader _reader = new TestReader();
            private readonly FileSystem _system = new FileSystem();
            private readonly string _topFolder;

            public HierarchyLoader(string topFolder, Hierarchy hierarchy)
            {
                _topFolder = topFolder;
                _hierarchy = hierarchy;
            }

            public void Load()
            {
                loadTestsInFolder(_topFolder, _hierarchy);
            }

            private void loadTestsInFolder(string folder, Suite parent)
            {
                foreach (string file in _system.GetFiles(folder, "xml"))
                {
                    Test test = _reader.ReadFromFile(file);
                    parent.AddTest(test);
                }

                // load the tests from the sub folders
                foreach (string subFolder in _system.GetSubFolders(folder))
                {
                    var child = new Suite(Path.GetFileName(subFolder));
                    parent.AddSuite(child);

                    loadTestsInFolder(subFolder, child);
                }
            }
        }

        #endregion
    }
}