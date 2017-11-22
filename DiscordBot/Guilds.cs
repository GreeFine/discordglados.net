using AREA.API;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBot
{
    public class Guilds
    {
        Core Core = Core.instance;

        public string id;
        public string name;
        private Dictionary<string, Channels> channels_list_;
        public Dictionary<string, Channels> Channels_list { get { if (channels_list_ == null) getChannels(); return channels_list_; } set { channels_list_ = value; } }
        public class Channels
        {
            Core Core = Core.instance;
            public string id;
            public string name;

            public JArray getInfo()
            {
                return Core.Get("/channels/" + id);
            }
            public JArray getMessages(string p_channel_id, int limit = 50)
            {
                return Core.Get("/channels/" + p_channel_id + "/messages?limit=" + limit.ToString());
            }
            public string send(string p_msg)
            {
                JObject jo = new JObject();
                jo["content"] = p_msg;

                return (Core.Post(jo, "/channels/" + id + "/messages").Result).Value<string>("id");
            }
        }
        private Dictionary<string, Members> members_list_;
        public Dictionary<string, Members> Members_list { get { if (members_list_ == null) getMembers(); return members_list_; } set { members_list_ = value; } }
        public class Members
        {
            Core Core = Core.instance;
            public string id;
            public string name;
            public string Private_Channel;
            public string send(string p_msg)
            {
                JObject jo = new JObject();
                jo["content"] = p_msg;

                return (Core.Post(jo, "/channels/" + id + "/messages").Result).Value<string>("id");
            }

            public void createChannel()
            {
                if (String.IsNullOrEmpty(Private_Channel))
                {
                    JObject jo = new JObject();
                    jo["recipient_id"] = id;
                    Private_Channel = (Core.Post(jo, "/users/@me/channels").Result).Value<string>("id");
                }
            }
        }

        public JArray updateJSON(string filename, JArray json)
        {
            FileStream fs = new FileStream(filename, FileMode.CreateNew);
            using (StreamWriter fsw = new StreamWriter(fs))
                fsw.Write(json);
            return json;
        }

        public JArray getInfo()
        {
            return Core.Get("/guilds/" + id);
        }

        public void getMembers(int limit = 1)
        {
            if (members_list_ == null)
                members_list_ = new Dictionary<string, Members>();
            var membersGet = Core.Get("/guilds/" + id + "/members?limit=" + limit.ToString());
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
            if (channels_list_ == null)
                channels_list_ = new Dictionary<string, Channels>();
            var channelsGet = Core.Get("/guilds/" + id + "/channels");
            foreach (var channel in channelsGet)
            {
                var name = channel.Value<string>("name");
                if (!channels_list_.ContainsKey(name))
                    channels_list_[name] = new Channels();
                channels_list_[name].id = channel.Value<string>("id");
                channels_list_[name].name = name;
            }
        }

        public List<string> channelsNames(string p_guild_id)
        {
            List<string> names = new List<string>();

            foreach (var channel in Channels_list)
                names.Add(channel.Key);
            return names;
        }
    }
}
