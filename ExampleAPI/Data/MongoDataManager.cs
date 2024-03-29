﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Raydreams.API.Example.Data
{
	/// <summary>Base class for a Mongo DB Manager</summary>
    /// <typeparam name="T">The entity object type</typeparam>
    /// <typeparam name="P">The data type of the _id field</typeparam>
	public abstract class MongoDataManager<T, P>
	{
		#region [Fields]

		private MongoClient _dbConn = null;

		private string _db = "dev";

		#endregion [Fields]

		public MongoDataManager( string connStr, string db )
		{
			// HACK - this is the only thing that still works on GUID match for now
			MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;
			//if ( BsonSerializer.LookupSerializer<Guid>() == null )
			//BsonSerializer.RegisterSerializer( new GuidSerializer( GuidRepresentation.Standard ) );
			// BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;

			if ( !String.IsNullOrWhiteSpace( connStr ) )
				this._dbConn = new MongoClient( connStr );

			if ( !String.IsNullOrWhiteSpace( connStr ) )
				this._db = db.Trim();

			// how to deal with Enums
			ConventionRegistry.Register( "EnumStringConvention", new ConventionPack { new EnumRepresentationConvention( BsonType.String ) }, t => true );
		}

		#region [Properties]

		/// <summary>The DB Client</summary>
		public MongoClient Client
		{
			get { return this._dbConn; }
			protected set { this._dbConn = value; }
		}

		/// <summary>The DB</summary>
		public IMongoDatabase Database
		{
			get { return this.Client.GetDatabase( this._db ); }
		}

		#endregion [Properties]

		#region [Base Methods]

		/// <summary>Gets all the collections in this DB.</summary>
		/// <returns>A list of collection names.</returns>
		/// <remarks>Useful to test the DB is alive</remarks>
		public List<string> GetAllCollections()
		{
			List<string> colls = new List<string>();
			List<BsonDocument> all = this.Database.ListCollections().ToList();
			foreach ( BsonDocument doc in all )
			{
				colls.Add( doc.GetValue( "name" ).ToString() );
			}
			return colls;
		}

		/// <summary>Gets a document by it's ID as a string input.</summary>
		protected T Get( string id, string tbl )
		{
			if ( String.IsNullOrWhiteSpace( id ) || String.IsNullOrWhiteSpace( tbl ) )
				return default( T );

			id = id.Trim();

			FilterDefinition<T> filter = null;

			IMongoCollection<T> col = this.Database.GetCollection<T>( tbl );

			if ( typeof( P ) == typeof( ObjectId ) )
				filter = Builders<T>.Filter.Eq( "_id", new ObjectId( id ) );
			else if ( typeof( P ) == typeof( Guid ) )
			{
				//BsonValue bid = new GuidSerializer( GuidRepresentation.Standard ).ToBsonValue( new Guid( id ) );
				filter = Builders<T>.Filter.Eq( "_id", new Guid( id ) );
			}
			else if ( typeof( P ) == typeof( string ) )
				filter = Builders<T>.Filter.Eq( "_id", id );
			else
				return default( T );

			List<T> results = col.Find<T>( filter ).ToList();

			return ( results != null && results.Count > 0 ) ? results[0] : default( T );
		}

		/// <summary>Gets a document by it's ID as the native type.</summary>
		protected T Get( P id, string tbl )
		{
			if ( String.IsNullOrWhiteSpace( tbl ) )
				return default( T );


			IMongoCollection<T> col = this.Database.GetCollection<T>( tbl );
			FilterDefinition<T> filter = Builders<T>.Filter.Eq( "_id", id );
			List<T> results = col.Find<T>( filter ).ToList();

			return ( results != null && results.Count > 0 ) ? results[0] : default( T );
		}

		/// <summary>Gets all documents in the collection</summary>
		protected List<T> GetAll( string tbl ) => this.List(tbl);

		/// <summary>Gets all documents in the collection</summary>
		protected List<T> List( string tbl )
		{
			if ( String.IsNullOrWhiteSpace( tbl ) )
				return new List<T>();

			// find the root account
			IMongoCollection<T> collection = this.Database.GetCollection<T>( tbl );
			List<T> results = collection.Find<T>( Builders<T>.Filter.Empty ).ToList();

			return ( results != null && results.Count > 0 ) ? results : new List<T>();
		}

		/// <summary>Deletes a document in the collection</summary>
		protected bool Delete( P id, string tbl )
		{
			if ( String.IsNullOrWhiteSpace( tbl ) )
				return false;

			IMongoCollection<T> col = this.Database.GetCollection<T>( tbl );
			FilterDefinition<T> filter = Builders<T>.Filter.Eq( "_id", id );
			DeleteResult result = col.DeleteOne( filter );

			return ( result.DeletedCount > 0 );
		}

        /// <summary>Inserts a document into the collection</summary>
        protected bool Insert( T item, string tbl )
		{
			if ( String.IsNullOrWhiteSpace( tbl ) )
				return false;

			this.Database.GetCollection<T>( tbl ).InsertOne( item );

			return true;
		}

		#endregion [Base Methods]
	}
}
