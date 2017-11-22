using AREA.API;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Me
    {
        public static readonly Me instance = new Me();
        private Me() { }

        Core Core = Core.instance;

        public Dictionary<string, Guilds> guilds = new Dictionary<string, Guilds>();

        public JArray getOauth2()
        {
            return Core.Get("/oauth2/applications/@me");
        }

        public JArray getMe()
        {
            return Core.Get("/users/@me");
        }

        public void getGuilds()
        {
            var guilds_get = Core.Get("/users/@me/guilds");
            foreach (JObject guild in guilds_get)
            {
                var name = guild.Value<string>("name");
                if (!guilds.ContainsKey(name))
                    guilds[name] = new Guilds();
                guilds[name].id = guild.Value<string>("id");
                guilds[name].name = name;
            }
        }

        public List<string> getGuildsNames()
        {
            List<string> names = new List<string>();
            foreach (var guild in guilds)
                names.Add(guild.Key);
            return (names);
        }

        public JArray getChannels()
        {
            return Core.Get("/users/@me/channels");
        }
    }
}
