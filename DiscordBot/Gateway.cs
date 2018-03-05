using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebSocketSharp;

namespace DiscordBot
{
    public class Gateway
    {
        public static readonly Gateway instance = new Gateway();
        private Gateway() { }

        Me Me = Me.instance;

        public int heartbeat_interval_ = 0;
        public int last_sequence_ = 0;
        private List<Tuple<string, string, string, bool>> mentioned_responsse = new List<Tuple<string, string, string, bool>>();
        private List<Tuple<string, string, Func<string[], string, string>, bool>> mentioned_commands = new List<Tuple<string, string, Func<string[], string, string>, bool>>();

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
                ws.OnMessage += (sender, e) => eventParse(e.Data);
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
                //gateway_identify["shard"] = new JObject { 0, 1 };
                gateway_identify["presence"] = empty;

                jo["op"] = 2;
                jo["d"] = gateway_identify;
                ws.Send(jo.ToString());
                while (true)
                {
                    if (heartbeat_interval_ > 0)
                    {
                        jo["op"] = 1;
                        jo["d"] = last_sequence_;
                        ws.Send(jo.ToString());
                        Thread.Sleep(heartbeat_interval_);
                    }
                    else
                        Thread.Sleep(500);
                }
            }
        }

        private void eventParse(string p_msg)
        {
            var obj = JObject.Parse(p_msg);
            if (obj.Value<int>("op") == 11)
                return;
            if (obj.Value<string>("t") != "PRESENCE_UPDATE")
                Console.WriteLine(obj.ToString());
            if (obj.Value<int>("op") == 10)
                heartbeat_interval_ = obj.Value<JToken>("d").Value<int>("heartbeat_interval");
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
                        ;
                        //   var user_name = getGuildMemberName(guild_id.ToString(), user_id.ToString());
                        //sendMessage("382250514752208897", "<@" + user_id.ToString() + ">" + " Moved to channel : " + "<#" + channel_id + ">");
        }

        private Priviledge priviledge = new Priviledge();

        private void reaction_triger(string p_channel_id, string p_msg, JObject p_d)
        {
            string user_id = p_d["author"].Value<string>("id");
            string[] cmd = p_msg.Split(' ');
            foreach (var responnse in mentioned_responsse)
            {
                if ((responnse.Item1 == null || Me.Guilds[responnse.Item1].containChanel(p_channel_id))
                    && cmd[0].ToLower() == responnse.Item2.ToLower())
                {
                    if (responnse.Item4 && priviledge.isAdmin(user_id))
                        sendMessage(p_channel_id, "<@" + user_id + "> " + responnse.Item3);
                    else
                        sendMessage(p_channel_id, "<@" + user_id + "> Admin priviledges needed.");
                    return;
                }
            }
        }

        private void command_triger(string p_channel_id, string p_msg, JObject p_d)
        {
            string user_id = p_d["author"].Value<string>("id");
            string[] cmd = p_msg.Trim().Split(' ');
            foreach (var command in mentioned_commands)
            {
                if ((command.Item1 == null || Me.Guilds[command.Item1].containChanel(p_channel_id))
                    && cmd[0].ToLower() == command.Item2.ToLower())
                {
                    if (!command.Item4 || priviledge.isAdmin(user_id))
                        sendMessage(p_channel_id, "<@" + user_id + "> " + command.Item3(cmd.Skip(1).ToArray(), user_id));
                    else
                        sendMessage(p_channel_id, "<@" + user_id + "> Admin priviledges needed.");
                    return;
                }
            }
        }

        private void memtioned_me(JObject p_messge_event)
        {
            var d = p_messge_event.Value<JObject>("d");
            var channel_id = d.Value<string>("channel_id");

            if (d.TryGetValue("content", out JToken content))
            {
                var msg = content.ToString().Replace("<@" + Core.client_id_ + ">", "").Trim();

                reaction_triger(channel_id, msg, d);
                command_triger(channel_id, msg, d);
            }
            else
                sendMessage(channel_id, "I don't understand you " + d["author"].Value<string>("username"));
        }

        public void add_mentioned_reaction(string triger, string reaction, string guild_name = null, bool isAdmin = false)
        {
            mentioned_responsse.Add(new Tuple<string, string, string, bool>(guild_name, triger.ToLower(), reaction, isAdmin));
        }

        public void add_mentioned_command(string triger, Func<string[], string, string> reaction, string guild_name = null, bool isAdmin = false)
        {
            mentioned_commands.Add(new Tuple<string, string, Func<string[], string, string>, bool>(guild_name, triger.ToLower(), reaction, isAdmin));
        }

    }
}
