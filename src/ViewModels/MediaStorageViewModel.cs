using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Collections.Generic;
using RitualOS.Services;
using RitualOS.Helpers;

namespace RitualOS.ViewModels
{
    public class MediaStorageViewModel : ViewModelBase
    {
        private readonly MediaStorageService _mediaStorage;
        
        // Search and Filter Properties
        private string _searchText = string.Empty;
        private MediaStorageService.MediaType _selectedType = MediaStorageService.MediaType.Other;
        private MediaStorageService.MediaCategory _selectedCategory = MediaStorageService.MediaCategory.Other;
        private string _selectedTag = string.Empty;
        private bool _showAdvancedFilters = false;
        
        // Display Properties
        private MediaStorageService.MediaFile? _selectedMedia;
        private bool _isLoading = false;
        private bool _isUploading = false;
        private string _uploadProgress = string.Empty;
        private long _uploadedBytes = 0;
        private long _totalBytes = 0;
        
        // Collections
        public ObservableCollection<MediaStorageService.MediaFile> AllMedia { get; } = new();
        public ObservableCollection<MediaStorageService.MediaFile> FilteredMedia { get; } = new();
        public ObservableCollection<string> AvailableTags { get; } = new();
        public ObservableCollection<MediaStorageService.MediaType> AvailableTypes { get; } = new();
        public ObservableCollection<MediaStorageService.MediaCategory> AvailableCategories { get; } = new();
        public ObservableCollection<string> AvailableSymbolIds { get; } = new();

        // Commands
        public ICommand SearchCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand UploadMediaCommand { get; }
        public ICommand DeleteMediaCommand { get; }
        public ICommand DownloadMediaCommand { get; }
        public ICommand PlayMediaCommand { get; }
        public ICommand ViewMediaCommand { get; }
        public ICommand CreateBackupCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAdvancedFiltersCommand { get; }
        public ICommand ExportMediaCommand { get; }
        public ICommand BatchUploadCommand { get; }

        public MediaStorageViewModel(MediaStorageService mediaStorage)
        {
            _mediaStorage = mediaStorage;
            
            // Initialize commands
            SearchCommand = new RelayCommand(_ => PerformSearch());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            UploadMediaCommand = new RelayCommand(_ => UploadMedia());
            DeleteMediaCommand = new RelayCommand(_ => DeleteSelectedMedia(), _ => SelectedMedia != null);
            DownloadMediaCommand = new RelayCommand(_ => DownloadSelectedMedia(), _ => SelectedMedia != null);
            PlayMediaCommand = new RelayCommand(_ => PlaySelectedMedia(), _ => CanPlaySelectedMedia());
            ViewMediaCommand = new RelayCommand(_ => ViewSelectedMedia(), _ => CanViewSelectedMedia());
            CreateBackupCommand = new RelayCommand(_ => CreateBackup());
            RefreshCommand = new RelayCommand(_ => RefreshData());
            ToggleAdvancedFiltersCommand = new RelayCommand(_ => ShowAdvancedFilters = !ShowAdvancedFilters);
            ExportMediaCommand = new RelayCommand(_ => ExportMedia());
            BatchUploadCommand = new RelayCommand(_ => BatchUpload());

            // Initialize collections
            InitializeCollections();
        }

