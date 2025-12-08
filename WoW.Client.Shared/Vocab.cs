using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    public enum RaceType
    {
        Human = 1,
        Orc
    }

    public enum ClassType
    {
        Warrior = 1,
        Mage
    }

    public enum StatisticType
    {
        Strength = 1,
        Agility,
        Intellect,
        Stamina,
        Sprit
    }

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
        IsRoamer = 1 << 0,
        IsMerchant = 1 << 1
    }

    public enum SpriteDirection
    {
        North = 1,
        East,
        South,
        West
    }

    //public enum ChatMessageFlag
    //{
    //    IsServerMessage = 1 << 0,
    //    IsWhisper = 1 << 1,
    //    IsGM = 1 << 2,
    //    IsLocal = 1 << 3,
    //}

    public enum ChatChannel
    {
        Say,
        Yell,
        Whisper,
        World,

        Server, // yellow messages.
        Support, // gm chat window.
        // todo: custom channels created by users?
    }

    public static class Utils
    {
        /// <summary>
        /// Converts the given <paramref name="input"/> into a SHA256 hashed string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSHA256(string input)
        {
            string shaHash = "";

            using (var hash = SHA256.Create())
            {
                var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                shaHash = Convert.ToHexString(byteArray).ToLower();
            }

            return shaHash;
        }

        public static Dictionary<ChatChannel, Vector4> ChatChannelColors = new Dictionary<ChatChannel, Vector4>()
        {
            { ChatChannel.Server, new Vector4(250f / 255f, 244f / 255f, 125f / 255f, 1f) },
            { ChatChannel.Say, new Vector4(255f / 255f, 255f / 255f, 255f / 255f, 1f) },
        };
    }
}
