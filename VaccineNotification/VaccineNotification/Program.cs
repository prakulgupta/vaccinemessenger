namespace VaccineNotification

{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Telegram.Bot;

    class Program
    {
        private const string TelegramToken = ""; // Specify your telegram bot token here.

        static void Main(string[] args)
        {
            var periodTimeSpan = TimeSpan.FromMinutes(23);
            var periodTimeSpan1 = TimeSpan.FromMinutes(2);

            // Configure User List with ChatId and their name as per need.
            var hydDic = new Dictionary<string, string>()
            {
                { "UserName1", "ChatId1" },
            };

            var amritsarDic = new Dictionary<string, string>()
            {
                { "UserName1", "ChatId1" },
            };

            var jbpList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var southeastdelhiList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var southdelhiList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var northdelhiList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var westdelhiList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var keralaList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var bhopalList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var ghaziabadList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            var pathankotList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };
            var taskList = new List<Task>();

            var task1 = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var delayTask = Task.Delay(periodTimeSpan);
                        var currentDate = DateTime.Now;
                        var formatedDate = currentDate.ToString("dd-MM-yyyy");
                        //RunAsyncCalander(formatedDate, "149", southdelhiList, 18, "SouthDelhi").Wait();
                        RunAsyncCalander(formatedDate, "581", hydDic, 18, "Hyderabad").Wait();
                        RunAsyncCalander(formatedDate, "312", bhopalList, 18, "Bhopal").Wait();
                        RunAsyncCalander(formatedDate, "485", amritsarDic, 18, "Amritsar").Wait();
                        RunAsyncCalander(formatedDate, "486", pathankotList, 18, "Pathankot").Wait();
                        RunAsyncCalander(formatedDate, "651", ghaziabadList, 18, "UP Ghaziabad").Wait();
                        //RunAsyncCalander(formatedDate, "144", southeastdelhiList, 18, "SouthEastDelhi").Wait();
                        RunAsyncCalander(formatedDate, "142", westdelhiList, 18, "WestDelhi").Wait();
                        RunAsyncCalander(formatedDate, "140", westdelhiList, 18, "NewDelhi").Wait();
                        RunAsyncCalander(formatedDate, "143", northdelhiList, 18, "NorthWestDelhi").Wait();
                        Console.WriteLine("Task 1 Round completed: " + currentDate.ToString());
                        delayTask.Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });

            var task2 = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var delayTask = Task.Delay(periodTimeSpan1);
                        var currentDate = DateTime.Now;
                        var formatedDate = currentDate.ToString("dd-MM-yyyy");
                        RunAsyncCalander(formatedDate, "315", jbpList, 18, "Jabalpur").Wait();
                        RunAsyncCalander(formatedDate, "303", keralaList, 18, "Kerala Thrissur").Wait();
                        RunAsyncCalander(formatedDate, "308", keralaList, 18, "Kerala Palakkad").Wait();
                        RunAsyncCalander(formatedDate, "307", keralaList, 18, "Kerala Ernakulam").Wait();
                        RunAsyncCalander(formatedDate, "296", keralaList, 18, "Kerala Thiruvananthapuram").Wait();
                        RunAsyncCalander(formatedDate, "295", keralaList, 18, "Kerala Kasargod").Wait();
                        Console.WriteLine("Task 2 Round completed: " + currentDate.ToString());
                        delayTask.Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });

            taskList.Add(task1);
            taskList.Add(task2);
            Task.WhenAll(taskList.ToArray()).Wait();

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
                        var filteredRes = res.centers.Where(x => x.sessions.Where(y => y.min_age_limit == minAge && y.available_capacity > 3).ToList().Count > 0).ToList();

                        if (filteredRes.Count > 0)
                        {
                            var districtName = filteredRes.FirstOrDefault().district_name;
                            var textMessage = filteredRes.Select(x => new { x.pincode, x.name }).Take(20);
                            var messageString = new StringBuilder();
                            messageString.Append($"Vaccines are available | District: {districtName} | Age Limit: {minAge} \n");
                            foreach (var item in textMessage)
                            {
                                var text = $"Hospital: {item.name}. Pincode: {item.pincode} \n";
                                messageString.Append(text);
                            }
                            var teClient = new TelegramBotClient(TelegramToken); //Specifiy the telegram token

                            foreach (var item in sendersList)
                            {
                                await teClient.SendTextMessageAsync(item.Value, messageString.ToString());
                            }

                            Console.WriteLine($"{districtName} Vaccine Available & Users Notified.");
                        }
                        else
                        {
                            Console.WriteLine($"{district}: No Vaccine");
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
