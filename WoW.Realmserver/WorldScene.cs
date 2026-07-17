using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models;
using WoW.Framework.Logging;
using WoW.Realmserver.Components;
using static WoW.Framework.Utils;

namespace WoW.Realmserver
{
    public class WorldScene : Scene
    {
        public override void Update()
        {
            // verify incoming authserver transfers.
            if (Global.Transfers.TryDequeue(out var session))
            {
                var sessionId = session.Item1;
                var peer = session.Item2;

                using (var ctx = new AuthContext())
                {
                    if (ctx.Accounts.Any(account => account.SessionId.Equals(sessionId)))
                    {
                        var accountData = ctx.Accounts.Where(account => account.SessionId.Equals(sessionId)).Single();

                        PlayerComponent newPlayer = new PlayerComponent(accountData);
                        Entity newPlayerEntity = CreateEntity($"{accountData.Username}({accountData.SessionId})");
                        newPlayerEntity.AddComponent(newPlayer);
                        newPlayerEntity.Tag = (int)ActorType.Networked;
                        peer.Tag = newPlayerEntity;

                        Logger.Print($"Enqeued user ({accountData.Username}) with session id ({accountData.SessionId}) has successfully transfered.", LogEntryType.Debug);

                        // todo: send character list.
                    }
                }
            }
        }
    }
}
