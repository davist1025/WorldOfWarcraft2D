using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    public enum GameObjectType : int
    {
        Player
    }

    public enum LogonCode
    {
        Success,
        Banned,
        Suspended,
        AlreadyOnline,
        NoRecord,
    }

    [Flags]
    public enum NpcTypeFlags
    {
        IsMerchant = 1 << 0,
        IsQuestGiver = 1 << 1,
        CanDialogue = 1 << 2
    }

}
