using AREA.API;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Guilds
    {
        public static readonly Guilds instance = new Guilds();
        private Guilds() { }

        Core Core = Core.instance;

        public List<string> getGuildsNames()
        {
            List<string> names = new List<string>();
            foreach (var guild in Core.Guilds_)
                names.Add(guild.Value<string>("name"));
            return (names);
        }

        public string getGuildId(string p_guild_name)
        {
            foreach (var guild in Core.Guilds_)
                if (guild.Value<string>("name") == p_guild_name)
                    return (guild.Value<string>("id"));
            return ("Invalid");
        }

        public JArray getGuildInfo(string p_guild_id)
        {
            return Core.Get("/guilds/" + p_guild_id);
        }

        public JArray getGuildMembers(string p_guild_id, int limit = 1)
        {
            Core.Guilds_[p_guild_id]["members"] = Core.Get("/guilds/" + p_guild_id + "/members?limit=" + limit.ToString());
            return Core.Guilds_[p_guild_id].Value<JArray>("members");
        }

        public string getGuildMemberName(string p_guild_id, string p_member_id)
        {
            foreach (var m in Core.Guilds_[p_guild_id].Value<JArray>("members"))
            {
                if (m["user"].Value<string>("id") == p_member_id)
                    return (m["user"].Value<string>("nane"));
            }
            return ("Invalid");
        }

        public string getGuildMemberId(string p_guild_id, string name)
        {
            foreach (var m in Core.Guilds_[p_guild_id].Value<JArray>("members"))
            {
                if (m["user"].Value<string>("username") == name)
                    return (m["user"].Value<string>("id"));
            }
            return ("Invalid");
        }

        public JArray getGuildChannels(string p_guild_id)
        {
            Core.Guilds_[p_guild_id]["channels"] = Core.Get("/guilds/" + p_guild_id + "/channels");
            return Core.Guilds_[p_guild_id].Value<JArray>("channels"); ;
        }

        public List<string> getGuildChannelsNames(string p_guild_id)
        {
            List<string> names = new List<string>();

            foreach (var channel in Core.Guilds_[p_guild_id]["channels"])
                names.Add(channel.Value<string>("name"));
            return names;
        }

        public string getGuildChannelId(string p_guild_id, string p_channel_name)
        {
            foreach (var channel in Core.Guilds_[p_guild_id]["channels"])
                if (channel.Value<string>("name") == p_channel_name)
                    return (channel.Value<string>("id"));
            return "Invalid";
        }

        public JArray getGuildChannelInfo(string p_channel_id)
        {
            return Core.Get("/channels/" + p_channel_id);
        }

        public JArray getChannelMessages(string p_channel_id, int limit = 50)
        {
            return Core.Get("/channels/" + p_channel_id + "/messages?limit=" + limit.ToString());
        }

    }
}
