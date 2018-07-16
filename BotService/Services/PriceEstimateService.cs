using System.Net.Http;
using System.Threading.Tasks;
using Build.Labs.BotFramework.Api;

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

                MyWebRequest myRequest = new MyWebRequest("https://localhost:5050/connect/token", "POST", "a=value1&b=value2");

                //var response = await client.GetAsync($"{RideSharingApiUrl}?from=Seattle&to={location}");
                //response.EnsureSuccessStatusCode();
                //var price = await response.Content.ReadAsStringAsync();
                //return price.Replace("\"", "");
                return "Your Product has been added";
            }
        }
    }
}
