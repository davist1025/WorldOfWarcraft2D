using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Auth
{
    /// <summary>
    /// Auth -> Client.
    /// </summary>
    public class AuthClient_Realmserver
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }

        // array and list cant be used here for some reason, serializer doesnt recognize it.
    }

    //public struct Auth_Realmserver : INetSerializable
    //{
    //    public string Name;
    //    public string Ip;
    //    public int Port;

    //    public void Deserialize(NetDataReader reader)
    //    {
    //        Name = reader.GetString();
    //        Ip = reader.GetString();
    //        Port = reader.GetInt();
    //    }

    //    public void Serialize(NetDataWriter writer)
    //    {
    //        writer.Put(Name);
    //        writer.Put(Ip);
    //        writer.Put(Port);
    //    }
    //}
}
