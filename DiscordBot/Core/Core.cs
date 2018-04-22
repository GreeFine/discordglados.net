using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class DiscordWebRequest
    {
        private DiscordWebRequest() { }

        public const string bot_token_ = "MzgwMzEwOTgwODA4NDA5MDg5.DPMlaA.ejWsbNkaqrIHUPSU8us5wYNd5Uc";
        public const string client_id_ = "380310980808409089";
        public const string client_secret_ = "erJTxd0T2moIeqFykHIdULIrhnq7YJoY";
        public const string base_api_url_ = "https://discordapp.com/api/v6";

        public static string getLoginURL()
        {
            return ("https://discordapp.com/oauth2/authorize?client_id=380310980808409089&scope=bot&permissions=519240");
        }

        public class LoggingHandler : DelegatingHandler
        {
            public LoggingHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Request:");
                Console.WriteLine(request.ToString());
                if (request.Content != null)
                {
                    Console.WriteLine(await request.Content.ReadAsStringAsync());
                }
                Console.WriteLine();

                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                Console.WriteLine("Response:");
                Console.WriteLine(response.ToString());
                if (response.Content != null)
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                Console.WriteLine();

                return response;
            }
        }

        public static async Task<JObject> Post(JObject p_data, string p_endpoint)
        {
            //HttpClient client = new HttpClient();
            HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", bot_token_);

            var content = new StringContent(p_data.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(base_api_url_ + p_endpoint, content);
            //var response = await client.PostAsync(base_api_url_ + p_endpoint, content);
            var result = response.Content.ReadAsStringAsync().Result;
            if (result.Length > 0)
                return JObject.Parse(result);
            else
                return null;
        }

        public static async void Put(JObject p_data, string p_endpoint)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", bot_token_);

            var content = new StringContent(p_data.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(base_api_url_ + p_endpoint, content);
        }

        public static JArray Get(string p_endpoint)
        {
            var html = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(base_api_url_ + p_endpoint);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            request.Headers.Add(HttpRequestHeader.Authorization, "Bot " + bot_token_);
            request.UserAgent = "DiscordBot (http:/smartdeck.ovh:8080/area, 0.4)";
            request.ContentType = "x-www-form-urlencoded";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
                if (html[0] != '[')
                    html = '[' + html + ']';
                return (JArray.Parse(html));
            }
        }

        public static async void Patch(JObject p_data, string p_endpoint)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", bot_token_);
            var content = new StringContent(p_data.ToString(), Encoding.UTF8, "application/json");

            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, base_api_url_ + p_endpoint)
            {
                Content = content
            };
            var returned = await client.SendAsync(request);
        }

        public static async void Delete(string p_endpoint)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", bot_token_);

            var returned = await client.DeleteAsync(base_api_url_ + p_endpoint);
        }
    }
}
