using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foscamun.Models
{ 
    /// <summary>
    /// Represents an ICJ member (advocate or juror) with their country affiliation.
    /// </summary>
    public class ICJMember
    {
        public int MemberID { get; set; }

        public string? Name { get; set; }

        public string? IsoCode { get; set; }

        /// <summary>
        /// Member type: "Advocate" or "Juror".
        /// </summary>
        public string? Kind { get; set; }

        /// <summary>
        /// Number of warnings issued to this member during a session.
        /// </summary>
        public int Warnings { get; set; }

        /// <summary>
        /// Gets the member's name and kind formatted for display.
        /// </summary>
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
