using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    public enum GameObjectType : int
    {
        Player,
        Creature,
        Interactable
    }

    [Flags]
    public enum GameObjectFlags : int
    {
        None = 0, // should never contain this flag. used to init the object.
        IsTargetable = 1,
        IsContainer = 1 << 1,
        IsMailbox = 1 << 2,
    }

    public enum LogonCode
    {
        Success,
        Banned,
        Suspended,
        AlreadyOnline,
        NoRecord,
    }

    public enum CharacterClassType
    {
        Warrior = 1,
        Mage
    }

    public enum RaceType
    {
        Human = 1,
        Undead,
    }
}
