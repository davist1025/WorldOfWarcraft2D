using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    public enum WorldEntityType : int
    {
        Player,
        Creature
    }

    public enum ObjectFlags
    {
        None = 0,
        Ambient = 1,
        QuestGiver = 2,
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
