using TwitterStats.Services;

namespace TwitterStats.API.Services
{
    public class TwitterHostService: BackgroundService
    {
        private readonly ITweetStatsService _tweetStatsService;
        private readonly ITwitterClientService _twitterClientService;
        private readonly ILogger<TwitterHostService> _logger;

        public TwitterHostService(ITweetStatsService tweetStatsService, ITwitterClientService twitterClientService,
            ILogger<TwitterHostService> logger)
        {
            _tweetStatsService = tweetStatsService;
            _twitterClientService = twitterClientService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = ProcessTweets(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                if (task.IsCompleted)
                    break;
            }
        }

        /// <summary>
        ///     Process Twitter Sample Stream
        ///     NOTE: For higher volume rate, we can use some these concepts
        ///         (i) Channels
        ///         (ii) Actor services if we are using Service Fabric
        ///         (iii) Azure Service Bus
        ///         (iv) Azure Queue
        ///         (v) Produce/Consumer pattern using ConcurrentQueue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ProcessTweets(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var tweet in _twitterClientService.StreamSampleTweets(cancellationToken))
                {
                    _tweetStatsService.AddTweet(tweet);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
            }
        }
    }
}
