using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Channels
    {
        Core Core = Core.instance;

        public string id;
        public string name;
        List<Message> Messages = new List<Message>();

        public JArray getInfo()
        {
            return Core.Get("/channels/" + id);
        }

        public JArray getMessages(string p_channel_id, int limit = 50)
        {
            return Core.Get("/channels/" + p_channel_id + "/messages?limit=" + limit.ToString());
        }

        public Message sendMessage(string p_msg)
        {
            Message message = new Message();
            JObject jo = new JObject();
            jo["content"] = p_msg;

            jo = (Core.Post(jo, "/channels/" + id + "/messages").Result);
            message.id = jo.Value<string>("id");
            message.author_id = jo["author"].Value<string>("id");
            message.author_name = jo["author"].Value<string>("username");
            message.content = jo.Value<string>("content");
            message.channel_id = jo.Value<string>("channel_id");
            Messages.Add(message);

            return message;
        }
    }
}
