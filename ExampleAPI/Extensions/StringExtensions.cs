using System.Globalization;
using System.Text.RegularExpressions;

namespace Raydreams.API.Example.Extensions
{
    /// <summary>Static Functions to more safely convert data types from a string to a specific data type.</summary>
    /// <remarks>Not all data types have been added yet and more are added as they are needed</remarks>
    public static class DataTypeConverter
    {
        /// <summary>Returns the database to use from a Mongo Connection String</summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        /// <remarks>mongodb+srv://myuser-rw:password2022!@bimrx-dev.ot79q.mongodb.net/CloudManager-DEV?retryWrites=true&w=majority</remarks>
        public static string ParseMongoConnStr( this string connStr )
        {
            if ( String.IsNullOrWhiteSpace( connStr ) )
                return null;

            Regex pattern = new Regex( @"mongodb\+srv://.+/(?<db>[-a-zA-Z]+)?.*", RegexOptions.IgnoreCase | RegexOptions.Singleline );
            Match matches = pattern.Match( connStr );

            var group = matches.Groups["db"];

            return group.Value;
        }

        /// <summary>Parses a string to an Integer with a default</summary>
        /// <param name="def">Default value to return when a fail occurs</param>
        /// <returns>an integer</returns>
        public static int GetIntValue(this string value, int def = 0) => (Int32.TryParse(value, out int convert)) ? convert : def;

        /// <summary>Parses a string to an Integer with a default and minimum value</summary>
        /// <param name="def">Default value to return when a fail occurs</param>
        /// <param name="min">Minimum value to return</param>
        /// <returns>an integer</returns>
        public static int GetIntValue(this string value, int min, int def = 0)
        {
            if (Int32.TryParse(value, out int convert))
                return (convert < min) ? min : convert;

            return def;
        }

        /// <summary>Gets a nullable Int value</summary>
        /// <returns></returns>
        /// <param name="def">Default value to return when a fail occurs</param>
        public static int? GetNullableIntValue(this string value) => (Int32.TryParse(value, out int convert)) ? convert : null;

        /// <summary>Gets a long value with a default when conversion fails</summary>
        /// <returns></returns>
        /// <param name="def">Default value to return when a fail occurs</param>
        public static long GetLongValue(this string value, long def = 0) => (Int64.TryParse(value, out long convert)) ? convert : def;

        /// <summary>Converts a string to a boolean value based on the first char</summary>
        /// <returns></returns>
        public static bool GetBooleanValue(this string value)
        {
            // empty and null are always false
            if (String.IsNullOrWhiteSpace(value))
                return false;

            // get the first char
            char leading = value.Trim().ToLower()[0];

            // true, yes and 1 all produce true
            return (leading == 't' || leading == 'y' || leading == '1') ? true : false;
        }

        /// <summary>Converts a string to a boolean value based on the first char where null is returned for null/empty strings</summary>
        /// <returns></returns>
        public static bool? GetNullableBooleanValue(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            char leading = value.Trim().ToLower()[0];

            return (leading == 't' || leading == 'y' || leading == '1') ? true : false;
        }

        /// <summary>Gets a double value from the string where 0 is the default</summary>
        /// <returns></returns>
        public static double GetDoubleValue(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return 0;

            if (Double.TryParse(value, out double result))
                return result;

            return 0;
        }

        /// <summary>Gets a double value from the string where null is the default</summary>
        /// <returns></returns>
        public static double? GetNullableDoubleValue(this string value) => (Double.TryParse(value, out double convert)) ? convert : null;

        /// <summary>Gets a float value from the string where 0 is the default</summary>
        /// <returns></returns>
        public static float GetFloatValue(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return 0;

            if (Single.TryParse(value, out float result))
                return result;

            return 0;
        }

        /// <summary>Converts a string to a DateTimeOffset with an explicit default if not parsed</summary>
        /// <returns></returns>
        public static DateTimeOffset GetDateTimeValue(this string value, DateTimeOffset def)
        {
            if (String.IsNullOrWhiteSpace(value))
                return def;

            if (!DateTimeOffset.TryParse(value, out DateTimeOffset convert))
                return def;

            return convert;
        }

        /// <summary>Converts a string to a DateTimeOffset returning null if not parsed</summary>
        /// <returns></returns>
        public static DateTimeOffset? GetNullableDateTimeValue(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            if (!DateTimeOffset.TryParse(value, out DateTimeOffset convert))
                return null;

            return convert;
        }

        /// <summary>Convert a string to a GUID or get GUID.Empty</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid GetGuidValue(this string value) => (Guid.TryParse(value, out Guid convert)) ? convert : Guid.Empty;

        /// <summary>Converts a string to an enum value of enum T failing to default(T)</summary>
        /// <param name="ignoreCase">Ignore case by default</param>
        /// <returns></returns>
        /// <remarks>Case is ignored</remarks>
        public static T GetEnumValue<T>(this string value, bool ignoreCase = true) where T : struct, IConvertible
        {
            T result = default(T);

            if (String.IsNullOrWhiteSpace(value))
                return result;

            if (Enum.TryParse<T>(value.Trim(), ignoreCase, out result))
                return result;

            return default(T);
        }

        /// <summary>Converts a string to an enum value with the specified default on fail</summary>
        /// <param name="def">Explicit default value if parsing fails</param>
        /// <param name="ignoreCase">Ignore case by default</param>
        /// <returns></returns>
        public static T GetEnumValue<T>(this string value, T def, bool ignoreCase = true) where T : struct, IConvertible
        {
            T result = def;

            if (String.IsNullOrWhiteSpace(value))
                return result;

            if (Enum.TryParse<T>(value.Trim(), ignoreCase, out result))
                return result;

            return def;
        }

        /// <summary>Converts a string to an enum value with the specified default on fail</summary>
        /// <param name="def">Explicit default value if parsing fails</param>
        /// <param name="ignoreCase">Ignore case by default</param>
        /// <returns></returns>
        public static Nullable<T> GetNullableEnumValue<T>(this string value, bool ignoreCase = true) where T : struct, IConvertible
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            if (Enum.TryParse<T>(value.Trim(), ignoreCase, out T result))
                return result;

            return null;
        }

        /// <summary>Encodes a byte array as BASE64 URL encoded</summary>
        /// <remarks>This needs to move to Array Extensions</remarks>
        public static string BASE64UrlEncode( byte[] arg )
        {
            string s = Convert.ToBase64String( arg ); // Regular base64 encoder
            //s = s.Split( '=' )[0];
            s = s.TrimEnd( '=' ); // remove any trailing =
            s = s.Replace( '+', '-' ); // 62nd char of encoding
            s = s.Replace( '/', '_' ); // 63rd char of encoding
            return s;
        }

        /// <summary>Decodes a BASE64 URL encoded string back to its original bytes</summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static byte[] BASE64UrlDecode( this string str )
        {
            str = str.Replace( '-', '+' ); // 62nd char of encoding
            str = str.Replace( '_', '/' ); // 63rd char of encoding

            // the modulus of the length by 4 can not be remaninder 1
            switch ( str.Length % 4 )
            {
                // no padding necessary
                case 0: break;
                // pad with two =
                case 2: str += "=="; break;
                // pad once
                case 3: str += "="; break;
                // hopefully this does not happen
                default:
                    throw new System.Exception( "Illegal BASE64URL string!" );
            }

            return Convert.FromBase64String( str ); // Standard base64 decoder
        }

    }
}
