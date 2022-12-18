using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitterStats.Models;

namespace TwitterStats.Tests
{
    public class StreamTestData
    {
        public static IEnumerable<Tweet> GetStreamTestData(string streamDataFile)
        {
            var reader = new StreamReader(streamDataFile);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var tweet = JsonSerializer.Deserialize<Tweet>(line);
                yield return tweet;
            }
        }
    }
}
