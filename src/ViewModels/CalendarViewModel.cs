using System;
using System.Collections.ObjectModel;
using System.Globalization;
using RitualOS.Models;
using System.Linq;

namespace RitualOS.ViewModels
{
    public class CalendarViewModel : ViewModelBase
    {
        public ObservableCollection<CalendarEvent> Events { get; } = new();
        public ObservableCollection<DateTime> Sabbats { get; } = new();
        public ObservableCollection<CalendarDayViewModel> Days { get; } = new();

        private DateTime _displayMonth;
        public DateTime DisplayMonth
        {
            get => _displayMonth;
            set
            {
                if (_displayMonth != value)
                {
                    _displayMonth = value;
                    OnPropertyChanged();
                    GenerateMonthDays();
                }
            }
        }

        private CalendarDayViewModel? _selectedDay;
        public CalendarDayViewModel? SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    _selectedDay = value;
                    OnPropertyChanged();
                }
            }
        }

        public CalendarViewModel()
        {
            _displayMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            LoadSabbats(_displayMonth.Year);
            GenerateMonthDays();
        }

        public void NextMonth() => DisplayMonth = DisplayMonth.AddMonths(1);
        public void PreviousMonth() => DisplayMonth = DisplayMonth.AddMonths(-1);

        private void LoadSabbats(int year)
        {
            Sabbats.Clear();
            // Example: Add 8 sabbat dates for the year (Northern Hemisphere, Wiccan Wheel)
            Sabbats.Add(new DateTime(year, 2, 1));   // Imbolc
            Sabbats.Add(new DateTime(year, 3, 20));  // Ostara (Spring Equinox)
            Sabbats.Add(new DateTime(year, 5, 1));   // Beltane
            Sabbats.Add(new DateTime(year, 6, 21));  // Litha (Summer Solstice)
            Sabbats.Add(new DateTime(year, 8, 1));   // Lughnasadh
            Sabbats.Add(new DateTime(year, 9, 22));  // Mabon (Autumn Equinox)
            Sabbats.Add(new DateTime(year, 10, 31)); // Samhain
            Sabbats.Add(new DateTime(year, 12, 21)); // Yule (Winter Solstice)
        }

        private void GenerateMonthDays()
        {
            Days.Clear();
            var firstDay = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(DisplayMonth.Year, DisplayMonth.Month);
            var firstDayOfWeek = (int)firstDay.DayOfWeek;
            var calendarStart = firstDay.AddDays(-firstDayOfWeek);
            for (int i = 0; i < 42; i++) // 6 weeks grid
            {
                var date = calendarStart.AddDays(i);
                var isCurrentMonth = date.Month == DisplayMonth.Month;
                var isSabbat = Sabbats.Any(s => s.Month == date.Month && s.Day == date.Day);
                var moonPhase = MoonPhaseCalculator.GetPhase(date);
                var events = Events.Where(e => e.Date.Date == date.Date).ToList();
                Days.Add(new CalendarDayViewModel(date, isCurrentMonth, isSabbat, moonPhase, events));
            }
        }
    }

    public class CalendarDayViewModel : ViewModelBase
    {
        public DateTime Date { get; }
        public bool IsCurrentMonth { get; }
        public bool IsSabbat { get; }
        public string MoonPhase { get; }
        public ObservableCollection<CalendarEvent> Events { get; }

        public CalendarDayViewModel(DateTime date, bool isCurrentMonth, bool isSabbat, string moonPhase, System.Collections.Generic.List<CalendarEvent> events)
        {
            Date = date;
            IsCurrentMonth = isCurrentMonth;
            IsSabbat = isSabbat;
            MoonPhase = moonPhase;
            Events = new ObservableCollection<CalendarEvent>(events);
        }
    }

    // Simple moon phase calculator (returns emoji or phase name)
    public static class MoonPhaseCalculator
    {
        public static string GetPhase(DateTime date)
        {
            // Simple algorithm: https://www.subsystems.us/uploads/9/8/9/4/98948044/moonphase.pdf
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            if (month < 3)
            {
                year--;
                month += 12;
            }
            ++month;
            double c = 365.25 * year;
            double e = 30.6 * month;
            double jd = c + e + day - 694039.09; // jd is total days elapsed
            jd /= 29.5305882; // divide by the moon cycle
            double b = jd - Math.Floor(jd); // fractional part
            int phase = (int)(b * 8 + 0.5) % 8;
            return phase switch
            {
                0 => "🌑", // New Moon
                1 => "🌒", // Waxing Crescent
                2 => "🌓", // First Quarter
                3 => "🌔", // Waxing Gibbous
                4 => "🌕", // Full Moon
                5 => "🌖", // Waning Gibbous
                6 => "🌗", // Last Quarter
                7 => "🌘", // Waning Crescent
                _ => ""
            };
        }
    }
} 