using System;

namespace GDQScraper.DTOs
{
    public class DonationDTO : IDTO
    {
        public string model { get; set; }
        public int pk { get; set; }
        public DonationFieldsDTO fields { get; set; }
    }

    public class DonationFieldsDTO
    {
        public int donor { get; set; }
        public int _event { get; set; }
        public string domain { get; set; }
        public string transactionstate { get; set; }
        public string readstate { get; set; }
        public string commentstate { get; set; }
        public float amount { get; set; }
        public string currency { get; set; }
        public DateTime timereceived { get; set; }
        public bool pinned { get; set; }
        public string canonical_url { get; set; }
        public string _public { get; set; }
        public string donor__alias { get; set; }
        public int donor__alias_num { get; set; }
        public string donor__visibility { get; set; }
        public string donor__canonical_url { get; set; }
        public string donor__public { get; set; }
        public string comment { get; set; }
        public string commentlanguage { get; set; }
    }
}
