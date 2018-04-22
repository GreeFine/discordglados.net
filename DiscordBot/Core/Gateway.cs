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
        Me Me = Me.instance;
        const string path = @"./GateWay.JSON";
        private static Storage storage = new Storage(path);

        public static readonly Gateway instance = new Gateway();
        private Gateway()
        {
            var saved = storage.get();
            if (saved["d"] != null)
            {
                last_sequence_ = saved.Value<int>("last_sequence");
                session_id_ = saved["d"].Value<string>("session_id");
            }
        }

        private int heartbeat_interval_ = 0;
        private int last_sequence_ = 0;
        private int ack_n_ = 0;
        private string session_id_;
        private bool invalid_session_ = false;
        private bool connect_failed_ = false;

        public static bool ShowEvents = true;

        private List<Tuple<string, string, string, bool>> mentioned_responsse = new List<Tuple<string, string, string, bool>>();
        private List<Tuple<string, string, Func<string[], Message, string>, bool>> mentioned_commands = new List<Tuple<string, string, Func<string[], Message, string>, bool>>();

        private string getPath()
        {
            var gatePath = DiscordWebRequest.Get("/gateway");
            if (gatePath != null)
                return gatePath.First.Value<string>("url");
            else
                return null;
        }

        public string sendMessage(string p_channel_id, string p_msg)
        {
            JObject jo = new JObject();
            jo["content"] = p_msg;

            return (DiscordWebRequest.Post(jo, "/channels/" + p_channel_id + "/messages").Result).Value<string>("id");
        }

        public void reconnect(string p_error_msg)
        {
            Console.WriteLine(p_error_msg);
            Console.WriteLine("Trying to reconnect in 5 seconds");
            Thread.Sleep(5000);
            connect();
            return;
        }

        public async void connect()
        {
            var api_path = getPath();
            if (api_path != null)
                using (var ws = new WebSocket(api_path + "/?v=6&encoding=json"))
                {
                    JObject jo = new JObject();
                    jo["op"] = 1;

                    ws.OnMessage += (sender, e) => eventParse(ws, e.Data);
                    ws.OnError += (sender, error) => reconnect(error.Message);
                    ws.OnClose += (sender, closeArg) => reconnect(closeArg.Reason);
                    ws.Connect();
                    while (true)
                    {
                        if (connect_failed_)
                        {
                            Console.WriteLine("Not able to connect closing connection.");
                            ws.Close();
                            Environment.Exit(1);
                        }
                        else if (heartbeat_interval_ > 0)
                        {
                            storage.addAndSave("last_sequence", last_sequence_);

                            jo["d"] = last_sequence_;
                            ws.Send(jo.ToString());
                            Thread.Sleep(heartbeat_interval_);
                        }
                        else
                        {
                            if (--heartbeat_interval_ < -2)
                                return;
                            Thread.Sleep(500);
                        }
                    }
                }
            else
                reconnect("Could no request Gateway path");
        }

        private void eventParse(WebSocket ws, string p_msg)
        {
            var obj = JObject.Parse(p_msg);
            var op = obj.Value<int>("op");
            if (ShowEvents)
                Console.WriteLine(obj);
            switch (op)
            {
                case 11:
                    ++ack_n_;
                    break;
                case 10:
                    heartbeat_interval_ = obj.Value<JToken>("d").Value<int>("heartbeat_interval");
                    resume(ws);
                    break;
                case 9:
                    Console.WriteLine("Invalid sessions...");
                    if (!invalid_session_)
                        identify(ws);
                    else
                        connect_failed_ = true;
                    invalid_session_ = true;
                    break;
                case 0:
                    if (obj.TryGetValue("s", out JToken value))
                        last_sequence_ = (int)value;
                    if (obj.Value<string>("t") == "READY")
                        ready_event(obj);
                    if (obj.Value<string>("t") == "MESSAGE_CREATE")
                        messageEvent(obj);
                    //else if (obj.Value<string>("t") == "VOICE_STATE_UPDATE")
                    //voice_event(obj);
                    break;
                default:
                    break;
            }
        }
        public void identify(WebSocket ws)
        {
            JObject empty = new JObject();
            JObject jo = new JObject();
            JObject gateway_identify = new JObject();
            JObject properties = new JObject();

            properties["$os"] = "Windows_10";
            properties["$browser"] = "Area.net";
            properties["$device"] = "Area.net";

            gateway_identify["token"] = "Bot " + DiscordWebRequest.bot_token_;
            gateway_identify["properties"] = properties;
            gateway_identify["compress"] = false;
            //gateway_identify["large_threshold"] = 50;
            //gateway_identify["shard"] = new JObject { 0, 1 };                
            gateway_identify["presence"] = empty;

            jo["op"] = 2;
            jo["d"] = gateway_identify;
            Thread.Sleep(1500);
            Console.WriteLine("Identiying....");
            ws.Send(jo.ToString());
        }

        public void resume(WebSocket ws)
        {
            JObject jo = new JObject();
            JObject resume = new JObject
            {
                ["token"] = "Bot " + DiscordWebRequest.bot_token_,
                ["session_id"] = session_id_,
                ["seq"] = last_sequence_
            };

            jo["op"] = 6;
            jo["d"] = resume;

            Console.WriteLine("Resuming(" + session_id_ + " : " + last_sequence_ + ")...");
            ws.Send(jo.ToString());
        }

        private void ready_event(JObject obj)
        {
            //TODO PArse other data
            session_id_ = obj["d"].Value<string>("session_id");
            obj["last_sequence"] = 0;
            storage.save(obj);
        }

        private void messageEvent(JObject p_messge_event)
        {
            if (p_messge_event.Value<JObject>("d").TryGetValue("mentions", out JToken memtions))
                foreach (JObject mention in memtions)
                    if (mention.TryGetValue("id", out JToken mentioned_id) && mentioned_id.ToString() == DiscordWebRequest.client_id_)
                        memtioned_me(p_messge_event);
        }

        private void voice_event(JObject p_voice_event)
        {
            var d = p_voice_event.Value<JObject>("d");
            if (d.TryGetValue("user_id", out JToken user_id))
                if (d.TryGetValue("channel_id", out JToken channel_id))
                    if (d.TryGetValue("guild_id", out JToken guild_id) && guild_id.ToString() == "229633582665170944")
                        sendMessage("382250514752208897", "<@" + user_id.ToString() + ">" + " Moved to channel : " + "<#" + channel_id + ">");
        }

        string userTag(string p_user_id)
        {
            return "<@" + p_user_id + ">";
        }

        private void reaction_triger(string p_channel_id, string p_msg, JObject p_d)
        {
            string user_id = p_d["author"].Value<string>("id");
            string[] cmd = p_msg.Split(' ');
            Channel channel = Me.getChannel(p_channel_id);
            foreach (var responnse in mentioned_responsse)
            {
                if ((responnse.Item1 == null || channel.guild.name == responnse.Item1)
                    && cmd[0].ToLower() == responnse.Item2.ToLower())
                {
                    if (!responnse.Item4 || Priviledge.isAdmin(user_id))
                        channel.sendEmbedObject(responnse.Item2, responnse.Item3, userTag(user_id));
                    else
                        channel.sendEmbedObject(responnse.Item2, "Admin priviledges needed.", userTag(user_id));
                    return;
                }
            }
        }

        private void command_triger(string p_channel_id, string p_msg, JObject p_raw)
        {
            string user_id = p_raw["author"].Value<string>("id");
            string[] cmd = p_msg.Trim().Split(' ');
            Channel channel = Me.getChannel(p_channel_id);
            foreach (var command in mentioned_commands)
            {
                if ((command.Item1 == null || channel.guild.name == command.Item1)
                    && cmd[0].ToLower() == command.Item2.ToLower())
                {
                    if (!command.Item4 || Priviledge.isAdmin(user_id))
                        channel.sendEmbedObject(command.Item2, command.Item3(cmd.Skip(1).ToArray(), new Message(p_raw, channel)), userTag(user_id));
                    else
                        channel.sendEmbedObject(command.Item2, "Admin priviledges needed.", userTag(user_id));
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
                var msg = content.ToString().Replace("<@" + DiscordWebRequest.client_id_ + ">", "").Trim();

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

        public void add_mentioned_command(string triger, Func<string[], Message, string> reaction, string guild_name = null, bool isAdmin = false)
        {
            mentioned_commands.Add(new Tuple<string, string, Func<string[], Message, string>, bool>(guild_name, triger.ToLower(), reaction, isAdmin));
        }

    }
}
