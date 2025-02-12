using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    /// <summary>
    /// Realm -> Client
    /// 
    /// Sent to all clients when someone sends a chat message.
    /// This packet is also used for whispering, group chatting, etc.
    /// </summary>
    public class RealmClient_Chat : INetSerializable
    {
        public ChatChannelType Channel = ChatChannelType.Say;
        public Dictionary<ChatMessageParameter, string> Parameters = new Dictionary<ChatMessageParameter, string>();
        public string Message;

        public void Deserialize(NetDataReader reader)
        {
            Parameters.Clear();

            Channel = (ChatChannelType)reader.GetInt();
            int parameterCount = reader.GetInt();

            for (int i = 0; i < parameterCount; i++)
            {
                var kv = reader.GetString().Split(':');
                var key = kv[0];
                var val = kv[1];

                var parameter = Enum.Parse<ChatMessageParameter>(key);
                Parameters.TryAdd(parameter, val);
            }
            Message = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((int)Channel);
            writer.Put(Parameters.Count);
            foreach (var kv in Parameters)
                writer.Put($"{kv.Key.ToString()}:{kv.Value}");
            writer.Put(Message);
        }

        public void AddParameter(ChatMessageParameter parameter, string value)
        {
            Parameters.TryAdd(parameter, value);
        }
    }
}
