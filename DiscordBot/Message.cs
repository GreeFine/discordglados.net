using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Message
    {
        Core Core = Core.instance;

        public string id;
        public string content;
        public string author_id;
        public string author_name;
        public string channel_id;

        public Message addReaction(string p_emoji)
        {
            JObject jo = new JObject();

            Core.Put(jo, "/channels/" + channel_id + "/messages/" + id + "/reactions/" + p_emoji + "/@me");
            return this;
        }
    }
}
