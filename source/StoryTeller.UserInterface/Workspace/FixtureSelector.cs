using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using StoryTeller.Model;
using StoryTeller.Workspace;

namespace StoryTeller.UserInterface.Workspace
{
    public class FixtureSelector : StackPanel, IFixtureSelector
    {
        private readonly FixtureDto _dto;
        private CheckBox _checkbox;

        public FixtureSelector(FixtureDto dto)
        {
            _dto = dto;

            Height = 25;
            this.Horizontal();
            _checkbox = this.Add<CheckBox>();
            _checkbox.Padding = new Thickness(0, 5, 0, 0);
            _checkbox.VerticalAlignment = VerticalAlignment.Center;

            this.Add<System.Windows.Controls.Label>(l =>
            {
                l.Content = dto.Name;
                l.VerticalAlignment = VerticalAlignment.Top;
            });

            ToolTip = new ToolTip() {Content = dto.Fullname};
        }

        // for testing
        public void Select(bool isSelected)
        {
            _checkbox.IsChecked = isSelected;
        }

        public IEnumerable<FixtureFilter> GetFilters()
        {
            if (!IsSelected()) yield break;

            yield return new FixtureFilter()
            {
                Name = _dto.Name,
                Type = FilterType.Fixture
            };
        }

        public bool IsSelected()
        {
            return _checkbox.IsChecked.GetValueOrDefault(false);
        }

        public void Enable(bool enabled)
        {
            IsEnabled = enabled;
        }
    }
}