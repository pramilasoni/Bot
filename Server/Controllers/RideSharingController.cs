using System;
using System.IdentityModel.Tokens;
using System.Web.Http;

namespace Server.Controllers
{
    [RoutePrefix("api/ride-sharing")]
    public class RideSharingController : ApiController
    {
        [Route("price"), HttpGet]
        public IHttpActionResult GetPriceEstimate(string from, string to)
        {
            var estimatedPrice = GetRandomPriceValue(40, 55);
            return Ok(estimatedPrice);
        }

        [Route("book"), HttpPost]
        public IHttpActionResult BookRide(string from, string to)
        {
            return Ok();
        }

        private string GetRandomPriceValue(double minimum, double maximum)
        {
            Random random = new Random();
            var num = random.NextDouble() * (maximum - minimum) + minimum;
            return string.Format("${0:N2}", num);
        }
    }
}
