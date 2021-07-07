using GDQScraper.DTOs;

namespace GDQScraper.EventData
{
    public class Bid : IData<BidDTO>
    {
        public IData<BidDTO> Init(BidDTO dto)
        {
            Name = dto.fields.name;
            Id = dto.pk;
            RunId = dto.fields.speedrun;
            Goal = dto.fields.goal.HasValue ? (int)dto.fields.goal.Value : 0;
            Total = dto.fields.total;
            Donations = dto.fields.count;

            return this;
        }

        public string Name { get; set; }
        public int Id { get; set; }
        public int RunId { get; set; }
        public int Goal { get; set; }
        public double Total { get; set; }
        public int Donations { get; set; }
    }
}
