using Azunt.Utilities.Identifiers;
using DotNetNote.Services.Interfaces;

namespace DotNetNote.Services
{
    public class VendorEmployeeLicenseNumberService : IVendorEmployeeLicenseNumberService
    {
        private readonly List<string> existingLicenseNumbers = new()
        {
            "RN-3001",
            "RN-3002",
            "LPN-9001"
        };

        public string GetLicenseNumberSuggestion()
        {
            return this.GetRecentLicenseNumberSuggestions(1).FirstOrDefault() ?? string.Empty;
        }

        public List<string> GetRecentLicenseNumberSuggestions(int take = 5)
        {
            if (take <= 0)
            {
                take = 5;
            }

            var candidateSuggestions = this.existingLicenseNumbers
                .Select(LicenseNumberUtility.GetNext)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingSuggestionSet = new HashSet<string>(
                this.existingLicenseNumbers,
                StringComparer.OrdinalIgnoreCase);

            return candidateSuggestions
                .Where(x => !existingSuggestionSet.Contains(x))
                .GroupBy(GetLicensePrefix, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .Take(take)
                .ToList();
        }

        private static string GetLicensePrefix(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            int index = value.Length - 1;
            while (index >= 0 && char.IsDigit(value[index]))
            {
                index--;
            }

            return index >= 0 ? value.Substring(0, index + 1) : string.Empty;
        }
    }
}