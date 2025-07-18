using System.Collections.ObjectModel;
using RitualOS.Models;
using RitualOS.Helpers;
using RitualOS.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

// ASCII art cat shape start
//   /_/\  
//  ( o.o ) 
//  > ^ <
namespace RitualOS.ViewModels
{
    /// <summary>
    /// Provides a comprehensive library of magical and occult schools for reference and exploration. 🧙‍♂️
    /// </summary>
    public class MagicSchoolsViewModel : ViewModelBase
    {
        public ObservableCollection<MagicSchool> Schools { get; } = new();
        public ObservableCollection<MagicSchool> FilteredSchools { get; } = new();
        public RelayCommand OpenDocCommand { get; }
        public RelayCommand<string> FilterCommand { get; }

        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged();
                    ApplyFilter();
                    CheckEasterEgg();
                }
            }
        }

        // ASCII art cat shape middle
        //  /| |\
        //  /| | \
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
            FilterCommand = new RelayCommand<string>(keyword =>
            {
                FilterText = keyword ?? string.Empty;
            });
            FilteredSchools = new ObservableCollection<MagicSchool>(Schools); // Initial full list
        }

        private void OpenDoc(string path)
        {
            var full = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (File.Exists(full))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = full,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    LoggingService.Error($"Failed to open document {path}: {ex.Message}");
                }
            }
            else
            {
                LoggingService.Warn($"Document not found at {full}");
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                FilteredSchools.Clear();
                foreach (var school in Schools)
                {
                    FilteredSchools.Add(school);
                }
            }
            else
            {
                FilteredSchools.Clear();
                var lowerFilter = FilterText.ToLowerInvariant();
                foreach (var school in Schools.Where(s => s.Keywords.Any(k => k.ToLowerInvariant().Contains(lowerFilter)) ||
                                                         s.Name.ToLowerInvariant().Contains(lowerFilter)))
                {
                    FilteredSchools.Add(school);
                }
            }
        }

        private void CheckEasterEgg()
        {
            if (FilterText?.ToLowerInvariant() == "cat")
            {
                new RitualOS.Views.CatWindow().Show();
                FilterText = string.Empty; // Reset to hide the trigger
            }
        }

        // ASCII art cat shape end
        //   _/| |_
        //  /  | |  \
        private void LoadSchools()
        {
            // Comprehensive list of traditions with detailed insights ✨
            Schools.Add(new MagicSchool
            {
                Name = "Wicca",
                Overview = "A modern pagan path embracing the cycles of nature and celebrating the divine in both feminine and masculine forms.",
                HistoricalContext = "Emerging in the mid-20th century, Wicca was formalized by Gerald Gardner in the 1950s, drawing from ancient pagan traditions and ceremonial magic.",
                CommonPractices = "Observing the eight Sabbats, holding esbat rituals at the full moon, and crafting spells with herbs or candles.",
                WhoIsItFor = "Seekers who enjoy structured circles, reverence for nature, and a balance of tradition with modern sensibilities.",
                NotableFigures = "Gerald Gardner, Doreen Valiente",
                RecommendedReadings = "‘Wicca: A Guide for the Solitary Practitioner’ by Scott Cunningham",
                Keywords = new[] { "pagan", "nature", "ritual", "wicca" },
                InfoPath = "docs/schools/wicca.md"
            });

            Schools.Add(new MagicSchool
            {
                Name = "Haitian Vodou",
                Overview = "A vibrant Afro-Caribbean faith honoring ancestral spirits known as lwa. Catholic iconography and African ritual blend to create a powerful community tradition.",
                HistoricalContext = "Originating in Haiti during the colonial era, Vodou evolved from West African religions and French Catholicism under enslavement conditions.",
                CommonPractices = "Ceremonies with drumming and dance, possession by the lwa, healing baths, and offerings of food or rum.",
                WhoIsItFor = "Practitioners rooted in Haitian heritage or anyone drawn to spirit-centered rites steeped in history.",
                NotableFigures = "Marie Laveau (influential figure in related New Orleans Voodoo)",
                RecommendedReadings = "‘The Haitian Vodou Handbook’ by Kenaz Filan",
                Keywords = new[] { "vodou", "lwa", "afro-caribbean", "spirit" },
                InfoPath = "docs/schools/vodou.md"
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hoodoo",
                Overview = "African American rootwork emphasizing practical results using herbs, roots, and biblical psalms.",
                HistoricalContext = "Developed by enslaved African Americans, Hoodoo blends African spiritual practices with Southern folk magic and Christian elements.",
                CommonPractices = "Creating mojo hands, candle rituals, spiritual baths, and conjure oils for love or luck.",
                WhoIsItFor = "Those looking for down-to-earth magic closely tied to ancestral wisdom and folk Christianity.",
                NotableFigures = "Aunt Caroline Dye, Dr. Buzzard",
                RecommendedReadings = "‘Hoodoo Herb and Root Magic’ by Catherine Yronwode",
                Keywords = new[] { "hoodoo", "rootwork", "folk", "herbs" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Chaos Magic",
                Overview = "A results-driven approach where belief is a tool and no symbolism is off limits.",
                HistoricalContext = "Emerging in the 1970s with figures like Peter J. Carroll and Austin Osman Spare, it challenges traditional magical frameworks.",
                CommonPractices = "Sigil crafting, gnosis through meditation or trance, and building personal ritual systems.",
                WhoIsItFor = "Freethinkers who enjoy mixing and matching techniques to see what truly works.",
                NotableFigures = "Peter J. Carroll, Austin Osman Spare",
                RecommendedReadings = "‘Liber Null & Psychonaut’ by Peter J. Carroll",
                Keywords = new[] { "chaos", "sigil", "flexible", "modern" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hermeticism",
                Overview = "Ancient esoteric philosophy rooted in the writings of Hermes Trismegistus. It weaves together astrology, alchemy, and mystical wisdom.",
                HistoricalContext = "Traced to Hellenistic Egypt (circa 1st-3rd century CE), it influenced Renaissance occultism and modern esoteric traditions.",
                CommonPractices = "Study of the Hermetica, ceremonial magic, contemplation of correspondences, and inner alchemy.",
                WhoIsItFor = "Occultists fascinated by the foundations of Western esoteric thought.",
                NotableFigures = "Marsilio Ficino, Giordano Bruno",
                RecommendedReadings = "‘The Hermetica’ translated by Brian P. Copenhaver",
                Keywords = new[] { "hermetic", "alchemy", "astrology", "esoteric" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Golden Dawn",
                Overview = "Influential 19th century order that codified many ceremonial magick practices still used today.",
                HistoricalContext = "Founded in 1888 by William Wynn Westcott and Samuel Liddell MacGregor Mathers, it shaped modern occultism.",
                CommonPractices = "Initiatory grades, complex rituals based on the elements and Qabalah, and deep study of symbolism.",
                WhoIsItFor = "Students seeking structured advancement and historically significant ritual work.",
                NotableFigures = "Aleister Crowley, W.B. Yeats",
                RecommendedReadings = "‘The Golden Dawn’ by Israel Regardie",
                Keywords = new[] { "golden dawn", "ceremonial", "qabalah", "initiation" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Ceremonial Magick",
                Overview = "An umbrella term for high ritual using precise tools, divine names, and planetary timing to focus intent.",
                HistoricalContext = "Evolving from Renaissance magic, it was formalized by groups like the Golden Dawn and Thelema.",
                CommonPractices = "Casting circles, invoking angels or spirits, and working from classical grimoires.",
                WhoIsItFor = "Practitioners who enjoy formal ritual and detailed correspondences.",
                NotableFigures = "John Dee, Eliphas Levi",
                RecommendedReadings = "‘The Key of Solomon’",
                Keywords = new[] { "ceremonial", "ritual", "grimoire", "precision" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Thelema",
                Overview = "Philosophy introduced by Aleister Crowley encouraging the discovery of one’s True Will through magick and self-discipline.",
                HistoricalContext = "Founded in 1904 with the reception of ‘The Book of the Law,’ it built on Golden Dawn teachings.",
                CommonPractices = "Rituals invoking Thelemic deities, daily practices like the Lesser Banishing Ritual of the Pentagram, and study of Crowley’s corpus.",
                WhoIsItFor = "Magicians seeking a structured yet liberating path focused on personal spiritual sovereignty.",
                NotableFigures = "Aleister Crowley, Kenneth Anger",
                RecommendedReadings = "‘The Book of the Law’ by Aleister Crowley",
                Keywords = new[] { "thelema", "true will", "crowley", "ceremonial" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Kabbalah",
                Overview = "Mystical interpretation of Jewish scripture focusing on the Tree of Life and the flow of divine energy.",
                HistoricalContext = "Rooted in medieval Jewish mysticism, it gained prominence with the Zohar (13th century).",
                CommonPractices = "Meditation on Hebrew letters, pathworking through the sefirot, and contemplation of sacred texts.",
                WhoIsItFor = "Students of Jewish mysticism or occultists drawn to Qabalistic symbolism.",
                NotableFigures = "Isaac Luria, Moses Cordovero",
                RecommendedReadings = "‘The Zohar’ translated by Gershom Scholem",
                Keywords = new[] { "kabbalah", "sefirot", "jewish", "mysticism" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Jewish Mysticism",
                Overview = "Wider stream of practices including Merkabah meditation and study of works like the Sefer Yetzirah.",
                HistoricalContext = "Traces back to early Jewish esoteric traditions, with Merkabah texts from the 1st century CE.",
                CommonPractices = "Chanting divine names, letter permutations, and visionary prayer.",
                WhoIsItFor = "Practitioners seeking direct communion with the divine through Hebrew mystical traditions.",
                NotableFigures = "Abraham Abulafia",
                RecommendedReadings = "‘Sefer Yetzirah: The Book of Creation’",
                Keywords = new[] { "jewish mysticism", "merkabah", "prayer", "visionary" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Hellenism",
                Overview = "Modern revival of ancient Greek religion honoring the Olympian gods and local spirits.",
                HistoricalContext = "Revived in the 20th century, drawing from archaeological and literary sources of ancient Greece.",
                CommonPractices = "Ritual offerings, festival observances, and reconstruction of classical hymns and rites.",
                WhoIsItFor = "Devotees of Greek deities and lovers of classical culture.",
                NotableFigures = "Sannion, Drew Campbell",
                RecommendedReadings = "‘Hellenic Polytheism: Household Worship’ by Labrys",
                Keywords = new[] { "hellenism", "greek", "polytheism", "offerings" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Orgonite Work",
                Overview = "Based on Wilhelm Reich’s theories of orgone, this path involves crafting resin, metal, and crystal devices to harmonize energy fields.",
                HistoricalContext = "Developed by Wilhelm Reich in the mid-20th century, later popularized in New Age circles.",
                CommonPractices = "Pouring orgonite pyramids or pendants, meditating with orgone accumulators, and gifting orgonite to spaces in need of cleansing.",
                WhoIsItFor = "Energy healers and curious makers who enjoy hands-on experimentation.",
                NotableFigures = "Wilhelm Reich",
                RecommendedReadings = "‘The Orgone Accumulator Handbook’ by James DeMeo",
                Keywords = new[] { "orgonite", "energy", "healing", "craft" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Technomancy",
                Overview = "Playful fusion of code and craft where technology becomes a conduit for spells and intention.",
                HistoricalContext = "Emerging in the digital age, it blends modern tech with magical practice.",
                CommonPractices = "QR sigils, algorithmic divination, and hacking together custom electronic altars.",
                WhoIsItFor = "Geeky magicians eager to explore how the digital world can amplify their workings.",
                NotableFigures = "No specific figures yet; community-driven.",
                RecommendedReadings = "‘Technomancy 101’ (hypothetical online resource)",
                Keywords = new[] { "technomancy", "tech", "sigil", "digital" }
            });

            Schools.Add(new MagicSchool
            {
                Name = "Magical Titles",
                Overview = "Common honorifics used across traditions with tips on choosing one that fits you.",
                HistoricalContext = "Titles like ‘witch’ or ‘adept’ have evolved through centuries of magical practice.",
                CommonPractices = "Read the companion document for a breakdown of witch, magician, adept, and more.",
                WhoIsItFor = "Anyone curious about what to call themselves.",
                InfoPath = "docs/magical_titles.md",
                Keywords = new[] { "titles", "honorifics", "identity", "magical" }
            });

            // New Schools Added
            Schools.Add(new MagicSchool
            {
                Name = "Druidry",
                Overview = "A spiritual tradition rooted in the ancient practices of the Celtic Druids, focusing on nature reverence and seasonal cycles.",
                HistoricalContext = "Revived in the 18th century by figures like Iolo Morganwg, drawing from Celtic mythology and modern ecological awareness.",
                CommonPractices = "Celebrating the eight seasonal festivals (e.g., Samhain, Beltane), tree worship, and meditative communion with nature.",
                WhoIsItFor = "Those who feel a deep connection to the natural world and Celtic heritage.",
                NotableFigures = "Iolo Morganwg, Philip Carr-Gomm",
                RecommendedReadings = "‘The Druidry Handbook’ by John Michael Greer",
                Keywords = new[] { "druidry", "celtic", "nature", "seasonal" },
                InfoPath = "docs/schools/druidry.md"
            });

            Schools.Add(new MagicSchool
            {
                Name = "Shamanism",
                Overview = "An ancient practice involving interaction with the spirit world through trance, journeying, and healing.",
                HistoricalContext = "Found across indigenous cultures worldwide, with roots predating written history, adapted into modern neo-shamanic practices.",
                CommonPractices = "Drumming, soul retrieval, spirit communication, and herbal medicine in ritual contexts.",
                WhoIsItFor = "Individuals seeking direct spiritual experiences and healing through ancestral techniques.",
                NotableFigures = "Michael Harner, Sandra Ingerman",
                RecommendedReadings = "‘The Way of the Shaman’ by Michael Harner",
                Keywords = new[] { "shamanism", "spirit", "trance", "healing" },
                InfoPath = "docs/schools/shamanism.md"
            });

            Schools.Add(new MagicSchool
            {
                Name = "Santería",
                Overview = "A syncretic religion blending Yoruba traditions with Catholicism, honoring orishas (deities) through ritual and community.",
                HistoricalContext = "Developed among enslaved Africans in Cuba during the 19th century, merging with Spanish colonial influences.",
                CommonPractices = "Offerings to orishas, animal sacrifice, dance, and divination with cowrie shells.",
                WhoIsItFor = "People with Afro-Cuban roots or those drawn to vibrant, community-based spiritual practices.",
                NotableFigures = "No single figure; community elders are key.",
                RecommendedReadings = "‘Santería: The Beliefs and Rituals’ by Migene González-Wippler",
                Keywords = new[] { "santería", "orisha", "afro-cuban", "divination" },
                InfoPath = "docs/schools/santeria.md"
            });

            Schools.Add(new MagicSchool
            {
                Name = "Tantra",
                Overview = "A spiritual path integrating meditation, ritual, and physical practices to awaken kundalini energy and achieve enlightenment.",
                HistoricalContext = "Originating in India around the 5th-9th centuries, it spans Hindu and Buddhist traditions with diverse modern interpretations.",
                CommonPractices = "Mantra chanting, yantra meditation, and tantric yoga or sexual rituals in some lineages.",
                WhoIsItFor = "Seekers interested in energy work, transformation, and the union of body and spirit.",
                NotableFigures = "Abhinavagupta, Osho (modern context)",
                RecommendedReadings = "‘Tantra: The Path of Ecstasy’ by Georg Feuerstein",
                Keywords = new[] { "tantra", "kundalini", "meditation", "energy" },
                InfoPath = "docs/schools/tantra.md"
            });

            Schools.Add(new MagicSchool
            {
                Name = "Rosicrucianism",
                Overview = "A mystical and philosophical tradition combining Christian mysticism, alchemy, and esoteric knowledge.",
                HistoricalContext = "Emerged in the early 17th century with the publication of the Rosicrucian Manifestos, influencing modern occult orders.",
                CommonPractices = "Alchemical experiments, meditation on spiritual symbols, and study of esoteric texts.",
                WhoIsItFor = "Those intrigued by the intersection of science, spirituality, and secret societies.",
                NotableFigures = "Christian Rosenkreuz (legendary), Max Heindel",
                RecommendedReadings = "‘The Rosicrucian Cosmo-Conception’ by Max Heindel",
                Keywords = new[] { "rosicrucian", "alchemy", "mysticism", "secret" },
                InfoPath = "docs/schools/rosicrucianism.md"
            });
        }
    }
}