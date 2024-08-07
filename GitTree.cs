using System.Text.Json.Serialization;

namespace Infinit
{
    [Serializable]
    public class GitTree
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tree")]
        public GitNode[] Tree { get; set; }

        [JsonPropertyName("truncated")]
        public bool Truncated { get; set; }

        public IEnumerable<GitNode> getTree()
        {
            return Tree;
        }

    }
}
