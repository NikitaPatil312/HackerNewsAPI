using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace HackerNewsNikita.Models
{

    public class StoryModel
    {
        public string title { get; set; }
        public string url { get; set; }

        [JsonProperty("by")]
        public string postedBy { get; set; }

        private string _time;
        public string time { get { return _time; } set {
                _time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(value)).ToUniversalTime().ToString("o");

            } }
        public int score { get; set; }
        [JsonProperty("descendants")]
        public int commentCount { get; set; }


    }
}

