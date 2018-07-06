
namespace Build.Labs.BotFramework.Models
{
    public class LuisEntities
    {
        public const string CarType = "CarType";
        public const string Location = "location";
        public const string Time = "datetime";
    }

    public class LuisIntents
    {
        public const string GetKindOfVehicles = "GetKindOfVehicles";
        public const string GetPriceEstimate = "GetPriceEstimate";
        public const string Greetings = "Greetings";
        public const string None = "None";
        public const string ScheduleRide = "ScheduleRide";
    }

    public class BotConstants
    {
        public static readonly string[] CarTypes = new string[] { "Sedan", "SUV", "Sports car" };
        public static readonly string[] LuisEntities = new string[] { "CarType", "datetime", "location" };
        public const float QnAScoreThreshold = 0.9f;
        public const string CarImageUrl = "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png";
        public const string CompanyLogoUrl = "https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png";
    }
}
