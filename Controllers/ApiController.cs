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
        static List<ConcurrentDictionary<char, int>> dictList = new List<ConcurrentDictionary<char, int>> ();

        [HttpGet(Name = "GetFrequencies")]
        public async Task<string> Get()
        {

            var task = Task.Run(() => GetResponse());
            task.Wait();



            string results = "";
            foreach(var pair in counter.Where(p => !p.Key.Equals('\n') && !p.Key.Equals(' ')).OrderByDescending(p => p.Value))
                results += "The character " + pair.Key + " appears " + pair.Value + " times.\n";


            return results;
        }

        private async Task GetResponse()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer [YOUR TOKEN HERE]");
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            var task = Task.Run(() => client.GetAsync(url));
            task.Wait();
            HttpResponseMessage response = task.Result;
            response.EnsureSuccessStatusCode();
            GitTree tree = JsonSerializer.Deserialize<GitTree>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(tree.getTree().Count());
            nodes = tree.getTree().ToArray();

            nodes.Where(x => pattern.Match(x.getPath()).Success).ToList().ForEach(x => readFile(x.getPath()));

            foreach (GitNode n in nodes.Where(x => pattern.Match(x.getPath()).Success))
            {
                if (!pattern.Match(n.getPath()).Success)
                {
                    var getTask = Task.Run(() => readFile(n.getPath()));
                    task.Wait();
                }
            }
        }

        private async Task readFile(string path)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer [YOUR TOKEN HERE]");
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            var task = Task.Run(() => client.GetAsync("https://api.github.com/repos/lodash/lodash/contents/" + path));
            task.Wait();
            HttpResponseMessage response = task.Result;
            response.EnsureSuccessStatusCode();

            GitFileItem item = JsonSerializer.Deserialize<GitFileItem>(await response.Content.ReadAsStringAsync());
            item.getTranslatedContent().ToCharArray().ToList().ForEach(x => counter.AddOrUpdate(x, 1, (key, oldValue) => oldValue + 1));

        }
    }
}