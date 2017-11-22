using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using WebSocketSharp;

namespace DiscordBot
{
    public class Gateway
    {
        public static readonly Gateway instance = new Gateway();
        private Gateway() { }

        Core Core = Core.instance;

        private string getPath()
        {
            return Core.Get("/gateway").First.Value<string>("url");
        }

        public string sendMessage(string p_channel_id, string p_msg)
        {
            JObject jo = new JObject();
            jo["content"] = p_msg;

            return (Core.Post(jo, "/channels/" + p_channel_id + "/messages").Result).Value<string>("id");
        }

        public void connect()
        {
            using (var ws = new WebSocket(getPath() + "/?v=6&encoding=json"))
            {
                ws.OnMessage += (sender, e) => messageParse(e.Data);
                ws.Connect();
                JObject empty = new JObject();
                JObject jo = new JObject();
                JObject gateway_identify = new JObject();
                JObject properties = new JObject();

                properties["$os"] = "Windows_10";
                properties["$browser"] = "Area.net";
                properties["$device"] = "Area.net";

                gateway_identify["token"] = "Bot " + Core.bot_token_;
                gateway_identify["properties"] = properties;
                gateway_identify["compress"] = false;
                //gateway_identify["large_threshold"] = 50;
                //                gateway_identify["shard"] = new JObject { 0, 1 };
                gateway_identify["presence"] = empty;

                jo["op"] = 2;
                jo["d"] = gateway_identify;
                ws.Send(jo.ToString());
                while (true)
                {
                    if (Core.heartbeat_interval_ > 0)
                    {
                        jo["op"] = 1;
                        jo["d"] = Core.last_sequence_;
                        ws.Send(jo.ToString());
                        Thread.Sleep(Core.heartbeat_interval_);
                    }
                    else
                        Thread.Sleep(500);
                }
            }
        }

        private void messageParse(string p_msg)
        {
            var obj = JObject.Parse(p_msg);
            if (obj.Value<string>("t") != "PRESENCE_UPDATE")
                Console.WriteLine(obj.ToString());
            if (obj.Value<int>("op") == 10)
                Core.heartbeat_interval_ = obj.Value<JToken>("d").Value<int>("heartbeat_interval");
            else if (obj.Value<int>("op") == 0 && obj.Value<string>("t") == "MESSAGE_CREATE")
                messageEvent(obj);
            else if (obj.Value<int>("op") == 0 && obj.Value<string>("t") == "VOICE_STATE_UPDATE")
                voice_event(obj);
        }

        private void messageEvent(JObject p_messge_event)
        {
            if (p_messge_event.Value<JObject>("d").TryGetValue("mentions", out JToken memtions))
                foreach (JObject mention in memtions)
                    if (mention.TryGetValue("id", out JToken mentioned_id) && mentioned_id.ToString() == Core.client_id_)
                        memtioned_me(p_messge_event);
        }

        private void voice_event(JObject p_voice_event)
        {
            var d = p_voice_event.Value<JObject>("d");
            if (d.TryGetValue("user_id", out JToken user_id))
                if (d.TryGetValue("channel_id", out JToken channel_id))
                    if (d.TryGetValue("guild_id", out JToken guild_id) && guild_id.ToString() == "229633582665170944")
                        //   var user_name = getGuildMemberName(guild_id.ToString(), user_id.ToString());
                        sendMessage("382250514752208897", "<@" + user_id.ToString() + ">" + " Moved to channel : " + "<#" + channel_id + ">");
        }

        private void memtioned_me(JObject p_messge_event)
        {
            var d = p_messge_event.Value<JObject>("d");
            var channel_id = d.Value<string>("channel_id");
            if (d.TryGetValue("content", out JToken content))
            {
                var msg = content.ToString().Replace("<@" + Core.client_id_ + ">", "").Trim().ToLower();
                if (msg == "help")
                    sendMessage(channel_id, "I can't do much for now " + d["author"].Value<string>("username"));
                else if (msg == "info")
                    sendMessage(channel_id, "I am a little bot made by GreeFine nothing fancy just testing " + d["author"].Value<string>("username"));
                else if (msg.IsNullOrEmpty())
                    sendMessage(channel_id, "You asked for me " + d["author"].Value<string>("username") + " ?");
                else
                    sendMessage(channel_id, "I don't understand you " + d["author"].Value<string>("username"));
            }
        }

    }
}
