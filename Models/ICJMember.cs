using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foscamun2026.Models
{ 
    public class ICJMember
    {
        public int MemberID { get; set; }

        public string? Name { get; set; }

        public string? IsoCode { get; set; }

        public string? Kind { get; set; }

        public int Warnings { get; set; }

        public string NameAndKind => $"{Name} ({Kind})";

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
