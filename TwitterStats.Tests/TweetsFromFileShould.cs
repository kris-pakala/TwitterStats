using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitterStats.Models;
using TwitterStats.Services;

namespace TwitterStats.Tests
{
    public class Tests
    {
        private ILogger<TwitterClientService>? _logger;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            if (factory != null) _logger = factory.CreateLogger<TwitterClientService>();
        }

        [Test]
        public async Task ShouldStreamTweetsFromFile()
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

        [Test]
        public async Task ShouldStream100Tweets()
        {
            var twitterClientService = new TwitterClientService(_logger);
            var cancellationTokeSource = new CancellationTokenSource();
            var totalTweets = 0;

            // read 10 tweets
            await foreach (var tweet in twitterClientService.StreamSampleTweets(cancellationTokeSource.Token))
            {
                totalTweets++;

                if (totalTweets < 100) continue;

                cancellationTokeSource.Cancel();
                break;
            }

            Assert.That(totalTweets, Is.GreaterThanOrEqualTo(100));

            await Task.Delay(10000);
        }
    }
}