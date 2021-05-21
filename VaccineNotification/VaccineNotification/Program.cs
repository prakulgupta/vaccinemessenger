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
        private static TelegramBotClient teleClient;
        private static Dictionary<string, string> hydDic;
        private static Dictionary<string, string> amritsarDic;
        private static Dictionary<string, string> jbpList;
        private static Dictionary<string, string> southeastdelhiList;
        private static Dictionary<string, string> southdelhiList;
        private static Dictionary<string, string> northdelhiList;
        private static Dictionary<string, string> westdelhiList;
        private static Dictionary<string, string> keralaList;
        private static Dictionary<string, string> bhopalList;
        private static Dictionary<string, string> ghaziabadList;
        private static Dictionary<string, string> pathankotList;
        private static Dictionary<string, string> gautamBuddhNagarList;

        static void Main(string[] args)
        {
            SetTelegramClient();
            
            // Run this method in case you need to send custom message to the list of users
            //SendCustomMessageToUser();
            ManageUsersList();
            
            var periodTimeSpan = TimeSpan.FromMinutes(53);
            var periodTimeSpan1 = TimeSpan.FromMinutes(1);

            var taskList = new List<Task>
            {
                SlowExecutingTasks(periodTimeSpan),
                FastExecutingTasks(periodTimeSpan1)
            };

            Task.WhenAll(taskList.ToArray()).Wait();

            //GetTelegramBotQueries().Wait();
        }

        private static Task SlowExecutingTasks(TimeSpan periodTimeSpan) {
            return Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var delayTask = Task.Delay(periodTimeSpan);
                        var currentDate = DateTime.Now;
                        var formatedDate = currentDate.ToString("dd-MM-yyyy");
                        //RunAsyncCalander(formatedDate, "149", southdelhiList, 18, "SouthDelhi").Wait();
                        RunAsyncCalander(formatedDate, "312", bhopalList, 18, "Bhopal").Wait();
                        RunAsyncCalander(formatedDate, "485", amritsarDic, 18, "Amritsar").Wait();
                        //RunAsyncCalander(formatedDate, "486", pathankotList, 18, "Pathankot").Wait();
                        RunAsyncCalander(formatedDate, "651", ghaziabadList, 18, "UP Ghaziabad").Wait();
                        //RunAsyncCalander(formatedDate, "144", southeastdelhiList, 18, "SouthEastDelhi").Wait();
                        RunAsyncCalander(formatedDate, "142", westdelhiList, 18, "WestDelhi").Wait();
                        RunAsyncCalander(formatedDate, "140", westdelhiList, 18, "NewDelhi").Wait();
                        RunAsyncCalander(formatedDate, "143", northdelhiList, 18, "NorthWestDelhi").Wait();
                        RunAsyncCalander(formatedDate, "296", keralaList, 18, "Kerala Thiruvananthapuram").Wait();
                        RunAsyncCalander(formatedDate, "295", keralaList, 18, "Kerala Kasargod").Wait();
                        RunAsyncCalander(formatedDate, "308", keralaList, 18, "Kerala Palakkad").Wait();
                        RunAsyncCalander(formatedDate, "303", keralaList, 18, "Kerala Thrissur").Wait();
                        RunAsyncCalander(formatedDate, "650", gautamBuddhNagarList, 18, "Gautam Buddh Nagar").Wait();
                        Console.WriteLine("Task 1 Round completed: " + currentDate.ToString());
                        delayTask.Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });
        }

        private static Task FastExecutingTasks(TimeSpan periodTimeSpan1) { 
            return Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var delayTask = Task.Delay(periodTimeSpan1);
                        var currentDate = DateTime.Now;
                        var formatedDate = currentDate.ToString("dd-MM-yyyy");
                        RunAsyncCalander(formatedDate, "581", hydDic, 18, "Hyderabad").Wait();
                        //RunAsyncCalander(formatedDate, "314", keralaList, 18, "Indore").Wait();
                        RunAsyncCalander(formatedDate, "307", keralaList, 18, "Kerala Ernakulam").Wait();
                        RunAsyncCalander(formatedDate, "315", jbpList, 18, "Jabalpur").Wait();
                        Console.WriteLine("Task 2 Round completed: " + currentDate.ToString());
                        delayTask.Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });
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
                    SetCowinClientProperties(client, url);

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

                            foreach (var item in sendersList)
                            {
                                await teleClient.SendTextMessageAsync(item.Value, messageString.ToString());
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

        static void SetTelegramClient()
        {
            teleClient = new TelegramBotClient(TelegramToken);
        }

        static void SetCowinClientProperties(HttpClient client, string url)
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US,en;q=0.9,ar;q=0.8,en-GB;q=0.7"));
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Add("User-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.51");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

        static async void SendCustomMessageToUser()
        {
            var customMessageList = new Dictionary<string, string>()
            {
            { "UserName1", "ChatId1" },
            };

            var message = "Welcome! \nPlease share vaccination city/district.";
            //var message = "You will recieve message if any center in Gautam Buddh Nagar UP has vaccination slot available for 18+ category.\nPlease send stop if you don't require service anymore.";
            if (customMessageList.Count > 0)
            {
                foreach (var item in customMessageList)
                {
                    await teleClient.SendTextMessageAsync(item.Value, message);
                }
            }
        }

        static void ManageUsersList()
        {
            gautamBuddhNagarList = new Dictionary<string, string>()
            {
                { "UserName1", "ChatId1" },
            };


            hydDic = new Dictionary<string, string>()
            {
                { "UserName1", "ChatId1" },
            };

            amritsarDic = new Dictionary<string, string>()
            {
                { "UserName1", "ChatId1" },
            };

            jbpList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            southeastdelhiList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            southdelhiList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            northdelhiList = new Dictionary<string, string>()
             {
                { "garimagupta", "1162669723" }
            };

            westdelhiList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            keralaList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            bhopalList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            ghaziabadList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };

            pathankotList = new Dictionary<string, string>()
             {
                { "UserName1", "ChatId1" },
            };
        }
    }
}
