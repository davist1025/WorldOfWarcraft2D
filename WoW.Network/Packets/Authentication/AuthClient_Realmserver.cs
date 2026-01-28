using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Network.Objects;

namespace WoW.Network.Packets.Authenticcation
{
    /// <summary>
    /// Auth -> Client.
    /// </summary>
    public class AuthClient_Realm : INetSerializable
    {
        public List<RealmserverMetadataObject> Realmlist;

        public void Deserialize(NetDataReader reader)
        {
            Realmlist = new List<RealmserverMetadataObject>();
            int realmCount = reader.GetInt();

            if (realmCount > 0)
            {
                for (int i = 0; i < realmCount; i++)
                {
                    var name = reader.GetString();
                    var hostname = reader.GetString();
                    var port = reader.GetInt();

                    var realmserver = new RealmserverMetadataObject(name, hostname, port);
                    Realmlist.Add(realmserver);
                }
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Realmlist.Count);

            for (int i = 0; i < Realmlist.Count; i++)
            {
                var realm = Realmlist[i];

                writer.Put(realm.Name);
                writer.Put(realm.Hostname);
                writer.Put(realm.Port);
            }
        }
    }
}
