using System.Net.Http;
using System.Threading.Tasks;

namespace Build.Labs.BotFramework.Services
{
    public class PriceEstimateService
    {
        private readonly static HttpClient Client = new HttpClient();
        private const string RideSharingApiUrl = "https://buildbotapi.azurewebsites.net/api/ride-sharing/price";

        public async Task<string> GetRidePriceEstimate(string location)
        {
            using (var client = new HttpClient())
            {
                //var response = await client.GetAsync($"{RideSharingApiUrl}?from=Seattle&to={location}");
                //response.EnsureSuccessStatusCode();
                //var price = await response.Content.ReadAsStringAsync();
                //return price.Replace("\"", "");
                return "1000";
            }
        }
    }
}
