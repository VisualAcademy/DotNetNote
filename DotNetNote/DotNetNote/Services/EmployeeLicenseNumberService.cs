using Azunt.Utilities.Identifiers;
using DotNetNote.Services.Interfaces;

namespace DotNetNote.Services
{
    /// <summary>
    /// Employee용 License Number 추천 서비스
    /// </summary>
    public class EmployeeLicenseNumberService : IEmployeeLicenseNumberService
    {
        // DB 대신 메모리 데이터 사용
        private readonly List<string> existingLicenseNumbers = new()
        {
            "RN-100245",
            "RN-100246",
            "LPN-200578",
            "LPN-200579",
            "2026-LN-1234"
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

            var recentLicenseNumbers = this.existingLicenseNumbers
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Take(100)
                .ToList();

            var candidateSuggestions = recentLicenseNumbers
                .Select(LicenseNumberUtility.GetNext)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!candidateSuggestions.Any())
            {
                return new List<string>();
            }

            var existingSuggestionSet = new HashSet<string>(
                this.existingLicenseNumbers,
                StringComparer.OrdinalIgnoreCase);

            var suggestions = candidateSuggestions
                .Where(x => !existingSuggestionSet.Contains(x))
                .GroupBy(GetLicensePrefix, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .Take(take)
                .ToList();

            return suggestions;
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

            return index >= 0
                ? value.Substring(0, index + 1)
                : string.Empty;
        }
    }
}