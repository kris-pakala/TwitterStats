using System.Text.Json;
using TwitterStats.Models;

namespace TwitterStats.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task StreamTweetsFromFile()
        {
            await foreach (var twt in GetTweets())
            {
                Console.WriteLine(twt.data.text);
            }

            Assert.Pass();
        }

        async IAsyncEnumerable<Tweet> GetTweets()
        {
            var reader = new StreamReader(@"C:\Users\KrisPakala\TwitterStream_2.json");

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var tweet = JsonSerializer.Deserialize<Tweet>(line);
                yield return tweet;
            }
        }
    }
}