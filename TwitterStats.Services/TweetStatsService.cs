using System.Collections.Concurrent;
using TwitterStats.Models;

namespace TwitterStats.Services
{
    public interface ITweetStatsService
    {
        /// <summary>
        ///     Get total tweets 
        /// </summary>
        /// <returns></returns>
        int GetTotalTweets();

        /// <summary>
        ///     Get top tweets
        /// </summary>
        /// <returns></returns>
        Tweet[] GetTopTweets();

        /// <summary>
        ///     Add a tweet to stats
        /// </summary>
        /// <param name="tweet"></param>
        void AddTweet(Tweet tweet);
    }

    public class TweetStatsService : ITweetStatsService
    {
        private int _totalTweets;
        private readonly ConcurrentQueue<Tweet> _topTweets; // queue to store top tweets
        private readonly ReaderWriterLockSlim _rwLock;      // to implement thread safety

        public TweetStatsService()
        {
            _rwLock = new ReaderWriterLockSlim();
            _totalTweets = 0;
            _topTweets = new ConcurrentQueue<Tweet>();
        }

        /// <summary>
        ///     Get total tweets 
        /// </summary>
        /// <returns></returns>
        public int GetTotalTweets()
        {
            _rwLock.EnterReadLock();
            try
            {
                return _totalTweets;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Get top tweets
        /// </summary>
        /// <returns></returns>
        public Tweet[] GetTopTweets()
        {
            _rwLock.EnterReadLock();
            try
            {
                return _topTweets.ToArray();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Add a tweet to stats
        /// </summary>
        /// <param name="tweet"></param>
        public void AddTweet(Tweet tweet)
        {
            _rwLock.EnterWriteLock();
            try
            {
                // increment total tweets
                _totalTweets ++;

                // add the the tweet to the top tweets queue
                _topTweets.Enqueue(tweet);

                // discard old tweet
                // NOTE: 10 can be configurable value
                if (_topTweets.Count > 10)
                    _topTweets.TryDequeue(out _);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }
    }
}