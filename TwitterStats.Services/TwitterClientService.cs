using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TwitterStats.Models;

namespace TwitterStats.Services
{
    public interface ITwitterClientService
    {
        /// <summary>
        ///     Stream sample tweets
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<Tweet> StreamSampleTweets([EnumeratorCancellation] CancellationToken cancellationToken);
    }

    public class TwitterClientService : ITwitterClientService
    {
        private readonly ILogger<TwitterClientService> _logger;
        private readonly string _twitterApiKey;
        private readonly string _twitterSampleStreamUrl;

        public TwitterClientService(ILogger<TwitterClientService> logger)
        {
            _logger = logger;
            _twitterApiKey = Environment.GetEnvironmentVariable("TWITTER_API_TOKEN") ?? string.Empty;
            _twitterSampleStreamUrl = Environment.GetEnvironmentVariable("TWITTER_SAMPLED_STREAM_URL") ?? string.Empty;
        }

        /// <summary>
        ///     Stream sample tweets
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Tweet> StreamSampleTweets([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // request sample streams
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _twitterApiKey);
            var stream = await httpClient.GetStreamAsync(_twitterSampleStreamUrl, cancellationToken);

            // read the sample tweets
            var reader = new StreamReader(stream);
            while (await reader.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var tweet = JsonSerializer.Deserialize<Tweet>(line);
                yield return tweet;
            }
        }
    }
}
