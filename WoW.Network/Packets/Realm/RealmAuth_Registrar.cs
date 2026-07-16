namespace WoW.Network.Packets.Realm
{
    /// <summary>
    /// Realm -> Auth.
    /// 
    /// Sent upon a successful connection.
    /// Identifies the connection as a realmserver with it's ip:port connection info.
    /// </summary>
    [Obsolete("Unused 7/14")]
    public class RealmAuth_Registrar
    {
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
