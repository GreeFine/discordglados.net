using System;
using System.Collections.Generic;
using System.Threading;

namespace DiscordBot.Features
{
    class Cleaner
    {
        public static string channel_remove_text_message(string[] p_params, Message p_message)
        {
            uint max = 50;
            bool me = false;
            List<string> messages_id = new List<string>();
            List<Message> messages_id_old = new List<Message>();

            if (p_params.Length > 0)
            {
                if (p_params[0] == "me")
                    me = true;
                else if (uint.TryParse(p_params[0], out max) && max > 100 && max != 0)
                    return "Delete number between 1 and 100";
            }
            foreach (var message in p_message.channel.Messages_list)
            {
                if (max-- == 0)
                    break;
                if (message.Value.embeds.Count == 0 && message.Value.attachments.Count == 0 &&
                    (!me || message.Value.author_id == p_message.author_id))
                {
                    TimeSpan ts = DateTime.Now - message.Value.timestamp;
                    if (ts.TotalDays > 14)
                        messages_id_old.Add(message.Value);
                    else
                        messages_id.Add(message.Key);
                }
            }
            if (messages_id.Count > 2)
                p_message.channel.delete(messages_id);
            foreach (var msg in messages_id_old)
            {
                msg.delete();
                Thread.Sleep(200);
            }
            return "Removed : " + messages_id.Count + " messages and " + messages_id_old.Count + " old messages.";
        }
    }
}
