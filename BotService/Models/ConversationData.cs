using System.Collections.Generic;

namespace Build.Labs.BotFramework.Models
{
    public class ReservationData : Dictionary<string, object>
    {
        private const string CarTypeKey = "CarType";
        private const string LocationKey = "Location";
        private const string PriceKey = "Price";
        private const string TimeKey = "Time";

        public ReservationData()
        {
            this[CarTypeKey] = null;
            this[LocationKey] = null;
            this[PriceKey] = null;
            this[TimeKey] = null;
        }

        public string CarType
        {
            get { return (string)this[CarTypeKey]; }
            set { this[CarTypeKey] = value; }
        }

        public string Time
        {
            get { return (string)this[TimeKey]; }
            set { this[TimeKey] = value; }
        }

        public string Location
        {
            get { return (string)this[LocationKey]; }
            set { this[LocationKey] = value; }
        }

        public string Price
        {
            get { return (string)this[PriceKey]; }
            set { this[PriceKey] = value; }
        }
    }
}
