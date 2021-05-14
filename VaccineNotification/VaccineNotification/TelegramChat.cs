namespace VaccineNotification
{

    public class TelegramChat
    {
        public int update_id { get; set; }

        public TelegramMessage message { get; set; }
    }

    public class TelegramMessage {
        public int message_id { get; set; }
        public Chat chat { get; set; }
        public string date { get; set; }
        public string text { get; set; }
    }

    public class Chat {
        public int id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }
    }
}
