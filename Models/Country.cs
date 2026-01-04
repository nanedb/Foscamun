using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foscamun2026.Models
{
    public class Country
    {
        public string IsoCode { get; set; } = "";
        public string EnglishName { get; set; } = "";
        public string FrenchName { get; set; } = "";
        public string SpanishName { get; set; } = "";

        public int Warnings { get; set; }

        public string Name => EnglishName;

        public string SmallFlagPath => $"/Resources/24/{IsoCode}.png";

        public string LargeFlagPath => $"/Resources/large/{IsoCode}.png";
    }
}
