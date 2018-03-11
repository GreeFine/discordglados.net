using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Features
{
    class Cleaner
    {
        public static string channel_remove_text_message(string[] p_chanel_names, Message p_message, int limit = 50)
        {
            int count = 0;
            List<string> messages_id = new List<string>();

            foreach (var message in p_message.channel.Messages_list)
            {
                if (message.Value.embeds.Count == 0 && message.Value.attachments.Count == 0)
                {
                    //message.Value.delete();
                    messages_id.Add(message.Key);
                    ++count;
                }
            }
            if (count > 2)
                p_message.channel.delete(messages_id);
            return "Removed : " + count + " messages.";
        }
    }
}
