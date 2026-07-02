using System.Collections;

namespace Foscamun.Models
{
    /// <summary>
    /// Custom comparer for ICJ members: sorts first by role (Defense, Plaintiff, Juror), then by last name.
    /// </summary>
    public class ICJMemberComparer : IComparer
    {
        public int Compare(object? x, object? y)
        {
            if (x is not ICJRollCallMember memberX || y is not ICJRollCallMember memberY)
                return 0;

            // Define role priority
            int GetRolePriority(string? kind)
            {
                return kind?.ToLower() switch
                {
                    "defense" => 0,
                    "plaintiff" => 1,
                    "juror" => 2,
                    _ => 3
                };
            }

            int roleComparison = GetRolePriority(memberX.Member.Kind).CompareTo(GetRolePriority(memberY.Member.Kind));

            if (roleComparison != 0)
                return roleComparison;

            // If roles are the same, sort by last name
            string lastNameX = GetLastName(memberX.Member.Name);
            string lastNameY = GetLastName(memberY.Member.Name);

            return string.Compare(lastNameX, lastNameY, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Extracts the last name from a full name.
        /// Assumes format "FirstName LastName" or "FirstName MiddleName LastName".
        /// </summary>
        private static string GetLastName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "";

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : "";
        }
    }
}
