using AREA.API;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Me
    {
        public static readonly Me instance = new Me();
        private Me() { }

        Core Core = Core.instance;

        public JArray getOauth2()
        {
            return Core.Get("/oauth2/applications/@me");
        }

        public JArray getMe()
        {
            return Core.Get("/users/@me");
        }

        public JArray getGuilds()
        {
            var guilds_get = Core.Get("/users/@me/guilds");
            foreach (JObject guild_get in guilds_get)
            {
                foreach (JObject guild_stored in Core.Guilds_)
                    guild.info_ = g;
            }
            return Core.Guilds_;
        }

        public JArray getChannels()
        {
            return Core.Get("/users/@me/channels");
        }
    }
}
