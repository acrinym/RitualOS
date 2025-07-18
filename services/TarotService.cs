using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RitualOS.Models;

namespace RitualOS.Services
{
    /// <summary>
    /// Provides tarot deck loading and random card draws. ✨
    /// </summary>
    public class TarotService
    {
        private readonly List<TarotCard> _deck = new();
        private readonly Random _rand = new();

        public IReadOnlyList<TarotCard> Deck => _deck;

        public TarotService()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "tarot", "tarot_cards.json");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var cards = JsonSerializer.Deserialize<List<TarotCard>>(json) ?? new List<TarotCard>();
                _deck.AddRange(cards);
            }
        }

        public TarotCard DrawCard()
        {
            if (_deck.Count == 0) throw new InvalidOperationException("Deck not loaded");
            return _deck[_rand.Next(_deck.Count)];
        }

        public List<TarotCard> DrawCards(int count)
        {
            return Enumerable.Range(0, count).Select(_ => DrawCard()).ToList();
        }
    }
}
