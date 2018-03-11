using AREA.API;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Me
    {
        public static readonly Me instance = new Me();
        private Me() { }

        private Dictionary<string, Guild> guilds_;
        public Dictionary<string, Guild> Guilds { get { if (guilds_ == null) getGuilds(); return guilds_;  } set { guilds_ = value; } }

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
            guilds_ = new Dictionary<string, Guild>();
            var guilds_get = Core.Get("/users/@me/guilds");
            foreach (JObject guild in guilds_get)
            {
                var name = guild.Value<string>("name");
                if (!guilds_.ContainsKey(name))
                    guilds_[name] = new Guild(guild);
            }
        }

        public List<string> getGuildsNames()
        {
            List<string> names = new List<string>();
            foreach (var guild in Guilds)
                names.Add(guild.Key);
            return (names);
        }

        public Channel getChannel(string p_channel_id)
        {
            foreach (var guild in Guilds)
            {
                Channel channel =  guild.Value.containChanel(p_channel_id);
                if (channel != null)
                    return channel;
            }
            return null;
        }

        public JArray getChannels()
        {
            return Core.Get("/users/@me/channels");
        }

        public void changeName(string p_name)
        {
            JObject jo = new JObject();
            jo["username"] = p_name;
            Core.Patch(jo, "/users/@me");
        }

        
    }
}
