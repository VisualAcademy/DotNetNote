using System.Collections.Generic;

namespace DotNetNote.Services.Interfaces
{
    public interface IEmployeeLicenseNumberService
    {
        string GetLicenseNumberSuggestion();
        List<string> GetRecentLicenseNumberSuggestions(int take = 5);
    }
}