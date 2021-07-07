using GDQScraper.DTOs;
using GDQScraper.EventData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GDQScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("(1) Get Data, (2) Calc");
            var k = Console.ReadKey().KeyChar;
            Console.WriteLine();

            Console.Write("gdq shortname: ");
            string shortName = Console.ReadLine();

            switch (k)
            {
                case '1':
                    await RetrieveData(shortName);
                    break;
                case '2':
                    Calc(shortName);
                    break;
            }
        }

        static void Calc(string shortName)
        {
            List<Run> runs = JsonSerializer.Deserialize<List<Run>>(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{shortName}-runs.json")));
            List<Donation> donations = JsonSerializer.Deserialize<List<Donation>>(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{shortName}-donations.json")));
            List<Bid> bids = JsonSerializer.Deserialize<List<Bid>>(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{shortName}-bids.json")));
            List<DonationBid> donationBids = JsonSerializer.Deserialize<List<DonationBid>>(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{shortName}-donationbids.json")));

            Run outside = new Run { Name = "Not during event" };

            foreach (Donation donation in donations)
            {
                Run run = runs.FirstOrDefault(r => r.StartTime < donation.Time && donation.Time < r.EndTime) ?? outside;
                run.AddDonation(donation.Amount);
            }
            Console.WriteLine($"Donations outside of known runs: \n{outside}");

            Comparer<Run> compareRuns = Comparer<Run>.Create((r1, r2) => -(int)(r1.donations - r2.donations));
            Comparer<Run> compareRunsPerMinute = Comparer<Run>.Create((r1, r2) => -(int)(r1.donations / r1.Duration() - r2.donations / r2.Duration()));

            Console.WriteLine("\n==========\nDonations during run");
            runs.Sort(compareRuns);
            Console.WriteLine(string.Join("\n", runs.Take(10)));


            Console.WriteLine("\n==========\nDonations during run (per run's minute)");
            runs.Sort(compareRunsPerMinute);
            Console.WriteLine(string.Join("\n", runs.Take(10).Select(run => run.ToString(Run.RunFormat.PerMinute))));

            runs = JsonSerializer.Deserialize<List<Run>>(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{shortName}-runs.json")));

            List<Donation> donationsRebalanceNeeded = new List<Donation>();
            foreach (Donation donation in donations)
            {
                if (donation.Bids.Count > 0)
                { 
                    donationsRebalanceNeeded.Add(donation);
                }

                Run run = runs.FirstOrDefault(r => r.StartTime < donation.Time && donation.Time < r.EndTime) ?? outside;
                run.AddDonation(donation.Amount - donation.Bids.Aggregate(0.0, (a,b) => a + b.Amount));
            }

            Console.WriteLine("\n==========\nDonations during run excluding with bids");
            runs.Sort(compareRuns);
            Console.WriteLine(string.Join("\n", runs.Take(10)));

            Console.WriteLine("\n==========\nDonations during run exclusing with bids (per run's minute)");
            runs.Sort(compareRunsPerMinute);
            Console.WriteLine(string.Join("\n", runs.Take(10).Select(run => run.ToString(Run.RunFormat.PerMinute))));

            List<DonationBid> donationBidsWithNoBid = new List<DonationBid>();
            foreach (Donation donation in donationsRebalanceNeeded)
            {
                foreach (DonationBid dbid in donation.Bids)
                {
                    Bid bid = bids.FirstOrDefault(bs => bs.Id == dbid.BidId);
                    if (bid == null)
                    {
                        donationBidsWithNoBid.Add(dbid);
                        continue;
                    }
                    Run run = runs.FirstOrDefault(r => r.Id == bid.RunId) ?? outside;
                    run.AddDonation(dbid.Amount);
                }
            }

            Console.WriteLine("\n==========\nDonations during run exclusing with bids reasign bids to run");
            runs.Sort(compareRuns);
            Console.WriteLine(string.Join("\n", runs.Take(10)));

            Console.WriteLine("\n==========\nDonations during run exclusing with bids reasign bids to run (per run's minute)");
            runs.Sort(compareRunsPerMinute);
            Console.WriteLine(string.Join("\n", runs.Take(10).Select(run => run.ToString(Run.RunFormat.PerMinute))));


            Console.ReadLine();
        }

        static async Task RetrieveData(string shortName)
        {
            Console.Write("Requery Data?");
            bool requery = false;
            switch(Console.ReadKey().KeyChar)
            {
                case 'y':
                    requery = true;
                    break;
            }

            using HttpClient client = new HttpClient();
            var response = await GetAsync(client, $"https://gamesdonequick.com/tracker/search/?type=event&short={shortName}");

            EventDTO eventDTO = JsonSerializer.Deserialize<List<EventDTO>>(await response.Content.ReadAsStringAsync()).First();
            int id = eventDTO.pk;
            int totalDonations = eventDTO.fields.count;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            Console.WriteLine("Getting Run Data...");
            string runsJson = Path.Combine(desktopPath, $"{shortName}-runs.json");
            List<Run> runs = requery || !File.Exists(runsJson) 
                ? await GetData<Run, RunDTO>(client, $"https://gamesdonequick.com/tracker/search/?type=run&event={id}")
                : JsonSerializer.Deserialize<List<Run>>(File.ReadAllText(runsJson));

            File.WriteAllText(runsJson, JsonSerializer.Serialize(runs));

            Console.WriteLine("Getting Bid Data...");
            string bidsJson = Path.Combine(desktopPath, $"{shortName}-bids.json");
            List<Bid> bids = requery || !File.Exists(bidsJson)
                ? await GetData<Bid, BidDTO>(client, $"https://gamesdonequick.com/tracker/search/?type=bid&event={id}")
                : JsonSerializer.Deserialize<List<Bid>>(File.ReadAllText(bidsJson));

            File.WriteAllText(bidsJson, JsonSerializer.Serialize(bids));

            Console.WriteLine("Getting Donation Data...");
            string donationsJson = Path.Combine(desktopPath, $"{shortName}-donations.json");
            bool reQueriedDontations = requery || !File.Exists(donationsJson);
            List<Donation> donations = reQueriedDontations
                ? await GetData<Donation, DonationDTO>(client, $"https://gamesdonequick.com/tracker/search/?type=donation&event={id}")
                : JsonSerializer.Deserialize<List<Donation>>(File.ReadAllText(donationsJson));
                
            File.WriteAllText(donationsJson, JsonSerializer.Serialize(donations));

            Console.WriteLine("Getting Donation Bid Data...");
            string donationBidsJson = Path.Combine(desktopPath, $"{shortName}-donationbids.json");
            List<DonationBid> donationBids = requery || !File.Exists(donationBidsJson)
                ? await GetData<DonationBid, DonationBidDTO>(client, $"https://gamesdonequick.com/tracker/search/?type=donationbid&event={id}")
                : JsonSerializer.Deserialize<List<DonationBid>>(File.ReadAllText(donationBidsJson));


            File.WriteAllText(donationBidsJson, JsonSerializer.Serialize(donationBids));

            if (reQueriedDontations)
            {
                Console.WriteLine("Assigning Bid Data...");
                List<DonationBid> missingDonos = new List<DonationBid>();
                foreach (DonationBid db in donationBids)
                {
                    Donation donation = donations.FirstOrDefault(d => d.Id == db.DonationId);
                    if (donation != null)
                    {
                        donation.Bids.Add(db);
                    }
                    else
                    {
                        missingDonos.Add(db);
                    }
                }
                File.WriteAllText(donationsJson, JsonSerializer.Serialize(donations));

                double totalMissingDonos = 0;
                foreach (DonationBid db in missingDonos)
                {
                    Console.WriteLine($"Missing donation bid: {db}");
                    totalMissingDonos += db.Amount;
                }

                Console.WriteLine($"Total Missing donos: {totalMissingDonos}");
                missingDonos.Sort((d1, d2) => (int)(100 * (d2.Amount - d1.Amount)));
            }
        }

        static async Task<List<T>> GetData<T, D>(HttpClient client, string url) where T:IData<D>, new() where D:IDTO, new()
        {
            List<T> data = new List<T>();

            while (true)
            {
                HttpResponseMessage response = await GetAsync(client, url + $"&offset={data.Count}");
                List<D> donationBidDs = JsonSerializer.Deserialize<List<D>>(await response.Content.ReadAsStringAsync());
                if (donationBidDs.Count == 0)
                {
                    break;
                }

                foreach (D dto in donationBidDs)
                {
                    data.Add((T)new T().Init(dto));
                }
            }

            return data;
        }

        static DateTime lastSent = DateTime.Now;
        static async Task<HttpResponseMessage> GetAsync(HttpClient client, string url)
        {
            HttpResponseMessage response;

            do
            {
                await Task.Delay((DateTime.Now - lastSent < TimeSpan.FromSeconds(1)) ? (DateTime.Now - lastSent) : TimeSpan.FromMilliseconds(0));
                response = await client.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    DateTime date;
                    TimeSpan delay;
                    if (response.Headers.Contains("Retry-After"))
                    {
                        string val = response.Headers.GetValues("Retry-After").First();
                        int seconds;
                        if (int.TryParse(val, out seconds))
                        {
                            delay = TimeSpan.FromSeconds(seconds + 2);
                        }
                        else if (DateTime.TryParse(val, out date))
                        {
                            delay = (date - DateTime.Now).Add(TimeSpan.FromSeconds(2));
                        }
                        else
                        {
                            delay = TimeSpan.FromMinutes(1);
                        }
                    }
                    else
                    {
                        delay = TimeSpan.FromMinutes(1);
                    }

                    Console.WriteLine($"Retry After: {delay}");
                    await Task.Delay(delay);
                    response = null;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error code {response.StatusCode} (Press the enter key to continue)");
                    Console.ReadLine();
                }
            } while (response == null);

            return response;
        }

        private static void ProgressBar(int current, int total)
        {
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            Console.WriteLine();
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);
            Console.Write($"{current} of {total}");
        }
    }
}
