using GDQScraper.DTOs;

namespace GDQScraper.EventData
{
    public class DonationBid : IData<DonationBidDTO>
    {
        public IData<DonationBidDTO> Init(DonationBidDTO dto)
        {
            Id = dto.pk;
            BidId = dto.fields.bid;
            DonationId = dto.fields.donation;
            Amount = dto.fields.amount;

            return this;
        }

        public int Id { get; set; }
        public int BidId { get; set; }
        public int DonationId { get; set; }
        public double Amount { get; set; }

        public override string ToString()
        {
            return $"{Id} bid {BidId} donation {DonationId} amount ${Amount:F2}";
        }
    }
}
