using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    public class DreamEntryViewModel : ViewModelBase
    {
        private DreamEntry _dream = new();
        private string _message = string.Empty;

        public DreamEntry Dream
        {
            get => _dream;
            set
            {
                if (_dream != value)
                {
                    _dream = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Symbols { get; } = new();
        public ObservableCollection<Chakra> ChakraTags { get; } = new();

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand AddSymbolCommand { get; }
        public ICommand AddChakraCommand { get; }

        public DreamEntryViewModel()
        {
            Dream.Id = Guid.NewGuid().ToString();
            Dream.Date = DateTime.Now;
            SaveCommand = new RelayCommand(_ => Save(), _ => !string.IsNullOrWhiteSpace(Dream.Title));
            AddSymbolCommand = new RelayCommand(_ => Symbols.Add(string.Empty));
            AddChakraCommand = new RelayCommand(param => AddChakra(param as Chakra?));
        }

        private void Save()
        {
            try
            {
                Dream.Symbols = Symbols.ToList();
                Dream.ChakraTags = ChakraTags.ToList();
                DreamDataLoader.SaveDreamToJson(Dream, $"dreams/{Dream.Id}.json");
                Message = $"Dream '{Dream.Title}' saved successfully!";
            }
            catch (Exception ex)
            {
                Message = $"Error saving dream: {ex.Message}";
            }
        }

        private void AddChakra(Chakra? chakra)
        {
            if (chakra.HasValue && !ChakraTags.Contains(chakra.Value))
            {
                ChakraTags.Add(chakra.Value);
            }
        }
    }
}