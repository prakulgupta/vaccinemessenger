namespace VaccineNotification
{

    public class TelegramChat
    {
        public int update_id { get; set; }

        public TelegramMessage message { get; set; }

        // "update_id": 564037112,
        //"message": {
        //    "message_id": 1,
        //    "from": {
        //        "id": 978665047,
        //        "is_bot": false,
        //        "first_name": "Prakul",
        //        "last_name": "Gupta",
        //        "username": "prakulgupta"
        //    },
        //    "chat": {
        //        "id": 978665047,
        //        "first_name": "Prakul",
        //        "last_name": "Gupta",
        //        "username": "prakulgupta",
        //        "type": "private"
        //    },
        //    "date": 1620138127,
        //    "text": "/start",
        //    "entities": [
        //        {
        //            "offset": 0,
        //            "length": 6,
        //            "type": "bot_command"
        //        }
        //    ]
        //}
    }

    public class TelegramMessage {
        public int message_id { get; set; }
        public Chat chat { get; set; }
        public string date { get; set; }
        public string text { get; set; }
    }

    public class Chat {
        public int id { get; set; }
    }
}
