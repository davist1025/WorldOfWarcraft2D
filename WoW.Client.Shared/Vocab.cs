using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    public enum RaceType
    {
        Human = 0,
        Orc
    }

    [Obsolete("Deprecated GObject type enum.")]
    public enum EntityType : int
    {
        LocalPlayer,
        NetPlayer,
        NPC
    }

    public enum LogonCode
    {
        Success,
        Banned,
        Suspended,
        AlreadyOnline,
        NoRecord,
        InvalidPassword
    }

    [Flags]
    public enum NpcTypeFlags
    {
        IsMerchant = 1 << 0,
        IsQuestGiver = 1 << 1,
        CanDialogue = 1 << 2
    }

}
