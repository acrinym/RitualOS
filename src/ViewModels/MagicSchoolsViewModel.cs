using System.Collections.ObjectModel;
using RitualOS.Models;
using RitualOS.Helpers;
using System;
using System.Diagnostics;
using System.IO;

namespace RitualOS.ViewModels
{
    /// <summary>
    /// Provides a library of magical and occult schools for reference. 🧙‍♂️
    /// </summary>
    public class MagicSchoolsViewModel : ViewModelBase
    {
        public ObservableCollection<MagicSchool> Schools { get; } = new();
        public RelayCommand OpenDocCommand { get; }

        public MagicSchoolsViewModel()
        {
            LoadSchools();
            OpenDocCommand = new RelayCommand(param =>
            {
                if (param is string path)
                {
                    OpenDoc(path);
                }
            });
        }

        private void OpenDoc(string path)
        {
            var full = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (File.Exists(full))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = full,
                    UseShellExecute = true
                });
            }
        }

        private void LoadSchools()
        {
            // Expanded list of traditions for offline reference ✨
            Schools.Add(new MagicSchool
            {
                Name = "Wicca",
                Overview = "A modern pagan path embracing the cycles of nature and celebrating the divine in both feminine and masculine forms.",
                CommonPractices = "Observing the eight Sabbats, holding esbat rituals at the full moon, and crafting spells with herbs or candles.",
                WhoIsItFor = "Seekers who enjoy structured circles, reverence for nature, and a balance of tradition with modern sensibilities."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Haitian Vodou",
                Overview = "A vibrant Afro-Caribbean faith honoring ancestral spirits known as lwa. Catholic iconography and African ritual blend to create a powerful community tradition.",
                CommonPractices = "Ceremonies with drumming and dance, possession by the lwa, healing baths, and offerings of food or rum.",
                WhoIsItFor = "Practitioners rooted in Haitian heritage or anyone drawn to spirit-centered rites steeped in history."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hoodoo",
                Overview = "African American rootwork emphasizing practical results using herbs, roots, and biblical psalms.",
                CommonPractices = "Creating mojo hands, candle rituals, spiritual baths, and conjure oils for love or luck.",
                WhoIsItFor = "Those looking for down-to-earth magic closely tied to ancestral wisdom and folk Christianity."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Chaos Magic",
                Overview = "A results-driven approach where belief is a tool and no symbolism is off limits.",
                CommonPractices = "Sigil crafting, gnosis through meditation or trance, and building personal ritual systems.",
                WhoIsItFor = "Freethinkers who enjoy mixing and matching techniques to see what truly works."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hermeticism",
                Overview = "Ancient esoteric philosophy rooted in the writings of Hermes Trismegistus. It weaves together astrology, alchemy, and mystical wisdom.",
                CommonPractices = "Study of the Hermetica, ceremonial magic, contemplation of correspondences, and inner alchemy.",
                WhoIsItFor = "Occultists fascinated by the foundations of Western esoteric thought."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Golden Dawn",
                Overview = "Influential 19th century order that codified many ceremonial magick practices still used today.",
                CommonPractices = "Initiatory grades, complex rituals based on the elements and Qabalah, and deep study of symbolism.",
                WhoIsItFor = "Students seeking structured advancement and historically significant ritual work."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Ceremonial Magick",
                Overview = "An umbrella term for high ritual using precise tools, divine names, and planetary timing to focus intent.",
                CommonPractices = "Casting circles, invoking angels or spirits, and working from classical grimoires.",
                WhoIsItFor = "Practitioners who enjoy formal ritual and detailed correspondences."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Thelema",
                Overview = "Philosophy introduced by Aleister Crowley encouraging the discovery of one\u2019s True Will through magick and self-discipline.",
                CommonPractices = "Rituals invoking Thelemic deities, daily practices like the Lesser Banishing Ritual of the Pentagram, and study of Crowley\u2019s corpus including The Book of the Law.",
                WhoIsItFor = "Magicians seeking a structured yet liberating path focused on personal spiritual sovereignty."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Kabbalah",
                Overview = "Mystical interpretation of Jewish scripture focusing on the Tree of Life and the flow of divine energy.",
                CommonPractices = "Meditation on Hebrew letters, pathworking through the sefirot, and contemplation of sacred texts.",
                WhoIsItFor = "Students of Jewish mysticism or occultists drawn to Qabalistic symbolism."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Jewish Mysticism",
                Overview = "Wider stream of practices including Merkabah meditation and study of works like the Sefer Yetzirah.",
                CommonPractices = "Chanting divine names, letter permutations, and visionary prayer.",
                WhoIsItFor = "Practitioners seeking direct communion with the divine through Hebrew mystical traditions."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hellenism",
                Overview = "Modern revival of ancient Greek religion honoring the Olympian gods and local spirits.",
                CommonPractices = "Ritual offerings, festival observances, and reconstruction of classical hymns and rites.",
                WhoIsItFor = "Devotees of Greek deities and lovers of classical culture."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Orgonite Work",
                Overview = "Based on Wilhelm Reich\u2019s theories of orgone, this path involves crafting resin, metal, and crystal devices to harmonize energy fields.",
                CommonPractices = "Pouring orgonite pyramids or pendants, meditating with orgone accumulators, and gifting orgonite to spaces in need of cleansing.",
                WhoIsItFor = "Energy healers and curious makers who enjoy hands-on experimentation."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Technomancy",
                Overview = "Playful fusion of code and craft where technology becomes a conduit for spells and intention.",
                CommonPractices = "QR sigils, algorithmic divination, and hacking together custom electronic altars.",
                WhoIsItFor = "Geeky magicians eager to explore how the digital world can amplify their workings."
            });

            Schools.Add(new MagicSchool
            {
                Name = "Magical Titles",
                Overview = "Common honorifics used across traditions with tips on choosing one that fits you.",
                CommonPractices = "Read the companion document for a breakdown of witch, magician, adept, and more.",
                WhoIsItFor = "Anyone curious about what to call themselves.",
                InfoPath = "docs/magical_titles.md"
            });
        }
    }
}
