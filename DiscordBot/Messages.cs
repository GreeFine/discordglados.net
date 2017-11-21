using AREA.API;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Messages
    {
        public static readonly Messages instance = new Messages();
        private Messages() {}

        Core Core = Core.instance;

        public string create(string p_channel_id, string p_msg)
        {
            JObject jo = new JObject();
            jo["content"] = p_msg;

            return (Core.Post(jo, "/channels/" + p_channel_id + "/messages").Result).Value<string>("id");
        }

        public string createChannel(string p_recipient_id)
        {
            if (!Core.channels_.ContainsKey(p_recipient_id))
            {
                JObject jo = new JObject();
                jo["recipient_id"] = p_recipient_id;
                Core.channels_[p_recipient_id] = (Core.Post(jo, "/users/@me/channels").Result).Value<string>("id");
            }
            return (Core.channels_[p_recipient_id]);
        }
    }
}
