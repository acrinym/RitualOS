using System;
using System.Collections.ObjectModel;
using System.Linq;
using RitualOS.Models;
using System.IO;
using System.Text.Json;

namespace RitualOS.ViewModels
{
    public class BookOfShadowsViewModel : ViewModelBase
    {
        public ObservableCollection<BoSPage> RootPages { get; } = new();
        private BoSPage? _selectedPage;
        public BoSPage? SelectedPage
        {
            get => _selectedPage;
            set { _selectedPage = value; OnPropertyChanged(); }
        }
        public string SearchText { get; set; } = string.Empty;
        public ObservableCollection<BoSPage> FilteredPages { get; } = new();
        private const string DataFile = "bos_data.json";

        public BookOfShadowsViewModel()
        {
            Load();
            ApplyFilter();
        }

        public void AddPage(Guid? parentId = null)
        {
            var page = new BoSPage { Title = "New Page" };
            if (parentId == null)
                RootPages.Add(page);
            else
            {
                var parent = FindPageById(parentId.Value, RootPages);
                parent?.Children.Add(page);
            }
            Save();
            ApplyFilter();
        }

        public void DeletePage(BoSPage page)
        {
            if (page.ParentId == null)
                RootPages.Remove(page);
            else
            {
                var parent = FindPageById(page.ParentId.Value, RootPages);
                parent?.Children.Remove(page);
            }
            Save();
            ApplyFilter();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(RootPages, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFile, json);
        }

        public void Load()
        {
            RootPages.Clear();
            if (!File.Exists(DataFile)) return;
            var json = File.ReadAllText(DataFile);
            var pages = JsonSerializer.Deserialize<ObservableCollection<BoSPage>>(json);
            if (pages != null)
                foreach (var p in pages) RootPages.Add(p);
        }

        public void ApplyFilter()
        {
            FilteredPages.Clear();
            foreach (var page in RootPages)
                AddFiltered(page, SearchText);
        }

        private void AddFiltered(BoSPage page, string search)
        {
            if (string.IsNullOrWhiteSpace(search) || page.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || page.Tags.Any(t => t.Contains(search, StringComparison.OrdinalIgnoreCase)))
                FilteredPages.Add(page);
            foreach (var child in page.Children)
                AddFiltered(child, search);
        }

        private BoSPage? FindPageById(Guid id, ObservableCollection<BoSPage> pages)
        {
            foreach (var page in pages)
            {
                if (page.Id == id) return page;
                var found = FindPageById(id, new ObservableCollection<BoSPage>(page.Children));
                if (found != null) return found;
            }
            return null;
        }
    }
} 