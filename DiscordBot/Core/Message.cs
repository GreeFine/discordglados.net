using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace DiscordBot
{
    public class Message
    {
        public readonly string id;
        public readonly string author_id;
        public readonly string author_name;
        public readonly DateTime timestamp;
        public readonly string content;
        public readonly dynamic attachments;
        public readonly dynamic embeds;

        public readonly Channel channel;

        private CultureInfo inputCulture = CultureInfo.CreateSpecificCulture("en-us");

        public Message(JObject p_data, Channel p_channel)
        {
            id = p_data.Value<string>("id");
            author_id = p_data["author"].Value<string>("id");
            author_name = p_data["author"].Value<string>("username");
            string date = p_data.Value<string>("timestamp");
            timestamp = DateTime.Parse(date, inputCulture);
            content = p_data.Value<string>("content");
            attachments = p_data.Value<dynamic>("attachments");
            embeds = p_data.Value<dynamic>("embeds");
            channel = p_channel;
        }

        public Message addReaction(string p_emoji)
        {
            JObject jo = new JObject();

            DiscordWebRequest.Put(jo, "/channels/" + channel.id + "/messages/" + id + "/reactions/" + p_emoji + "/@me");
            return this;
        }

        public void delete()
        {
            DiscordWebRequest.Delete("/channels/" + channel.id + "/messages/" + id);
        }
    }
}
