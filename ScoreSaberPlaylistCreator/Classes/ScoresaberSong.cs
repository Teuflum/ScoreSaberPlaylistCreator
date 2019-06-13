using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSaberPlaylistCreator.Classes
{
    public class ScoresaberSong
    {
        public int uid { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string songSubName { get; set; }
        public string songAuthorName { get; set; }
        public string levelAuthorName { get; set; }
        public int bpm { get; set; }
        public string diff { get; set; }
        public string scores { get; set; }
        public int scores_day { get; set; }
        public int ranked { get; set; }
        public double stars { get; set; }
        public string image { get; set; }
    }

    public class ScoresaberSongRootObject
    {
        public List<ScoresaberSong> songs { get; set; }
    }
}
