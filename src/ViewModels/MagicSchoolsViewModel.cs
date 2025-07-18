using System.Collections.ObjectModel;
using RitualOS.Models;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// Provides a library of magical and occult schools for reference. 🧙‍♂️
    /// </summary>
    public class MagicSchoolsViewModel : ViewModelBase
    {
        public ObservableCollection<MagicSchool> Schools { get; } = new();

        public MagicSchoolsViewModel()
        {
            LoadSchools();
        }

        private void LoadSchools()
        {
            // A small sampling of traditions. Expand as desired!
            Schools.Add(new MagicSchool
            {
                Name = "Wicca",
                Overview = "Wicca is a modern pagan religion focused on nature worship and ritual magic. It was popularized in the 1950s and emphasizes balance between feminine and masculine energies.",
                CommonPractices = "Sabbats, esbat moon rituals, spellcraft with herbs and candles.",
                WhoIsItFor = "Seekers drawn to earth-centered spirituality and structured ritual." 
            });

            Schools.Add(new MagicSchool
            {
                Name = "Haitian Vodou",
                Overview = "An African diasporic religion blending West African traditions with Catholicism, honoring spirits called lwa.",
                CommonPractices = "Spirit possession, drumming, dance, and offerings to the lwa.",
                WhoIsItFor = "Communities with ancestral ties to Haiti and those interested in syncretic Afro-Caribbean practices." 
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hoodoo",
                Overview = "A system of African American folk magic rooted in ancestral practices and herbal knowledge.",
                CommonPractices = "Candle work, mojo bags, and protection rituals.",
                WhoIsItFor = "Practitioners seeking pragmatic spellwork blended with Christian folk traditions." 
            });

            Schools.Add(new MagicSchool
            {
                Name = "Chaos Magic",
                Overview = "A modern approach that treats belief as a tool and encourages using whatever works to achieve results.",
                CommonPractices = "Sigil creation, meditation, and personalized ritual experiments.",
                WhoIsItFor = "Experimenters who value flexibility over tradition." 
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hermeticism",
                Overview = "A philosophical and spiritual tradition based on writings attributed to Hermes Trismegistus, encompassing alchemy and astrology.",
                CommonPractices = "Ritual magic, study of the Hermetica, and contemplative meditation.",
                WhoIsItFor = "Occult students interested in historic Western esotericism." 
            });

            Schools.Add(new MagicSchool
            {
                Name = "Thelema",
                Overview = "Founded by Aleister Crowley, Thelema teaches discovering one's True Will and honors several Thelemic deities.",
                CommonPractices = "Ceremonial rites, yoga, and the study of Crowley's texts including The Book of the Law.",
                WhoIsItFor = "Those drawn to ceremonial magic and personal spiritual liberation." 
            });

            Schools.Add(new MagicSchool
            {
                Name = "Orgonite Work",
                Overview = "Inspired by Wilhelm Reich's concept of orgone energy, practitioners craft resin, metal, and crystal devices to balance subtle energies.",
                CommonPractices = "Creating orgonite pieces, meditating with orgone accumulators, gifting pieces to the environment.",
                WhoIsItFor = "Energy workers and tinkerers exploring alternative healing concepts." 
            });

            Schools.Add(new MagicSchool
            {
                Name = "Technomancy",
                Overview = "A hybrid practice blending magical intent with modern technology, often in a playful or experimental manner.",
                CommonPractices = "Sigils encoded in QR codes, using computers or electronics as ritual tools.",
                WhoIsItFor = "Mages who love gadgets and the intersection of tech with mysticism." 
            });
        }
    }
}
