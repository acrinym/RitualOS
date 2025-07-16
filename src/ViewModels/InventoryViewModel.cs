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
        private InventoryItem? _selectedItem;
        private string _message = string.Empty;

        public ObservableCollection<InventoryItem> Items { get; } = new();
        public ObservableCollection<Chakra> SelectedChakras { get; } = new();
        public ObservableCollection<InventoryAlert> Alerts { get; } = new();

        public InventoryItem? SelectedItem
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
        public ICommand ClearAlertCommand { get; }

        public InventoryViewModel()
        {
            var items = InventoryDataLoader.LoadAllItems("inventory");
            foreach (var item in items)
            {
                Items.Add(item);
                if (item.Quantity < 5)
                {
                    Alerts.Add(new InventoryAlert
                    {
                        ItemName = item.Name,
                        AlertType = "LowQuantity",
                        Message = $"Low quantity for {item.Name} ({item.Quantity} left)."
                    });
                }
                if (item.ExpirationDate.HasValue && item.ExpirationDate.Value < DateTime.Now)
                {
                    Alerts.Add(new InventoryAlert
                    {
                        ItemName = item.Name,
                        AlertType = "Expired",
                        Message = $"{item.Name} expired on {item.ExpirationDate.Value.ToShortDateString()}."
                    });
                }
            }

            AddItemCommand = new RelayCommand(_ => AddItem());
            SaveItemCommand = new RelayCommand(_ => SaveItem(), _ => SelectedItem != null && !string.IsNullOrWhiteSpace(SelectedItem.Name) && SelectedItem.Quantity >= 0);
            AddChakraCommand = new RelayCommand(param => AddChakra(param as Chakra?));
            ClearAlertCommand = new RelayCommand(param => ClearAlert(param as InventoryAlert));
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
                UpdateAlerts();
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

        private void ClearAlert(InventoryAlert? alert)
        {
            if (alert == null) return;
            var item = Items.FirstOrDefault(i => i.Name == alert.ItemName);
            if (item != null)
            {
                if (alert.AlertType == "LowQuantity")
                {
                    item.Quantity = 10; // Reset to a safe quantity
                    InventoryDataLoader.SaveItemToJson(item, $"inventory/{item.Name}.json");
                }
                else if (alert.AlertType == "Expired")
                {
                    item.ExpirationDate = null; // Clear expiration
                    InventoryDataLoader.SaveItemToJson(item, $"inventory/{item.Name}.json");
                }
            }
            Alerts.Remove(alert);
            Message = $"Alert for {alert.ItemName} cleared.";
        }

        private void UpdateAlerts()
        {
            Alerts.Clear();
            foreach (var item in Items)
            {
                if (item.Quantity < 5)
                {
                    Alerts.Add(new InventoryAlert
                    {
                        ItemName = item.Name,
                        AlertType = "LowQuantity",
                        Message = $"Low quantity for {item.Name} ({item.Quantity} left)."
                    });
                }
                if (item.ExpirationDate.HasValue && item.ExpirationDate.Value < DateTime.Now)
                {
                    Alerts.Add(new InventoryAlert
                    {
                        ItemName = item.Name,
                        AlertType = "Expired",
                        Message = $"{item.Name} expired on {item.ExpirationDate.Value.ToShortDateString()}."
                    });
                }
            }
        }
    }
}