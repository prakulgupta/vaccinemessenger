namespace VaccineNotification

{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Telegram.Bot;

    class Program
    {
        private const string TelegramToken = ""; // Specify your telegram bot token here.
        static void Main(string[] args)
        {
            var periodTimeSpan = TimeSpan.FromHours(1);

            // Creation of list is manual for now
            var hydDic = new Dictionary<string, string>()
            {
                { "name", "chatId" },
            };

            var amritsarDic = new Dictionary<string, string>()
            {
            };

            var jbpList = new Dictionary<string, string>()
             {

            };

            var southeastdelhiList = new Dictionary<string, string>()
             {

            };

            var bhopalList = new Dictionary<string, string>()
             {

            };

            while (true)
            {
                var delayTask = Task.Delay(periodTimeSpan);
                try
                {
                    var currentDate = DateTime.Now;
                    var formatedDate = currentDate.ToString("dd-MM-yyyy");

                    RunAsyncCalander(formatedDate, "581", hydDic, 18, "Hyderabad").Wait();
                    RunAsyncCalander(formatedDate, "315", jbpList, 18, "Jabalpur").Wait();
                    RunAsyncCalander(formatedDate, "144", southeastdelhiList, 18, "SouthEastDelhi").Wait();
                    RunAsyncCalander(formatedDate, "312", bhopalList, 18, "Bhopal").Wait();
                    RunAsyncCalander(formatedDate, "485", amritsarDic, 18, "Amritsar").Wait();
                    //RunAsyncCalander(formatedDate, "55", amritsarDic, 18, "Assam-Morigaon").Wait();

                    Console.WriteLine("Round completed: " + currentDate.ToString());
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }

                delayTask.Wait();
            }


            //GetTelegramBotQueries().Wait();
        }

        static async Task RunAsync(string date, string districtId, Dictionary<string, string> sendersList, int minAge)
        {
            if (sendersList.Count > 0)
            {
                using (var client = new HttpClient())
                {
                    var url = $"https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByDistrict?district_id={districtId}&date={date}";
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        Result res = JsonConvert.DeserializeObject<Result>(data);
                        var filteredRes = res.sessions.Where(x => x.min_age_limit == minAge && x.available_capacity > 0).ToList();

                        if (filteredRes.Count > 0)
                        {
                            var textMessage = filteredRes.Select(x => new { x.pincode, x.name, x.available_capacity, x.vaccine }).Take(10);
                            var teClient = new TelegramBotClient(TelegramToken);

                            foreach (var item in sendersList)
                            {
                                await teClient.SendTextMessageAsync(item.Value, JsonConvert.SerializeObject($"Age Limit{minAge}/n" + textMessage));
                            }

                            Console.WriteLine("Message Sent");
                        }
                        else
                        {
                            Console.WriteLine("No Vaccine");
                        }
                    }
                }
            }
        }

        static async Task RunAsyncCalander(string date, string districtId, Dictionary<string, string> sendersList, int minAge, string district)
        {
            if (sendersList.Count > 0)
            {
                using (var client = new HttpClient())
                {
                    var url = $"https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id={districtId}&date={date}";
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US,en;q=0.9,ar;q=0.8,en-GB;q=0.7"));
                    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    client.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.51");


                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        CalanderResult res = JsonConvert.DeserializeObject<CalanderResult>(data);
                        var filteredRes = res.centers.Where(x => x.sessions.Where(y => y.min_age_limit == minAge && y.available_capacity > 0).ToList().Count > 0).ToList();

                        if (filteredRes.Count > 0)
                        {
                            var districtName = filteredRes.FirstOrDefault().district_name;
                            var textMessage = filteredRes.Select(x => new { x.pincode, x.name }).Take(20);
                            var teClient = new TelegramBotClient(TelegramToken); //Specifiy the telegram token

                            foreach (var item in sendersList)
                            {
                                await teClient.SendTextMessageAsync(item.Value, $"District: {districtName} Age Limit: {minAge} \n" + JsonConvert.SerializeObject(textMessage));
                            }

                            Console.WriteLine("Vaccine Available & Message Sent");
                        }
                        else
                        {
                            Console.WriteLine($"No Vaccine: {district}");
                        }
                    }
                }
            }
        }

        static async Task GetTelegramBotQueries()
        {

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(1620138127);


            using (var client = new HttpClient())
            {
                string url = $"https://api.telegram.org/bot{TelegramToken}/getUpdates";
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    TelegramBotResponse res = JsonConvert.DeserializeObject<TelegramBotResponse>(data);

                    Console.WriteLine("Success");
                }

            }
        }
    }
}
