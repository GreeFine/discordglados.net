using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Message
    {
        public readonly string id;
        public readonly string content;
        public readonly string author_id;
        public readonly string author_name;
        public readonly dynamic attachments;
        public readonly dynamic embeds;

        public readonly Channel channel;

        public Message(JObject p_data, Channel p_channel)
        {
            id = p_data.Value<string>("id");
            author_id = p_data["author"].Value<string>("id");
            author_name = p_data["author"].Value<string>("username");
            content = p_data.Value<string>("content");
            attachments = p_data.Value<dynamic>("attachments");
            embeds = p_data.Value<dynamic>("embeds");
            channel = p_channel;
        }

        public Message addReaction(string p_emoji)
        {
            JObject jo = new JObject();

            Core.Put(jo, "/channels/" + channel.id + "/messages/" + id + "/reactions/" + p_emoji + "/@me");
            return this;
        }

        public void delete()
        {
            Core.Delete("/channels/" + channel.id + "/messages/" + id);
        }
    }
}
