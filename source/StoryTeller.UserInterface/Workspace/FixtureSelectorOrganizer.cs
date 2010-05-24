using System;
using System.Diagnostics;
using FubuCore.Util;
using StoryTeller.Model;
using System.Collections.Generic;
using StoryTeller.Workspace;
using System.Linq;

namespace StoryTeller.UserInterface.Workspace
{
    public class FixtureSelectorOrganizer : IFixtureSelectorOrganizer
    {
        public IEnumerable<IFixtureSelector> Organize(FixtureLibrary library, WorkspaceFilter workspace)
        {
            var namespaces = new Cache<string, NamespaceSelector>(ns => new NamespaceSelector(ns));
            var fixtures = new Cache<string, FixtureSelector>();

            // pass #1, collate the namespaces
            string[] names = getAllNamespaces(library, namespaces);

            var topLevels = collateNamespaces(namespaces, names);
            buildFixtureSelectors(library, namespaces, fixtures);

            selectFixtures(fixtures, workspace);
            selectNamespaces(namespaces, workspace);

            namespaces.Each(x => x.SetInitialState());

            return topLevels.ToArray(); 
        }

        private void selectNamespaces(Cache<string, NamespaceSelector> namespaces, WorkspaceFilter workspace)
        {
            workspace.Filters.Where(x => x.Type == FilterType.Namespace).Each(x => namespaces[x.Name].Select(true));
        }

        private void selectFixtures(Cache<string, FixtureSelector> fixtures, WorkspaceFilter workspace)
        {
            workspace.Filters
                .Where(x => x.Type == FilterType.Fixture)
                .Each(x => fixtures.WithValue(x.Name, selector => selector.Select(true)));
        }

        private void buildFixtureSelectors(FixtureLibrary library, Cache<string, NamespaceSelector> namespaces, Cache<string, FixtureSelector> fixtures)
        {
            library.AllFixtures.Each(x =>
            {
                var item = new FixtureSelector(x);
                fixtures[x.Name] = item;

                namespaces[x.Namespace].Add(item);
            });
        }

        private List<NamespaceSelector> collateNamespaces(Cache<string, NamespaceSelector> namespaces, string[] names)
        {
            List<NamespaceSelector> topLevels = new List<NamespaceSelector>();

            var parents = new List<string>(names);
            parents.Sort();
            parents.Reverse();

            for (int i = 0; i < parents.Count; i++)
            {
                var ns = parents[i];
                var parent = parents.FirstOrDefault(x => ns.StartsWith(x) && x != ns);
                if (parent == null)
                {
                    topLevels.Add(namespaces[ns]);
                }
                else
                {
                    namespaces[parent].Add(namespaces[ns]);
                }
            }

            return topLevels;
        }

        private string[] getAllNamespaces(FixtureLibrary library, Cache<string, NamespaceSelector> namespaces)
        {
            library.AllFixtures.Each(x =>
            {
                var o = namespaces[x.Namespace];
            });

            var names = namespaces.GetAllKeys();
            Array.Sort(names);

            return names;
        }
    }
}