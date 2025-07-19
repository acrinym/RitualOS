using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using RitualOS.Models;
using RitualOS.Services;
using RitualOS.Helpers;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model for displaying various document types.
    /// </summary>
    public class DocumentViewerViewModel : ViewModelBase
    {
        private string _documentPath = string.Empty;
        private string _documentContent = "Select a document.";

        public DocumentViewerViewModel()
        {
            BrowseCommand = new RelayCommand(async (param) => await BrowseForFile());
            LoadDocumentCommand = new RelayCommand(async (param) => await LoadDocumentAsync());
        }

        public string DocumentPath
        {
            get => _documentPath;
            set
            {
                if (_documentPath != value)
                {
                    _documentPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DocumentContent
        {
            get => _documentContent;
            private set
            {
                if (_documentContent != value)
                {
                    _documentContent = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand BrowseCommand { get; }
        public RelayCommand LoadDocumentCommand { get; }

        private async Task BrowseForFile()
        {
            var options = new FilePickerOpenOptions
            {
                Title = "Select Document",
                AllowMultiple = false,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("All Files") { Patterns = new[] { "*" } },
                    new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt", "*.md" } },
                    new FilePickerFileType("PDF Files") { Patterns = new[] { "*.pdf" } }
                }
            };

            Window? mainWindow = null;
            if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                mainWindow = desktop.MainWindow;
            }

            IReadOnlyList<IStorageFile>? result = null;
            var topLevel = TopLevel.GetTopLevel(mainWindow);
            if (topLevel?.StorageProvider != null)
            {
                result = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            }

            if (result != null && result.Count > 0)
            {
                var item = result[0];
                if (item.Path != null)
                {
                    DocumentPath = item.Path.LocalPath;
                }
            }
        }

        private async Task LoadDocumentAsync()
        {
            if (string.IsNullOrWhiteSpace(DocumentPath) || !File.Exists(DocumentPath))
            {
                DocumentContent = "File not found.";
                return;
            }

            // Let the user know we're working some magic in the background! ✨
            DocumentContent = "Loading...";
            DocumentFile doc = await DocumentLoader.LoadAsync(DocumentPath);
            DocumentContent = doc.Content;
        }
    }
}
