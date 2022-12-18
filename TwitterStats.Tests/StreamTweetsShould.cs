using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitterStats.Models;
using TwitterStats.Services;

namespace TwitterStats.Tests
{
    public class StreamTweetsShould
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
        [TestCaseSource(typeof(StreamTestData), nameof(TwitterStats.Tests.StreamTestData.GetStreamTestData), new object[]{"StreamTestData.txt"})]
        public void AddTweet(Tweet tweet)
        {
            var tweetStatsService = new TweetStatsService();

            tweetStatsService.AddTweet(tweet);
        }

        [Test]
        public void ReturnTweetStats()
        {
            var tweetStatsService = new TweetStatsService();

            foreach (var tweet in StreamTestData.GetStreamTestData("StreamTestData.txt"))
            {
                tweetStatsService.AddTweet(tweet);
            }

            var topTweets = tweetStatsService.GetTopTweets();

            Assert.That(topTweets.Count, Is.EqualTo(10));

            var totalTweets = tweetStatsService.GetTotalTweets();
            Assert.That(totalTweets, Is.EqualTo(20));
        }

        [Test]
        public async Task Read100TweetsFromTwitter()
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
        }
    }
}