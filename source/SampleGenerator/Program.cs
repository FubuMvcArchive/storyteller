using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StoryTeller.Persistence;


namespace SampleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new SampleReader(args[0], args[1]);
            reader.FindSamples();
        }
    }

    public class SampleReader
    {
        private readonly string _codeFolder;
        private readonly string _outputFolder;

        public SampleReader(string codeFolder, string outputFolder)
        {
            _codeFolder = codeFolder;
            _outputFolder = outputFolder;
        }

        public void FindSamples()
        {
            
            if (Directory.Exists(_outputFolder))
            {
                Directory.Delete(_outputFolder, true);
            }

            Console.WriteLine("Creating folder " + _outputFolder);
            Directory.CreateDirectory(_outputFolder);

            ReadDirectory(_codeFolder);
        }

        public static void Read(string filename, Action<string> callback)
        {
            using (var reader = new StreamReader(filename))
            {
                string line;
                while ( (line=reader.ReadLine()) != null)
                {
                    callback(line.TrimEnd()) ;
                }
            }
        }

        public void ReadDirectory(string directory)
        {
            Console.WriteLine("Searching directory {0} for samples", directory);
            var system = new FileSystem();
            system.GetFiles(directory, "cs").Each(f => readFile(f));

            system.GetSubFolders(directory).Each(dir => ReadDirectory(dir));
        }

        private void readFile(string file)
        {
            Console.WriteLine("  reading " + file);
            var collector = new FileCollector(file);
            collector.Read().Each(x => x.WriteFile(_outputFolder));
        }
    }

    public class FileCollector
    {
        private readonly string _file;

        public FileCollector(string file)
        {
            _file = file;
        }

        private readonly IList<SampleCollector> _collectors = new List<SampleCollector>();

        public IList<SampleCollector> Read()
        {
            SampleReader.Read(_file, readLine);

            return _collectors;
        }

        private void readLine(string line)
        {
            if (line.TrimStart().StartsWith("// SAMPLE:"))
            {
                var sampleName = line.Split(':')[1].Trim();
                Console.WriteLine("    found '{0}'", sampleName);
                _collectors.Add(new SampleCollector(sampleName));
            }

            _collectors.Each(x => x.AddLine(line));
        }
    }

    public class SampleCollector
    {
        private readonly string _name;
        private readonly StringWriter _writer = new StringWriter();

        public SampleCollector(string name)
        {
            _name = name;
        }

        private bool _latched = false;
        public void AddLine(string line)
        {
            if (_latched) return;

            string startingString = line.TrimStart();
            if (startingString.StartsWith("// END:") && line.EndsWith(_name))
            {
                _latched = true;
                return;
            }

            if (startingString.StartsWith("// END:") || startingString.StartsWith("// SAMPLE:")) return;

            _writer.WriteLine(line);
        }

        public void WriteFile(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string file = Path.Combine(folder, _name + ".txt");
            new FileSystem().WriteStringToFile(_writer.GetStringBuilder().ToString().TrimEnd(), file);
        }
    }

     
}
