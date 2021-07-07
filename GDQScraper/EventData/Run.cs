using GDQScraper.DTOs;
using System;
using System.Collections.Generic;

namespace GDQScraper.EventData
{
    public class Run : IData<RunDTO>
    {

        public IData<RunDTO> Init(RunDTO dto)
        {
            Name = dto.fields.display_name;
            Id = dto.pk;
            StartTime = dto.fields.starttime;
            EndTime = dto.fields.endtime;
            IncentiveIds = new List<int>();

            return this;
        }

        public string Name { get; set; }
        public int Id { get; set; }
        public List<int> IncentiveIds { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool BidWars { get; set; }

        public double donations;

        public void AddDonation(double amount)
        {
            donations += amount;
        }

        public int Duration()
        {
            return (int)(EndTime - StartTime).TotalMinutes;
        }

        public override string ToString()
        {
            return $"{Name}: ${donations:F2}";
        }

        public string ToString(RunFormat format)
        {
            return ToString() + (format == RunFormat.PerMinute ? $" per minute: ${donations / Duration():F2}" : "");
        }

        public enum RunFormat { Default, PerMinute}
    }
}
