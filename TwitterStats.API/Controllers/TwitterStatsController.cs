using Microsoft.AspNetCore.Mvc;
using TwitterStats.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterStats.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterStatsController : ControllerBase
    {
        private readonly ITweetStatsService _tweetStatsService;

        public TwitterStatsController(ITweetStatsService tweetStatsService)
        {
            _tweetStatsService = tweetStatsService;
        }

        // GET: api/<TwitterStatsController>
        [HttpGet]
        public IActionResult Get()
        {
            var totalTweets = _tweetStatsService.GetTotalTweets();
            var topTweets = _tweetStatsService.GetTopTweets();

            return Ok(new {TotalTweets = totalTweets, TopTweets = topTweets});
        }
    }
}
