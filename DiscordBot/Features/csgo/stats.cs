using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.csgo
{
    class stats
    {
        const string steam_apikey = "79CFF2A7AF1A1BD2DE9071D05306B9D5";
        const string base_api_url_ = "http://api.steampowered.com";
        const int message_limit = 2000;

        public static JArray Get(string p_endpoint)
        {
            var html = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(base_api_url_ + p_endpoint);
            request.AutomaticDecompression = DecompressionMethods.GZip;

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

        public static string steamId(string[] p_args)
        {
            if (p_args.Length <= 0)
                return "Invalid Argument";
            var request = Get("/ISteamUser/ResolveVanityURL/v0001/?key=" + steam_apikey + "&vanityurl=" + p_args[0])[0];
            var response = request["response"];
            if (response.Value<int>("success") == 1)
            {
                return response.Value<string>("steamid");
            }
            else
                return "Not found";
        }

        public static string get(string[] p_args)
        {
            string response = "";
            string p_steamId;
            double n;

            if (p_args.Length <= 0)
                return "Invalid arguments";
            string steamName = p_args[0];
            if (steamName.Length != "76561198149643878".Length || !double.TryParse(steamName, out n))
                p_steamId = steamId(new string[] { steamName});
            else
                p_steamId = steamName;
            try
            {
                response = Get("/ISteamUserStats/GetUserStatsForGame/v0002/?appid=730&key=" + steam_apikey + "&steamid=" + p_steamId).ToString().Remove(message_limit - 30);
                response = "```\n" + response + "\n```";
            } catch (Exception e)
            {
                response = e.Message;
            }
            return response;
        }
    }
}
