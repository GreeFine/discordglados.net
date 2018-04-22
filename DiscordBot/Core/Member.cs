using Newtonsoft.Json.Linq;
using System;

namespace DiscordBot
{
    public class Members
    {
        public string id;
        public string name;
        public string nick;
        public string Private_Channel;
        public JArray roles;

        public Members(JObject obj)
        {
            nick = obj.Value<string>("nick");
            var user = obj["user"];
            name = user.Value<string>("username");
            id = user.Value<string>("id");
            roles = obj.Value<JArray>("roles");
        }

        public string sendMessage(string p_msg)
        {
            JObject jo = new JObject();
            jo["content"] = p_msg;

            return (DiscordWebRequest.Post(jo, "/channels/" + id + "/messages").Result).Value<string>("id");
        }

        public void createChannel()
        {
            if (String.IsNullOrEmpty(Private_Channel))
            {
                JObject jo = new JObject();
                jo["recipient_id"] = id;
                Private_Channel = (DiscordWebRequest.Post(jo, "/users/@me/channels").Result).Value<string>("id");
            }
        }
    }
}
