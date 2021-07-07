using System;

namespace GDQScraper.DTOs
{
    public class EventDTO : IDTO
    {
        public string model { get; set; }
        public int pk { get; set; }
        public EventFieldsDTO fields { get; set; }
    }

    public class EventFieldsDTO
    {
        public string _short { get; set; }
        public string name { get; set; }
        public string hashtag { get; set; }
        public bool use_one_step_screening { get; set; }
        public string receivername { get; set; }
        public float targetamount { get; set; }
        public float minimumdonation { get; set; }
        public string paypalemail { get; set; }
        public string paypalcurrency { get; set; }
        public DateTime datetime { get; set; }
        public string timezone { get; set; }
        public bool locked { get; set; }
        public bool allow_donations { get; set; }
        public string canonical_url { get; set; }
        public string _public { get; set; }
        public float amount { get; set; }
        public int count { get; set; }
        public float max { get; set; }
        public float avg { get; set; }
        public object[] allowed_prize_countries { get; set; }
        public int[] disallowed_prize_regions { get; set; }
    }
}
