using AREA.API;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Me
    {
        public static readonly Me instance = new Me();
        private Me() { }

        private Dictionary<string, Guilds> guilds_;
        public Dictionary<string, Guilds> Guilds { get { if (guilds_ == null) getGuilds(); return guilds_;  } set { guilds_ = value; } }

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
            guilds_ = new Dictionary<string, Guilds>();
            var guilds_get = Core.Get("/users/@me/guilds");
            foreach (JObject guild in guilds_get)
            {
                var name = guild.Value<string>("name");
                if (!guilds_.ContainsKey(name))
                    guilds_[name] = new Guilds();
                guilds_[name].id = guild.Value<string>("id");
                guilds_[name].name = name;
            }
        }

        public List<string> getGuildsNames()
        {
            List<string> names = new List<string>();
            foreach (var guild in Guilds)
                names.Add(guild.Key);
            return (names);
        }

        public JArray getChannels()
        {
            return Core.Get("/users/@me/channels");
        }

        public void ChangeName(string p_name)
        {
            JObject jo = new JObject();
            jo["username"] = p_name;
            Core.Patch(jo, "/users/@me");
        }

        
    }
}
