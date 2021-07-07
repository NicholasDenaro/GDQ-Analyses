using System;

namespace GDQScraper.DTOs
{
    public class BidDTO : IDTO
    {
        public string model { get; set; }
        public int pk { get; set; }
        public BidFieldsDTO fields { get; set; }
    }

    public class BidFieldsDTO
    {
        public int _event { get; set; }
        public int speedrun { get; set; }
        public object parent { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string description { get; set; }
        public string shortdescription { get; set; }
        public float? goal { get; set; }
        public bool istarget { get; set; }
        public bool allowuseroptions { get; set; }
        public int? option_max_length { get; set; }
        public DateTime revealedtime { get; set; }
        public object biddependency { get; set; }
        public float total { get; set; }
        public int count { get; set; }
        public bool pinned { get; set; }
        public string canonical_url { get; set; }
        public string _public { get; set; }
        public string speedrun__name { get; set; }
        public string speedrun__display_name { get; set; }
        public string speedrun__twitch_name { get; set; }
        public DateTime speedrun__starttime { get; set; }
        public DateTime speedrun__endtime { get; set; }
        public int speedrun__order { get; set; }
        public string speedrun__canonical_url { get; set; }
        public string speedrun__public { get; set; }
        public string event__short { get; set; }
        public string event__name { get; set; }
        public DateTime event__datetime { get; set; }
        public string event__timezone { get; set; }
        public string event__canonical_url { get; set; }
        public string event__public { get; set; }
    }
}
