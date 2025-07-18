using System.Collections.ObjectModel;
using RitualOS.Models;
using RitualOS.Services;
using RitualOS.Helpers;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// ViewModel for the tarot spread viewer.
    /// </summary>
    public class TarotViewModel : ViewModelBase
    {
        private readonly TarotService _service = new();

        public ObservableCollection<TarotCard> DrawnCards { get; } = new();

        public RelayCommand DrawOneCommand { get; }
        public RelayCommand DrawThreeCommand { get; }
        public RelayCommand DrawFullCommand { get; }

        public TarotViewModel()
        {
            DrawOneCommand = new RelayCommand(_ => DrawCards(1));
            DrawThreeCommand = new RelayCommand(_ => DrawCards(3));
            DrawFullCommand = new RelayCommand(_ => DrawCards(10));
        }

        private void DrawCards(int count)
        {
            DrawnCards.Clear();
            foreach (var card in _service.DrawCards(count))
                DrawnCards.Add(card);
        }
    }
}
