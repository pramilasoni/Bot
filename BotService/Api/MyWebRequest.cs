using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Build.Labs.BotFramework.Api
{
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        {
        }
    }

    public class MyWebRequest
    {
        public WebRequest request;
        private Stream dataStream;
        private static readonly HttpClient client = new HttpClient();
        private string status;

        public String Status
        {
            get { return status; }
            set { status = value; }
        }

        public MyWebRequest(string url)
        {
            // Create a request using a URL that can receive a post.

            request = WebRequest.Create(url);
        }

        public MyWebRequest(string url, string method)
            : this(url)
        {

            if (method.Equals("GET") || method.Equals("POST"))
            {
                // Set the Method property of the request to POST.
                request.Method = method;
            }
            else
            {
                throw new Exception("Invalid Method Type");
            }
        }

        public MyWebRequest(string url, string method, string data1)
            : this(url, method)
        {



            var b = new HttpClient().PostAsync("https://localhost:5050/connect/token",
                new JsonContent(new {password = 'c', grant_type = "password", username = "sitecore\\admin"}));
            GetResponse1();
            // Create POST data and convert it to a byte array.
            //string requestBody = string.Format("{{\"password\":\"{0}\",\"grant_type\":\"{1}\",\"username\":\"{2}\"}}", "b", "b", "sitecore\\admin");

            //string postData = requestBody;
            //byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //// Set the ContentType property of the WebRequest.
            //request.ContentType = "application/x-www-form-urlencoded";

            //// Set the ContentLength property of the WebRequest.
            //request.ContentLength = byteArray.Length;

            // // Get the request stream.
            // dataStream = request.GetRequestStream();

            //// Write the data to the request stream.
            //dataStream.Write(byteArray, 0, byteArray.Length);

            //// Close the Stream object.
            //dataStream.Close();

        }

        public async void GetCart( string accessToken,string expiresIn,string tokenType)
        {
            client.DefaultRequestHeaders.Clear();
            Uri Uri = new Uri("https://localhost:5000/api/Carts('Cart01')?$expand=Lines($expand=CartLineComponents)");
            client.DefaultRequestHeaders.Add("ShopName", "CommerceEngineDefaultStorefront");
            client.DefaultRequestHeaders.Add("ShopperId", "ShopperId");
            client.DefaultRequestHeaders.Add("Language", "en-US");
            client.DefaultRequestHeaders.Add("Currency", "USD");
            client.DefaultRequestHeaders.Add("Environment", "HabitatAuthoring");
            client.DefaultRequestHeaders.Add("GeoLocation", "IpAddress=1.0.0.0");
            client.DefaultRequestHeaders.Add("Authorization", tokenType.ToString() + " " + accessToken.ToString());
            client.DefaultRequestHeaders.Add("Postman-Token", "8a55c6b2-24eb-70bc-21bb-913a310de353");
            var response = await client.GetAsync(Uri);
           

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
              
               
                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;
                dynamic stuff2 = JObject.Parse(responseString);
                response.Dispose();
                
            }

            // Complete the connection.  

        }
        public async void AddCart(string accessToken, string expiresIn, string tokenType)
        {
            var values = new Dictionary<string, string>
            {
                {"cartId", "Cart01"},
                {"itemId", "Habitat_Master|6042185|56042185"},
                {"quantity", "3"}
                

            };
            client.DefaultRequestHeaders.Clear();
            var content = new FormUrlEncodedContent(values);
            client.DefaultRequestHeaders.Accept.Clear();
           
            Uri Uri = new Uri("https://localhost:5000/api/AddCartLine()");
            client.DefaultRequestHeaders.Add("ShopName", "CommerceEngineDefaultStorefront");
            client.DefaultRequestHeaders.Add("ShopperId", "ShopperId");
            client.DefaultRequestHeaders.Add("Language", "en-US");
            client.DefaultRequestHeaders.Add("Currency", "USD");
            client.DefaultRequestHeaders.Add("Environment", "HabitatAuthoring");
            client.DefaultRequestHeaders.Add("GeoLocation", "IpAddress=1.0.0.0");
          
            client.DefaultRequestHeaders.Add("Authorization", tokenType + " " + accessToken);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var response2 = await client.PostAsync(Uri, content);
            //var responseString2 = await response2.Content.ReadAsStringAsync();
            string json = JsonConvert.SerializeObject(values);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Uri, httpContent);
       
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;


                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;
                dynamic stuff2 = JObject.Parse(responseString);
                response.Dispose();

            }


        }
        public async void GetResponse1()
        {

            var values = new Dictionary<string, string>
            {
                {"password", "b"},
                {"grant_type", "password"},
                {"username", "sitecore\\admin"},
                {"client_id", "postman-api"},
                {"scope", "openid EngineAPI postman_api"}
            };

            var content = new FormUrlEncodedContent(values);
            
            var response1 = await client.PostAsync("https://localhost:5050/connect/token", content);
            var responseString = await response1.Content.ReadAsStringAsync();
            dynamic stuff = JObject.Parse(responseString);
            var accessToken = stuff.access_token;
            var expiresIn = stuff.expires_in;
            var tokenType = stuff.token_type;
            AddCart(accessToken.ToString(), expiresIn.ToString(), tokenType.ToString());
            GetCart(accessToken.ToString(), expiresIn.ToString(), tokenType.ToString());



            //string myJson = "{'password': 'b','grant_type':'password','username': 'sitecore\\admin'}";
            //using (var client = new HttpClient())
            //{
            //    var response = await client.PostAsync(
            //        "https://localhost:5050/connect/token",
            //        new StringContent(myJson, Encoding.UTF8, "application/json"));
            //    Console.WriteLine(response);
            //    return response;
            //}

        }


        public string GetResponse()
        {
            // Get the original response.

            WebResponse response = request.GetResponse();

            this.Status = ((HttpWebResponse) response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        public MyWebRequest()
        {


            System.Uri myUri = new System.Uri("https://localhost:5050/connect/token");
            HttpWebRequest webRequest = (HttpWebRequest) HttpWebRequest.Create(myUri);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), webRequest);

        }

        void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest) callbackResult.AsyncState;
            Stream postStream = webRequest.EndGetRequestStream(callbackResult);

            string requestBody = string.Format("{{\"password\":\"{0}\",\"grant_type\":\"{1}\",\"username\":\"{2}\"}}",
                "b", "b", "sitecore\\admin");
            byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);

            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            var a = webRequest.BeginGetResponse(new AsyncCallback(GetResponseStreamCallback), webRequest);
        }

        void GetResponseStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest request = (HttpWebRequest) callbackResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(callbackResult);
            using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
            {
                string result = httpWebStreamReader.ReadToEnd();
                Debug.WriteLine(result);
            }
        }
    }
}

