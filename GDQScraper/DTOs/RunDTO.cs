using System;

namespace GDQScraper.DTOs
{
    public class RunDTO : IDTO
    {
        public string model { get; set; }
        public int pk { get; set; }
        public RunFieldsDTO fields { get; set; }
    }

    public class RunFieldsDTO
    {
        public int _event { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public string twitch_name { get; set; }
        public string deprecated_runners { get; set; }
        public string console { get; set; }
        public string commentators { get; set; }
        public string description { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int order { get; set; }
        public string run_time { get; set; }
        public string setup_time { get; set; }
        public bool coop { get; set; }
        public string category { get; set; }
        public int? release_year { get; set; }
        public object giantbomb_id { get; set; }
        public int[] runners { get; set; }
        public string canonical_url { get; set; }
        public string _public { get; set; }
    }
}
