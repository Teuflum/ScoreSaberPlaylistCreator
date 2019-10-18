using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSaberPlaylistCreator.Classes
{
    public class BeatsaverSong
    {
        public Metadata metadata { get; set; }
        public Stats stats { get; set; }
        public string description { get; set; }
        public object deletedAt { get; set; }
        public string _id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public Uploader uploader { get; set; }
        public DateTime uploaded { get; set; }
        public string hash { get; set; }
        public string downloadURL { get; set; }
        public string coverURL { get; set; }
    }

    public class Difficulties
    {
        public bool easy { get; set; }
        public bool normal { get; set; }
        public bool hard { get; set; }
        public bool expert { get; set; }
        public bool expertPlus { get; set; }
    }

    public class Metadata
    {
        public Difficulties difficulties { get; set; }
        public List<object> characteristics { get; set; }
        public string songName { get; set; }
        public string songSubName { get; set; }
        public string songAuthorName { get; set; }
        public string levelAuthorName { get; set; }
        public double bpm { get; set; }
    }

    public class Stats
    {
        public int downloads { get; set; }
        public int plays { get; set; }
        public int upVotes { get; set; }
        public int downVotes { get; set; }
        public double rating { get; set; }
        public double heat { get; set; }
    }

    public class Uploader
    {
        public string _id { get; set; }
        public string username { get; set; }
    }
}
