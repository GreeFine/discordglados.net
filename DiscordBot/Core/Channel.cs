using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Channel
    {
        public string id;
        public string name;
        public Guild guild;
        private Dictionary<string, Message> messages_list;
        public Dictionary<string, Message> Messages_list { get { if (messages_list == null) getMessages(); return messages_list; } set { messages_list = value; } }

        public Channel(JToken p_data, Guild p_guild)
        {
            id = p_data.Value<string>("id");
            name = p_data.Value<string>("name");
            guild = p_guild;
        }

        private void getMessages(int limit = 50)
        {
            var messages = Core.Get("/channels/" + id + "/messages?limit=" + limit.ToString());
            messages_list = new Dictionary<string, Message>();
            foreach (JObject message in messages)
            {
                messages_list.Add(message.Value<string>("id"), new Message(message, this));
            }
        }

        public JArray getInfo()
        {
            return Core.Get("/channels/" + id);
        }

        public Message sendMessage(string p_msg)
        {            
            JObject jo = new JObject();
            jo["content"] = p_msg;

            jo = (Core.Post(jo, "/channels/" + id + "/messages").Result);
            Message message = new Message(jo, this);
            Messages_list.Add(message.id, message);

            return message;
        }

        public void delete(List<string> p_messages_id)
        {
            JObject jo = new JObject();
            jo["messages"] = new JArray(p_messages_id);

            Core.Post(jo, "/channels/" + id + "/messages/bulk-delete").Wait();
        }
    }
}
