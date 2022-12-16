using System.Text.Json;
using TwitterStats.Models;
using TwitterStats.Services;

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
            var tweetStatsService = new TweetStatsService();

            await foreach (var tweet in GetTweets())
            {
                tweetStatsService.AddTweet(tweet);
            }

            var topTweets = tweetStatsService.GetTopTweets();

            Assert.That(topTweets.Count, Is.EqualTo(10));

            var totalTweets = tweetStatsService.GetTotalTweets();
            Assert.That(totalTweets, Is.EqualTo(1171));
        }

        async IAsyncEnumerable<Tweet> GetTweets()
        {
            var reader = new StreamReader(@"C:\Users\KrisPakala\TwitterStream.json");

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var tweet = JsonSerializer.Deserialize<Tweet>(line);
                yield return tweet;
            }
        }
    }
}