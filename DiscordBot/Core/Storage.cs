using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace DiscordBot
{
    class Storage
    {
        const int size = 1024;
        byte[] buffer = new byte[size];
        public JObject data;

        private FileStream fileStream;
        public Storage(string p_path)
        {
            fileStream = new FileStream(p_path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            if (fileStream.Read(buffer, 0, size) == 0)
                save(new JObject());
            data = get();
        }

        public JObject get()
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, size);
            string fileContents = Encoding.ASCII.GetString(buffer);
            return JObject.Parse(fileContents);
        }

        public void add(string p_key, JToken p_data)
        {
            data[p_key] = p_data;
        }

        public void addAndSave(string p_key, JToken p_data)
        {
            add(p_key, p_data);
            save(data);
        }

        public void save(JObject p_data)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            string data = p_data.ToString();
            fileStream.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
            fileStream.Flush();
        }
    }
}
