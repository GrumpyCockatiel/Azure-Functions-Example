using System.Security.Cryptography;

namespace Raydreams.API.Example.Logic
{
    /// <summary>Inits a randomizer and has several methods for creating random things</summary>
	public class Randomizer
	{
        /// <summary></summary>
        private Random _rand;

        #region [Constructors]

        /// <summary>Init with some already created Random instance</summary>
        /// <param name="generator">A random generator to use</param>
        /// <remarks></remarks>
        public Randomizer( Random generator )
        {
            this._rand = generator ?? InitRandom();
        }

        /// <summary>Generate the random generator using RandomNumberGenerator</summary>
        public Randomizer()
        {
            this._rand = InitRandom();
        }

        /// <summary>Init a random generator with 4 random bytes chosen at creation</summary>
        /// <remarks>
        /// There are multiple ways to generate a seed like
        /// new Random( Guid.NewGuid().GetHashCode() );
        /// </remarks>
        protected static Random InitRandom()
        {
            // generate a random seed to size of Int32
            byte[] seed = RandomNumberGenerator.GetBytes( 4 );

            // convert to an integer value
            return new Random( BitConverter.ToInt32( seed, 0 ) );
        }

        #endregion [Constructors]

        /// <summary>Get the random generator being used.</summary>
        public Random Generator => this._rand;

        /// <summary>Gets the next random int from the generator calling Next where the max value is not included [min,max)</summary>
        /// <returns></returns>
        protected int NextRandom( int minValue, int maxValue )
        {
            return this._rand.Next( minValue, maxValue );
        }

        /// <summary>Generates a random character code using upper and lower case and all digits</summary>
        public string RandomCode( int len = 10, CharGroup set = CharGroup.Lower | CharGroup.Upper | CharGroup.Digits )
        {
            if ( len < 1 )
                len = 1;

            if ( len > 1024 )
                len = 1024;

            char[] chars = this.MakeCharSet( set );

            if ( chars.Length < 1 )
                return String.Empty;

            StringBuilder results = new StringBuilder();

            for ( int i = 0; i < len; ++i )
                results.Append( chars[this._rand.Next( 0, chars.Length )] );

            return results.ToString();
        }

        /// <summary>Generates a list of characters to choose from the specified sets</summary>
        /// <param name="set"></param>
        /// <returns></returns>
        protected char[] MakeCharSet( CharGroup set = CharGroup.Empty )
        {
            List<char> all = new List<char>();

            // add in digits
            if ( ( set & CharGroup.Digits ) == CharGroup.Digits )
                all.AddRange( CharSets.Digits );

            // add in lower case
            if ( ( set & CharGroup.Lower ) == CharGroup.Lower )
                all.AddRange( CharSets.LowerCase );

            // add in upper case
            if ( ( set & CharGroup.Upper ) == CharGroup.Upper )
                all.AddRange( CharSets.UpperCase );

            // add in all special chars
            // if both limited and all are specified - then its still all
            if ( ( set & CharGroup.AllSpecial ) == CharGroup.AllSpecial )
            {
                all.AddRange( CharSets.LimitedSpecial );
                all.AddRange( CharSets.MoreSpecial );
            }
            else if ( ( set & CharGroup.LimitedSpecial ) == CharGroup.LimitedSpecial )
                all.AddRange( CharSets.LimitedSpecial );

            // if at this point it's empty then nothing was specified

            // remove similars
            if ( all.Count > 0 && ( set & CharGroup.NoSimilar ) == CharGroup.NoSimilar )
            {
                return all.Except( CharSets.Similar ).ToArray();
            }

            return all.ToArray();
        }
    }
}

