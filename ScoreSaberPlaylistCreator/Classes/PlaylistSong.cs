using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSaberPlaylistCreator.Classes
{
    public class PlaylistSong
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; private set; }
        [JsonProperty(PropertyName = "songName")]
        public string SongName { get; private set; }
        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; private set; }

        public PlaylistSong(string songName, string key, string hash)
        {
            this.SongName = songName;
            this.Key = key;
            this.Hash = hash;
        }
        
    }
}
