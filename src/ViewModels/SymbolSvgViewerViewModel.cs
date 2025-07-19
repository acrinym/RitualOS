using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using RitualOS.Helpers;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    public class SymbolSvgViewerViewModel : ViewModelBase
    {
        private readonly SymbolImageService _imageService;
        
        // Display properties
        private string _symbolName = string.Empty;
        private IBitmap? _svgSource;
        private string _svgContent = string.Empty;
        private bool _isLoading = false;
        private bool _hasError = false;
        private string _errorMessage = string.Empty;
        
        // View properties
        private double _zoomLevel = 100.0;
        private double _rotation = 0.0;
        private bool _flipHorizontal = false;
        private bool _flipVertical = false;
        private bool _isSelected = false;
        private bool _showGrid = false;
        private bool _showCenterLines = false;
        
        // Canvas properties
        private double _canvasWidth = 800;
        private double _canvasHeight = 600;
        private double _svgWidth = 400;
        private double _svgHeight = 400;
        private double _svgX = 200;
        private double _svgY = 100;
        
        // File properties
        private string _svgDimensions = "0 × 0";
        private string _fileSize = "0 KB";

        public SymbolSvgViewerViewModel(SymbolImageService imageService)
        {
            _imageService = imageService;
            
            // Initialize commands
            ZoomInCommand = new RelayCommand(_ => ZoomIn());
            ZoomOutCommand = new RelayCommand(_ => ZoomOut());
            ResetZoomCommand = new RelayCommand(_ => ResetZoom());
            RotateCommand = new RelayCommand(_ => Rotate());
            FlipHorizontalCommand = new RelayCommand(_ => FlipHorizontal());
            FlipVerticalCommand = new RelayCommand(_ => FlipVertical());
            CopySvgCommand = new RelayCommand(_ => CopySvg());
            SaveSvgCommand = new RelayCommand(_ => SaveSvg());
            ExportPngCommand = new RelayCommand(_ => ExportPng());
            RetryCommand = new RelayCommand(_ => Retry());
        }

        #region Properties

        public string SymbolName
        {
            get => _symbolName;
            set
            {
                if (_symbolName != value)
                {
                    _symbolName = value;
                    OnPropertyChanged();
                    _ = LoadSymbolAsync();
                }
            }
        }

        public IBitmap? SvgSource
        {
            get => _svgSource;
            set
            {
                if (_svgSource != value)
                {
                    _svgSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                if (_zoomLevel != value)
                {
                    _zoomLevel = value;
                    OnPropertyChanged();
                    UpdateSvgSize();
                }
            }
        }

        public double Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool FlipHorizontal
        {
            get => _flipHorizontal;
            set
            {
                if (_flipHorizontal != value)
                {
                    _flipHorizontal = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool FlipVertical
        {
            get => _flipVertical;
            set
            {
                if (_flipVertical != value)
                {
                    _flipVertical = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                if (_showGrid != value)
                {
                    _showGrid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowCenterLines
        {
            get => _showCenterLines;
            set
            {
                if (_showCenterLines != value)
                {
                    _showCenterLines = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                if (_canvasWidth != value)
                {
                    _canvasWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                if (_canvasHeight != value)
                {
                    _canvasHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SvgWidth
        {
            get => _svgWidth;
            set
            {
                if (_svgWidth != value)
                {
                    _svgWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SvgHeight
        {
            get => _svgHeight;
            set
            {
                if (_svgHeight != value)
                {
                    _svgHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SvgX
        {
            get => _svgX;
            set
            {
                if (_svgX != value)
                {
                    _svgX = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SvgY
        {
            get => _svgY;
            set
            {
                if (_svgY != value)
                {
                    _svgY = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SvgDimensions
        {
            get => _svgDimensions;
            set
            {
                if (_svgDimensions != value)
                {
                    _svgDimensions = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FileSize
        {
            get => _fileSize;
            set
            {
                if (_fileSize != value)
                {
                    _fileSize = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ResetZoomCommand { get; }
        public ICommand RotateCommand { get; }
        public ICommand FlipHorizontalCommand { get; }
        public ICommand FlipVerticalCommand { get; }
        public ICommand CopySvgCommand { get; }
        public ICommand SaveSvgCommand { get; }
        public ICommand ExportPngCommand { get; }
        public ICommand RetryCommand { get; }

        #endregion

        #region Methods

        private async Task LoadSymbolAsync()
        {
            if (string.IsNullOrEmpty(SymbolName))
                return;

            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                // Get SVG content from the image service
                var svgContent = await _imageService.GetSvgContentAsync(SymbolName);
                
                if (!string.IsNullOrEmpty(svgContent))
                {
                    _svgContent = svgContent;
                    
                    // Convert SVG to bitmap for display
                    await LoadSvgAsBitmapAsync(svgContent);
                    
                    // Update file information
                    UpdateFileInfo();
                }
                else
                {
                    // Create placeholder SVG
                    _svgContent = CreatePlaceholderSvg();
                    await LoadSvgAsBitmapAsync(_svgContent);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Failed to load symbol: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadSvgAsBitmapAsync(string svgContent)
        {
            try
            {
                // For now, we'll create a simple placeholder bitmap
                // In a real implementation, you'd use an SVG rendering library
                var bitmap = CreatePlaceholderBitmap();
                SvgSource = bitmap;
                
                // Update dimensions
                SvgDimensions = $"{bitmap.PixelSize.Width} × {bitmap.PixelSize.Height}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to render SVG: {ex.Message}", ex);
            }
        }

        private IBitmap CreatePlaceholderBitmap()
        {
            // Create a simple placeholder bitmap
            var width = 200;
            var height = 200;
            var bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888);
            
            using (var lockedBitmap = bitmap.Lock())
            {
                var buffer = lockedBitmap.Address;
                var stride = lockedBitmap.RowBytes;
                
                // Fill with a simple pattern
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var offset = y * stride + x * 4;
                        var color = (x + y) % 2 == 0 ? 0xFF7B68EE : 0xFF3E3E42;
                        
                        unsafe
                        {
                            *(uint*)(buffer + offset) = color;
                        }
                    }
                }
            }
            
            return bitmap;
        }

        private string CreatePlaceholderSvg()
        {
            return $@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 200 200"">
  <rect width=""200"" height=""200"" fill=""#3E3E42"" stroke=""#7B68EE"" stroke-width=""2""/>
  <text x=""100"" y=""100"" text-anchor=""middle"" dominant-baseline=""middle"" 
        font-family=""Arial"" font-size=""16"" fill=""#7B68EE"">
    {SymbolName}
  </text>
</svg>";
        }

        private void UpdateFileInfo()
        {
            if (!string.IsNullOrEmpty(_svgContent))
            {
                var sizeInBytes = System.Text.Encoding.UTF8.GetByteCount(_svgContent);
                var sizeInKb = sizeInBytes / 1024.0;
                FileSize = $"{sizeInKb:F1} KB";
            }
        }

        private void UpdateSvgSize()
        {
            var baseSize = 200.0;
            var scale = ZoomLevel / 100.0;
            
            SvgWidth = baseSize * scale;
            SvgHeight = baseSize * scale;
            
            // Center the SVG
            SvgX = (CanvasWidth - SvgWidth) / 2;
            SvgY = (CanvasHeight - SvgHeight) / 2;
        }

        #endregion

        #region Command Implementations

        private void ZoomIn()
        {
            ZoomLevel = Math.Min(ZoomLevel * 1.2, 500.0);
        }

        private void ZoomOut()
        {
            ZoomLevel = Math.Max(ZoomLevel / 1.2, 10.0);
        }

        private void ResetZoom()
        {
            ZoomLevel = 100.0;
            Rotation = 0.0;
            FlipHorizontal = false;
            FlipVertical = false;
        }

        private void Rotate()
        {
            Rotation = (Rotation + 90) % 360;
        }

        private void FlipHorizontal()
        {
            FlipHorizontal = !FlipHorizontal;
        }

        private void FlipVertical()
        {
            FlipVertical = !FlipVertical;
        }

        private async void CopySvg()
        {
            if (!string.IsNullOrEmpty(_svgContent))
            {
                // Copy SVG content to clipboard
                // This would require clipboard integration
                await Task.CompletedTask;
            }
        }

        private async void SaveSvg()
        {
            if (!string.IsNullOrEmpty(_svgContent))
            {
                // Save SVG to file
                // This would require file dialog integration
                await Task.CompletedTask;
            }
        }

        private async void ExportPng()
        {
            if (SvgSource != null)
            {
                // Export current view as PNG
                // This would require file dialog integration
                await Task.CompletedTask;
            }
        }

        private void Retry()
        {
            _ = LoadSymbolAsync();
        }

        #endregion
    }
} 