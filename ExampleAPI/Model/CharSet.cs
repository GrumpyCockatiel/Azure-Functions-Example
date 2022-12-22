namespace Raydreams.API.Example.Model
{
    /// <summary>Groups combinations of character sets</summary>
    [FlagsAttribute]
    public enum CharGroup : short
    {
        /// <summary>The empty set - nothing</summary>
        Empty = 0,
        /// <summary>Digits only</summary>
        Digits = 1,
        /// <summary>lower case characters only</summary>
        Lower = 2,
        /// <summary>upper case chracters only</summary>
        Upper = 4,
        /// <summary>A subset of the special characters most systems accept for password creation</summary>
        LimitedSpecial = 8,
        /// <summary>All the special characters</summary>
        AllSpecial = 16,
        /// <summary>The set of characters that are similar</summary>
        NoSimilar = 32,
        /// <summary>All of the characters</summary>
        All = 64
    };

    /// <summary></summary>
    public static class CharSets
    {
        public static readonly char[] LowerCase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public static readonly char[] UpperCase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public static readonly char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static readonly char[] LimitedSpecial = { '!', '~', '*', '@', '#', '$', '%', '+', '?', '&' };

        public static readonly char[] MoreSpecial = { '"', '\'', '(', ')', ',', '-', '.', '/', ':', ';', '<', '=', '>', '[', '\\', ']', '^', '_', '`', '{', '|', '}' };

        public static readonly char[] Similar = { '0', 'O', 'o', '1', 'l', 'L', 'i', 'I', '!', '\'', '`', '"' };
    }
}

