using System.Text.Json.Serialization;

namespace Infinit
{
    [Serializable]
    public class GitNode
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        public string getPath()
        {
            return Path;
        }

    }
}
