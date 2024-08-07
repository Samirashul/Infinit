using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Collections.Concurrent;

namespace Infinit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        static readonly string url = "https://api.github.com/repos/lodash/lodash/git/trees/main?recursive=1";
        static Regex pattern = new Regex (".*\\.[jt]s$");
        static GitNode[] nodes;
        static ConcurrentDictionary<char, int> counter = new ConcurrentDictionary<char, int> ();
        static List<Stat> orderer = new List<Stat>();

        [HttpGet(Name = "GetGitNodes")]
        public async Task<string> Get()
        {
            await GetResponse();

            string results = "";
            foreach(var pair in counter.Where(p => !p.Key.Equals('\n') && !p.Key.Equals(' ')).OrderByDescending(p => p.Value))
                results += "The character " + pair.Key + " appears " + pair.Value + " times.\n";


            return results;
        }

        private async Task GetResponse()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer [TOKEN HERE]");
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            GitTree tree = JsonSerializer.Deserialize<GitTree>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(tree.getTree().Count());
            nodes = tree.getTree().ToArray();

            nodes.Where(x => pattern.Match(x.getPath()).Success).ToList().ForEach(async x => await readFile(x.getPath()));
        }

        private async Task readFile(string path)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer [TOKEN HERE]");
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/lodash/lodash/contents/" + path);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            GitFileItem item = JsonSerializer.Deserialize<GitFileItem>(result);
            item.getTranslatedContent().ToCharArray().ToList().ForEach(async x => await AddCharacter(x));

        }

        private async Task AddCharacter(char c)
        {
            counter.AddOrUpdate(c, 1, (key, oldValue) => oldValue + 1);
        }
    }

    class Stat
    {
        public Stat(char c, int i)
        { 
            this.character = c;
            this.frequency = i;
        }

            public char character;
            public int frequency;
    }
}