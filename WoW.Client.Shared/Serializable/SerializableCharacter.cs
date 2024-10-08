﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Serializable
{
    public class SerializableCharacter
    {
        /// <summary>
        /// The index of this user's character based on their list of characters.
        /// </summary>
        public int CharacterId { get; init; }

        public string Name { get; set; }

        public int RaceId { get; set; }

        public int GuildId { get; set; }

        public int Level { get; set; }

        public CharacterClassType Class { get; set; }

        // todo: should we load everything else into this object such as equipment, buffs, etc?

        public SerializableCharacter(int localId, int raceId, int guildId, string name, int level, CharacterClassType @class)
        {
            CharacterId = localId;
            RaceId = raceId;
            GuildId = guildId;
            Name = name;
            Level = level;
            Class = @class;
        }
    }
}
