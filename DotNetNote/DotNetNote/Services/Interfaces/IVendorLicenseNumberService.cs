using System.Collections.Generic;

namespace DotNetNote.Services.Interfaces
{
    public interface IVendorLicenseNumberService
    {
        string GetLicenseNumberSuggestion();
        List<string> GetRecentLicenseNumberSuggestions(int take = 5);
    }
}