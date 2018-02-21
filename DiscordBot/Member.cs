using Newtonsoft.Json.Linq;
using System;

namespace DiscordBot
{
    public class Members
    {
        Core Core = Core.instance;
        public string id;
        public string name;
        public string Private_Channel;
        public string sendMessage(string p_msg)
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
}
