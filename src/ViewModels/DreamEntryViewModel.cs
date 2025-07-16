using System;
using System.Collections.ObjectModel;
using System.Linq; // Added for ToList
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    public class DreamEntryViewModel : ViewModelBase
    {
        private DreamEntry _entry = new();
        private string _message = string.Empty;

        public ObservableCollection<string> Symbols { get; } = new();
        public ObservableCollection<Chakra> SelectedChakras { get; } = new();

        public string Title
        {
            get => _entry.Title;
            set
            {
                _entry.Title = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _entry.Description;
            set
            {
                _entry.Description = value;
                OnPropertyChanged();
            }
        }

        public string Interpretation
        {
            get => _entry.Interpretation;
            set
            {
                _entry.Interpretation = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => _entry.Date;
            set
            {
                _entry.Date = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand AddSymbolCommand { get; }
        public ICommand AddChakraCommand { get; }

        public DreamEntryViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save(), _ => !string.IsNullOrWhiteSpace(Title));
            AddSymbolCommand = new RelayCommand(param => AddSymbol(param as string));
            AddChakraCommand = new RelayCommand(param => AddChakra(param as Chakra?));
        }

        private void Save()
        {
            try
            {
                _entry.Symbols = Symbols.ToList();
                _entry.Chakras = SelectedChakras.ToList();
                DreamDataLoader.SaveDream(_entry, $"dreams/{_entry.Id}.json");
                Message = "Dream saved successfully!";
            }
            catch (Exception ex)
            {
                Message = $"Error saving dream: {ex.Message}";
            }
        }

        private void AddSymbol(string? symbol)
        {
            if (!string.IsNullOrWhiteSpace(symbol) && !Symbols.Contains(symbol))
            {
                Symbols.Add(symbol);
            }
        }

        private void AddChakra(Chakra? chakra)
        {
            if (chakra.HasValue && !SelectedChakras.Contains(chakra.Value))
            {
                SelectedChakras.Add(chakra.Value);
            }
        }
    }
}