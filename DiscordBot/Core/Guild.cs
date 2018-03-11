using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace DiscordBot
{
    public class Guild
    {
        public readonly string id;
        public readonly string name;
        private Dictionary<string, Channel> channels_list_;
        public Dictionary<string, Channel> Channels_list { get { if (channels_list_ == null) getChannels(); return channels_list_; } set { channels_list_ = value; } }

        private Dictionary<string, Members> members_list_;
        public Dictionary<string, Members> Members_list { get { if (members_list_ == null) getMembers(); return members_list_; } set { members_list_ = value; } }

        public Guild(JObject p_data)
        {
            id = p_data.Value<string>("id");
            name = p_data.Value<string>("name");
        }

        public JArray getInfo()
        {
            return DiscordWebRequest.Get("/guilds/" + id);
        }

        public JArray getEmojis()
        {
            return DiscordWebRequest.Get("/guilds/" + id + "/emojis");
        }

        public void getMembers(int limit = 1)
        {
            if (members_list_ == null)
                members_list_ = new Dictionary<string, Members>();
            var membersGet = DiscordWebRequest.Get("/guilds/" + id + "/members?limit=" + limit.ToString());
            foreach (var member in membersGet)
            {
                var name = member.Value<string>("name");
                if (!members_list_.ContainsKey(name))
                    members_list_[name] = new Members();
                members_list_[name].id = member.Value<string>("id");
                members_list_[name].name = member.Value<string>("name");
            }
        }

        public void getChannels()
        {
            channels_list_ = new Dictionary<string, Channel>();
            var channelsGet = DiscordWebRequest.Get("/guilds/" + id + "/channels");
            foreach (var channel in channelsGet)
            {
                var name = channel.Value<string>("name");
                if (!channels_list_.ContainsKey(name))
                    channels_list_[name] = new Channel(channel, this);
            }
        }

        public List<string> channelsNames(string p_guild_id)
        {
            List<string> names = new List<string>();

            foreach (var channel in Channels_list)
                names.Add(channel.Key);
            return names;
        }

        public Channel containChanel(string p_channel_id)
        {
            foreach (var channel in Channels_list)
                if (channel.Value.id == p_channel_id)
                    return channel.Value;
            return null;
        }
    }
}
