using System.Text.Json.Serialization;

namespace Infinit
{
    public class GitFileItem
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("sha")]
        public string? Sha { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("html_url")]
        public string? Html_url { get; set; }

        [JsonPropertyName("git_url")]
        public string Git_url { get; set; }

        [JsonPropertyName("download_url")]
        public string Download_url { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("encoding")]
        public string Encoding { get; set; }


        public string getTranslatedContent()
        {
            byte[] data = Convert.FromBase64String(Content);
            return System.Text.Encoding.UTF8.GetString(data);
        }
    }
}
