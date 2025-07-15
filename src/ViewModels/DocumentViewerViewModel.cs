using System.IO;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// View model for displaying various document types.
    /// </summary>
    public class DocumentViewerViewModel : ViewModelBase
    {
        private string _documentPath = string.Empty;
        private string _documentContent = "Select a document.";

        public string DocumentPath
        {
            get => _documentPath;
            set
            {
                if (_documentPath != value)
                {
                    _documentPath = value;
                    OnPropertyChanged();
                    LoadDocument();
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
