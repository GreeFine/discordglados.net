using DiscordBot;

namespace AREA.API
{
    public class Discord
    {
        public Me Me = Me.instance;
        public Users Users = Users.instance;
        public Gateway Gateway = Gateway.instance;
    }
}