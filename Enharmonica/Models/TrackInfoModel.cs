using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Enharmonica.Models
{
    public class TrackInfoModel
    {
        public string Status { get; set; } // "success" or "error"
        public AudDResult Result { get; set; } // Contains song details if matches found
    }
    public class AudDResult
    {
        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("album")]
        public string Album { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("song_link")]
        public string SongLink { get; set; }
    }
}
