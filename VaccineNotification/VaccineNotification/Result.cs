namespace VaccineNotification
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Result
    {
        public IList<AppointmentDetails> sessions { get; set; }
    }

    public class CalanderResult
    {
        public IList<Center> centers { get; set; }
    }

    public class Center
    {

        public int center_id { get; set; }
        public string name { get; set; }

        public string district_name { get; set; }
        public string address { get; set; }

        public string pincode { get; set; }
        public string fee_type { get; set; }

        public IList<Session> sessions { get; set; }
    }

    public class Session
    {
        public string session_id { get; set; }
        public string date { get; set; }

        public decimal available_capacity { get; set; }

        public int min_age_limit { get; set; }

        public string vaccine { get; set; }

        public string[] slots { get; set; }
    }
}
