using GDQScraper.DTOs;
using System;
using System.Collections.Generic;

namespace GDQScraper.EventData
{
    public class Donation : IData<DonationDTO>
    {
        public IData<DonationDTO> Init(DonationDTO dto)
        {
            Id = dto.pk;
            Time = dto.fields.timereceived;
            Amount = dto.fields.amount;
            Comment = !string.IsNullOrEmpty(dto.fields.comment);
            Bids = new List<DonationBid>();

            return this;
        }

        public int Id { get; set; }
        public DateTime Time { get; set; }
        public double Amount { get; set; }
        public bool Comment { get; set; }
        public List<DonationBid> Bids { get; set; }
    }
}
