namespace WoW.Server.Shared
{
    /// <summary>
    /// Realm -> Auth.
    /// 
    /// Sent upon a successful connection.
    /// Identifies the connection as a realmserver with it's ip:port connection info.
    /// </summary>
    public class RealmAuth_Registrar
    {
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
