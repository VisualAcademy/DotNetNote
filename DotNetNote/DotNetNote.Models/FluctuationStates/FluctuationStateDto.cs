using Newtonsoft.Json;

namespace DotNetNote.Models
{
    public class FluctuationStateDto
    {
        [JsonProperty("Text")]
        public string Text { get; set; }
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}
