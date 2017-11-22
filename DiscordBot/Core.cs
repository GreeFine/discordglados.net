using AREA.API;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Core
    {
        public static readonly Core instance = new Core();
        private Core() { }

        public const string bot_token_ = "MzgwMzEwOTgwODA4NDA5MDg5.DPMlaA.ejWsbNkaqrIHUPSU8us5wYNd5Uc";
        public const string client_id_ = "380310980808409089";
        public const string client_secret_ = "erJTxd0T2moIeqFykHIdULIrhnq7YJoY";
        public const string base_api_url_ = "https://discordapp.com/api/v6";
        public int heartbeat_interval_ = 0;
        public int last_sequence_ = 0;

        public string getLoginURL()
        {
            return ("https://discordapp.com/oauth2/authorize?client_id=380310980808409089&scope=bot&permissions=137612352");
        }


        public async Task<JObject> Post(JObject p_data, string p_endpoint)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", bot_token_);

            var content = new StringContent(p_data.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(base_api_url_ + p_endpoint, content);
            return JObject.Parse(response.Content.ReadAsStringAsync().Result);
        }

        public JArray Get(string p_endpoint)
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
    }
}
