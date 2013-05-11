namespace MMTD_Client.Domain
{
    public class GuildMember
    {
        public int accountId { get; set; }
        public int guildId { get; set; }
        public byte guildFlags { get; set; }
        public string screenName { get; set; }

        public GuildMember(int guildId, int accountId, byte guildFlags, string screenName = "")
        {
            this.accountId = accountId;
            this.guildId = guildId;
            this.guildFlags = guildFlags;
            this.screenName = screenName;
        }
    }
}
