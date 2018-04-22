using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Features
{
    class Someone
    {
        public static string find(Message p_message)
        {
            Random rand = new Random();
            var memberList = p_message.channel.guild.Members_list;

            return "<@" + memberList.ElementAt(rand.Next(0, memberList.Count)).Value.id + ">";
        }
    }
}
