using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Raydreams.API.Example.Serializers
{
    /// <summary>Converts an class object[] to a BSON string[]</summary>
    /// <remarks>This is for converting a object[] args in a logger</remarks>
    public class ObjToStringArraySerializer : SerializerBase<object[]>
    {
        /// <summary>Write the class object[] out as a BSON string[]</summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="value"></param>
        public override void Serialize( BsonSerializationContext context, BsonSerializationArgs args, object[] value )
        {
            // if the incoming array is null
            if ( value == null )
            {
                context.Writer.WriteNull();
                return;
            }

            BsonDocument doc = new BsonDocument();

            for ( int i = 0; i < value.Length; ++i )
            {
                if ( value[i] == null )
                    continue;

                string[] kvp = value[i].ToString().Split( "=", StringSplitOptions.RemoveEmptyEntries );

                if ( kvp.Length > 1 )
                    doc.Add( new BsonElement( kvp[0], kvp[1] ) );
                else if ( kvp.Length > 0 )
                    doc.Add( new BsonElement( $"arg{i + 1}", kvp[0] ) );
            }

            context.Writer.WriteRawBsonDocument( new RawBsonDocument( doc.ToBson() ).Slice );
        }

        /// <summary>Write back to a LogRecord object as an object[]</summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns>The object[] property type of args</returns>
        public override object[] Deserialize( BsonDeserializationContext context, BsonDeserializationArgs args )
        {
            // BsonType should be array
            BsonType bsonType = context.Reader.GetCurrentBsonType();

            if ( bsonType == BsonType.Null )
            {
                context.Reader.ReadNull();
                return null;
            }

            if ( bsonType != BsonType.Document )
                return null;

            IByteBuffer buffer = context.Reader.ReadRawBsonDocument();
            BsonDocument doc = new RawBsonDocument( buffer ).ToBsonDocument();

            if ( doc == null || doc.ElementCount < 1 )
                return null;

            // get all the document values and turn back to object[]
            return doc.ToDictionary().Select( i => $"{i.Key}={i.Value}" ).ToArray();
        }
    }
}

