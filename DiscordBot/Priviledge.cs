using Newtonsoft.Json.Linq;
using System.IO;

namespace DiscordBot
{
    class Priviledge
    {
        private static JObject data;
        const string path = @"./Priviledge.JSON";

        public void initStorage()
        {
            if (File.Exists(path))
                data = JObject.Parse(File.ReadAllText(path));
            else
            {
                data = new JObject();
                File.Create(path);
            }
        }

        public void saveStorage()
        {
            File.WriteAllText(path, data.ToString());
        }

        public bool isAdmin(string p_id)
        {
            if (data == null)
                initStorage();
            return (data["Admin"] != null && data["Admin"][p_id] != null);
        }
    }
}
