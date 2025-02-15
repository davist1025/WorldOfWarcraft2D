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

    public enum SpriteDirection
    {
        North = 1,
        East,
        South,
        West
    }

    public enum ChatChannelType
    {
        Say,
        Whisper,
        Yell
    }

    public enum ChatMessageFlag
    {
        IsServerMessage = 1 << 0,
        IsWhisper = 1 << 1,
        IsGM = 1 << 2,
        IsLocal = 1 << 3,
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

        public static Dictionary<ChatChannelType, Vector4> ChatChannelColors = new Dictionary<ChatChannelType, Vector4>()
        {
            { ChatChannelType.Say, new Vector4(255f, 255f, 255f, 1f) },
            { ChatChannelType.Whisper, new Vector4(116f, 0f, 194f, 1f) }
        };
    }
}
