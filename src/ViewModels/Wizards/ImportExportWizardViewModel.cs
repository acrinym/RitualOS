using System.Threading.Tasks;
using RitualOS.Services;
using RitualOS.Helpers;

namespace RitualOS.ViewModels.Wizards
{
    public class ImportExportWizardViewModel : ViewModelBase
    {
        private readonly IDataImportExportService _service;
        private string _importPath = string.Empty;
        private string _exportPath = string.Empty;

        public ImportExportWizardViewModel() : this(new DataImportExportService())
        {
        }

        public ImportExportWizardViewModel(IDataImportExportService service)
        {
            _service = service;
            ImportCommand = new RelayCommand(async () => await ImportAsync(), () => !string.IsNullOrWhiteSpace(ImportPath));
            ExportCommand = new RelayCommand(async () => await ExportAsync(), () => !string.IsNullOrWhiteSpace(ExportPath));
        }

        public string ImportPath
        {
            get => _importPath;
            set
            {
                SetProperty(ref _importPath, value);
                ImportCommand.RaiseCanExecuteChanged();
            }
        }

        public string ExportPath
        {
            get => _exportPath;
            set
            {
                SetProperty(ref _exportPath, value);
                ExportCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand ImportCommand { get; }
        public RelayCommand ExportCommand { get; }

        private async Task ImportAsync()
        {
            await _service.ImportRitualLogsAsync(ImportPath);
        }

        private async Task ExportAsync()
        {
            await _service.ExportAllDataAsync(ExportPath);
        }
    }
}
