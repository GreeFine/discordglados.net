using DiscordBot;

namespace AREA.API
{
    public class Discord
    {
        public Me Me = Me.instance;
        public Users Users = Users.instance;
        public Guilds Guilds = Guilds.instance;
        public Gateway Gateway = Gateway.instance;
        public Messages Messages = Messages.instance;
    }
}