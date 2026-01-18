namespace Azunt.Services.Terminology;

public sealed class TerminologySettings
{
    public string AppMode { get; set; } = "VisualAcademy";

    // Terminology[Mode][Key] = Value
    public Dictionary<string, Dictionary<string, string>> Terminology { get; set; } = new();
}
