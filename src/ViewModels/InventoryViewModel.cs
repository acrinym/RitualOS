using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RitualOS.Helpers;
using RitualOS.Models;
using RitualOS.Services;

namespace RitualOS.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        private InventoryItem _selectedItem;
        private string _message = string.Empty;

        public ObservableCollection<InventoryItem> Items { get; } = new();
        public ObservableCollection<Chakra> SelectedChakras { get; } = new();

        public InventoryItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                    if (_selectedItem != null)
                    {
                        SelectedChakras.Clear();
                        foreach (var chakra in _selectedItem.AssociatedChakras)
                        {
                            SelectedChakras.Add(chakra);
                        }
                    }
                }
            }
        }

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

        public ICommand AddItemCommand { get; }
        public ICommand SaveItemCommand { get; }
        public ICommand AddChakraCommand { get; }

        public InventoryViewModel()
        {
            var items = InventoryDataLoader.LoadAllItems("inventory");
            foreach (var item in items)
            {
                Items.Add(item);
                if (item.Quantity < 5)
                {
                    Message += $"Warning: Low quantity for {item.Name} ({item.Quantity} left). ";
                }
                if (item.ExpirationDate.HasValue && item.ExpirationDate.Value < DateTime.Now)
                {
                    Message += $"Warning: {item.Name} expired on {item.ExpirationDate.Value.ToShortDateString()}. ";
                }
            }

            AddItemCommand = new RelayCommand(_ => AddItem());
            SaveItemCommand = new RelayCommand(_ => SaveItem(), _ => SelectedItem != null && !string.IsNullOrWhiteSpace(SelectedItem.Name) && SelectedItem.Quantity >= 0);
            AddChakraCommand = new RelayCommand(param => AddChakra(param as Chakra?));
        }

        private void AddItem()
        {
            var newItem = new InventoryItem { Name = "New Item", Category = "Herb", Quantity = 1, StorageLocation = "" };
            Items.Add(newItem);
            SelectedItem = newItem;
        }

        private void SaveItem()
        {
            if (SelectedItem == null)
            {
                Message = "No item selected.";
                return;
            }

            try
            {
                SelectedItem.AssociatedChakras = SelectedChakras.ToList();
                InventoryDataLoader.SaveItemToJson(SelectedItem, $"inventory/{SelectedItem.Name}.json");
                Message = $"Item '{SelectedItem.Name}' saved successfully!";
            }
            catch (Exception ex)
            {
                Message = $"Error saving item: {ex.Message}";
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