        #region Properties

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public MediaStorageService.MediaType SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public MediaStorageService.MediaCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public string SelectedTag
        {
            get => _selectedTag;
            set
            {
                if (_selectedTag != value)
                {
                    _selectedTag = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        public MediaStorageService.MediaFile? SelectedMedia
        {
            get => _selectedMedia;
            set
            {
                if (_selectedMedia != value)
                {
                    _selectedMedia = value;
                    OnPropertyChanged();
                    // Update command states
                    (DeleteMediaCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DownloadMediaCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PlayMediaCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (ViewMediaCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ShowAdvancedFilters
        {
            get => _showAdvancedFilters;
            set
            {
                if (_showAdvancedFilters != value)
                {
                    _showAdvancedFilters = value;
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

        public bool IsUploading
        {
            get => _isUploading;
            set
            {
                if (_isUploading != value)
                {
                    _isUploading = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UploadProgress
        {
            get => _uploadProgress;
            set
            {
                if (_uploadProgress != value)
                {
                    _uploadProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public long UploadedBytes
        {
            get => _uploadedBytes;
            set
            {
                if (_uploadedBytes != value)
                {
                    _uploadedBytes = value;
                    OnPropertyChanged();
                    UpdateUploadProgress();
                }
            }
        }

        public long TotalBytes
        {
            get => _totalBytes;
            set
            {
                if (_totalBytes != value)
                {
                    _totalBytes = value;
                    OnPropertyChanged();
                    UpdateUploadProgress();
                }
            }
        }

        public double UploadPercentage => TotalBytes > 0 ? (double)UploadedBytes / TotalBytes * 100 : 0;

        #endregion

        #region Initialization

        private void InitializeCollections()
        {
            // Initialize types
            AvailableTypes.Clear();
            foreach (MediaStorageService.MediaType type in Enum.GetValues(typeof(MediaStorageService.MediaType)))
            {
                AvailableTypes.Add(type);
            }

            // Initialize categories
            AvailableCategories.Clear();
            foreach (MediaStorageService.MediaCategory category in Enum.GetValues(typeof(MediaStorageService.MediaCategory)))
            {
                AvailableCategories.Add(category);
            }
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                await _mediaStorage.InitializeAsync();
                await LoadAllDataAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAllDataAsync()
        {
            // Load all media
            var allMedia = await _mediaStorage.GetAllMediaAsync();
            AllMedia.Clear();
            foreach (var media in allMedia)
            {
                AllMedia.Add(media);
            }

            // Load available filters
            await LoadAvailableFiltersAsync();

            // Perform initial search
            PerformSearch();
        }

        private async Task LoadAvailableFiltersAsync()
        {
            var metadata = await _mediaStorage.GetMetadataAsync();
            
            AvailableTags.Clear();
            AvailableTags.Add(string.Empty); // Empty option
            foreach (var tag in metadata.AllTags)
            {
                AvailableTags.Add(tag);
            }

            AvailableSymbolIds.Clear();
            AvailableSymbolIds.Add(string.Empty); // Empty option
            foreach (var symbolId in metadata.AllSymbolIds)
            {
                AvailableSymbolIds.Add(symbolId);
            }
        }

        #endregion

        #region Search and Filtering

        private void PerformSearch()
        {
            var results = AllMedia.AsQueryable();

            // Apply type filter
            if (SelectedType != MediaStorageService.MediaType.Other)
            {
                results = results.Where(m => m.Type == SelectedType);
            }

            // Apply category filter
            if (SelectedCategory != MediaStorageService.MediaCategory.Other)
            {
                results = results.Where(m => m.Category == SelectedCategory);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchTerm = SearchText.ToLowerInvariant();
                results = results.Where(m => 
                    m.OriginalName.ToLowerInvariant().Contains(searchTerm) ||
                    m.Description.ToLowerInvariant().Contains(searchTerm) ||
                    m.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchTerm)) ||
                    m.Metadata.Any(meta => meta.Value.ToLowerInvariant().Contains(searchTerm))
                );
            }

            // Apply tag filter
            if (!string.IsNullOrWhiteSpace(SelectedTag))
            {
                results = results.Where(m => m.Tags.Contains(SelectedTag, StringComparer.OrdinalIgnoreCase));
            }

            // Update filtered results
            FilteredMedia.Clear();
            foreach (var media in results.OrderByDescending(m => m.LastAccessed))
            {
                FilteredMedia.Add(media);
            }
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedType = MediaStorageService.MediaType.Other;
            SelectedCategory = MediaStorageService.MediaCategory.Other;
            SelectedTag = string.Empty;
            ShowAdvancedFilters = false;
        }

        #endregion

        #region Media Management

        private async void UploadMedia()
        {
            // TODO: Implement file upload dialog
            // This would typically open a file dialog and upload the selected files
            IsUploading = true;
            try
            {
                // Simulate upload process
                await Task.Delay(2000);
                await RefreshData();
            }
            finally
            {
                IsUploading = false;
            }
        }

        private async void DeleteSelectedMedia()
        {
            if (SelectedMedia != null)
            {
                // TODO: Show confirmation dialog
                // For now, just delete directly
                await _mediaStorage.DeleteMediaAsync(SelectedMedia.Id);
                await LoadAllDataAsync();
            }
        }

        private async void DownloadSelectedMedia()
        {
            if (SelectedMedia != null)
            {
                // TODO: Implement download functionality
                // This would typically open a save dialog and download the file
                try
                {
                    using var stream = await _mediaStorage.RetrieveMediaAsync(SelectedMedia.Id);
                    // TODO: Save stream to file
                }
                catch (Exception ex)
                {
                    // TODO: Show error message
                }
            }
        }

        private void PlaySelectedMedia()
        {
            if (SelectedMedia != null)
            {
                // TODO: Implement media player
                // This would open the media in the appropriate player
            }
        }

        private void ViewSelectedMedia()
        {
            if (SelectedMedia != null)
            {
                // TODO: Implement media viewer
                // This would open the media in the appropriate viewer
            }
        }

        private bool CanPlaySelectedMedia()
        {
            return SelectedMedia != null && 
                   (SelectedMedia.Type == MediaStorageService.MediaType.Audio || 
                    SelectedMedia.Type == MediaStorageService.MediaType.Video);
        }

        private bool CanViewSelectedMedia()
        {
            return SelectedMedia != null && 
                   (SelectedMedia.Type == MediaStorageService.MediaType.Image || 
                    SelectedMedia.Type == MediaStorageService.MediaType.Svg ||
                    SelectedMedia.Type == MediaStorageService.MediaType.Document);
        }

        private async void CreateBackup()
        {
            try
            {
                await _mediaStorage.CreateBackupAsync();
                // TODO: Show success message
            }
            catch (Exception ex)
            {
                // TODO: Show error message
            }
        }

        private void ExportMedia()
        {
            // TODO: Implement export functionality
            // This would typically open a file dialog and export the filtered media
        }

        private async void BatchUpload()
        {
            // TODO: Implement batch upload functionality
            // This would allow uploading multiple files at once
        }

        private async void RefreshData()
        {
            await LoadAllDataAsync();
        }

        private void UpdateUploadProgress()
        {
            if (TotalBytes > 0)
            {
                var percentage = (double)UploadedBytes / TotalBytes * 100;
                UploadProgress = $"{UploadedBytes:N0} / {TotalBytes:N0} bytes ({percentage:F1}%)";
            }
            else
            {
                UploadProgress = "Ready to upload";
            }
        }

        #endregion

        #region Helper Methods

        public string GetTypeDisplayName(MediaStorageService.MediaType type)
        {
            var field = type.GetType().GetField(type.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
            return attribute?.DisplayName ?? type.ToString();
        }

        public string GetCategoryDisplayName(MediaStorageService.MediaCategory category)
        {
            var field = category.GetType().GetField(category.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
            return attribute?.DisplayName ?? category.ToString();
        }

        public string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        public string GetMediaIcon(MediaStorageService.MediaType type)
        {
            return type switch
            {
                MediaStorageService.MediaType.Image => "🖼️",
                MediaStorageService.MediaType.Audio => "🎵",
                MediaStorageService.MediaType.Video => "🎬",
                MediaStorageService.MediaType.Document => "📄",
                MediaStorageService.MediaType.Svg => "🎨",
                MediaStorageService.MediaType.Model3D => "🗿",
                MediaStorageService.MediaType.Archive => "📦",
                _ => "📁"
            };
        }

        public async Task<MediaStorageService.MediaMetadata> GetMetadataAsync()
        {
            return await _mediaStorage.GetMetadataAsync();
        }

        public async Task<MediaStorageService.MediaIndex> GetIndexAsync()
        {
            return await _mediaStorage.GetIndexAsync();
        }

        #endregion
    }
} 