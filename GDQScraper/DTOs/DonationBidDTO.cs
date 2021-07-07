namespace GDQScraper.DTOs
{
    public class DonationBidDTO : IDTO
    {
        public string model { get; set; }
        public int pk { get; set; }
        public DonationBidFieldsDTO fields { get; set; }
    }

    public class DonationBidFieldsDTO
    {
        public int bid { get; set; }
        public int donation { get; set; }
        public float amount { get; set; }
        public string _public { get; set; }
    }
}
