using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
            LoadDocumentCommand = new RelayCommand((param) => LoadDocument());
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

            var result = await TopLevel.GetTopLevel(mainWindow)?.StorageProvider.OpenFilePickerAsync(options);
            if (result != null && result.Count > 0)
            {
                DocumentPath = result[0].Path.LocalPath;
            }
        }

        private void LoadDocument()
        {
            if (string.IsNullOrWhiteSpace(DocumentPath) || !File.Exists(DocumentPath))
            {
                DocumentContent = "File not found.";
                return;
            }

            DocumentFile doc = DocumentLoader.Load(DocumentPath);
            DocumentContent = doc.Content;
        }
    }
}
