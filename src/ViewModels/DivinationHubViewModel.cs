using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.ViewModels
{
    public class DivinationHubViewModel : ViewModelBase
    {
        public ObservableCollection<DivinationDraw> History { get; } = new();
        public ObservableCollection<TarotCard> Deck { get; } = new();
        public ObservableCollection<TarotCard> CurrentDraw { get; } = new();
        public string JournalEntry { get; set; } = string.Empty;
        public string DrawType { get; set; } = "Single";
        private const string DataFile = "divination_history.json";

        public DivinationHubViewModel()
        {
            LoadDeck();
            LoadHistory();
        }

        public void DrawCards()
        {
            CurrentDraw.Clear();
            var rng = new Random();
            var deck = Deck.OrderBy(_ => rng.Next()).ToList();
            int count = DrawType == "ThreeCard" ? 3 : 1;
            foreach (var card in deck.Take(count))
                CurrentDraw.Add(card);
        }

        public void SaveDraw()
        {
            var draw = new DivinationDraw
            {
                Date = DateTime.Now,
                Type = DivinationType.Tarot,
                Spread = DrawType == "ThreeCard" ? DivinationSpread.ThreeCard : DivinationSpread.Single,
                CardNames = CurrentDraw.Select(c => c.Name).ToList(),
                Journal = JournalEntry
            };
            History.Insert(0, draw);
            SaveHistory();
        }

        private void LoadDeck()
        {
            // For MVP, hardcode a few cards; in real app, load all 78 from data
            Deck.Clear();
            Deck.Add(new TarotCard { Name = "The Fool", MeaningUp = "New beginnings, innocence, adventure." });
            Deck.Add(new TarotCard { Name = "The Magician", MeaningUp = "Manifestation, resourcefulness, power." });
            Deck.Add(new TarotCard { Name = "The High Priestess", MeaningUp = "Intuition, mystery, subconscious." });
            Deck.Add(new TarotCard { Name = "The Empress", MeaningUp = "Fertility, beauty, nature." });
            Deck.Add(new TarotCard { Name = "The Emperor", MeaningUp = "Authority, structure, control." });
            // ...add more as needed
        }

        private void SaveHistory()
        {
            var json = JsonSerializer.Serialize(History, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFile, json);
        }

        private void LoadHistory()
        {
            History.Clear();
            if (!File.Exists(DataFile)) return;
            var json = File.ReadAllText(DataFile);
            var draws = JsonSerializer.Deserialize<ObservableCollection<DivinationDraw>>(json);
            if (draws != null)
                foreach (var d in draws) History.Add(d);
        }
    }
} 