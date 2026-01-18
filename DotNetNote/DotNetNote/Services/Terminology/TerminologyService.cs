namespace Azunt.Services.Terminology;

public sealed class TerminologyService : ITerminologyService
{
    private readonly TerminologySettings _settings;

    public TerminologyService(IOptions<TerminologySettings> options)
        => _settings = options.Value;

    public string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return "";
        }

        // 현재 AppMode 기준으로 용어 찾기
        if (_settings.Terminology.TryGetValue(_settings.AppMode, out var dict) &&
            dict.TryGetValue(key, out var value) &&
            !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        // 못 찾으면 key 그대로(안 깨지게)
        return key;
    }
}
