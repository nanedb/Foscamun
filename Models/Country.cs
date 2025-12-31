using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foscamun2026.Models
{
    public class Country
    {
        public string? IsoCode { get; set; }

        public string? EnglishName { get; set; }

        public string? FrenchName { get; set; }

        public string? SpanishName { get; set; }

        public int Warnings { get; set; }

        public string Name
        {   get
            {
                //if (Properties.Settings.Default.Lang == "fr-FR") return FrenchName;
                //else if (Properties.Settings.Default.Lang == "es-ES") return SpanishName;
                //else return EnglishName;
                return EnglishName;
            }
        }

        public string SmallFlagPath
        {
            get
            {
                return $@"/Resources/24/{IsoCode}.png";
            }
        }

        public string LargeFlagPath
        {
            get
            {
                return $@"/Resources/large/{IsoCode}.png";
            }
        }
    }
}
