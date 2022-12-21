using System;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Raydreams.API.Example.Model;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Raydreams.API.Example.Logic;

namespace Raydreams.API.Example.Data
{
    /// <summary>Data manager with Azure Files and Blobs.For now mainly considers Blobs to be image files</summary>
    /// <remarks>Blob and Container names are CASE SENSITIVE</remarks>
    public class AzureBlobRepository
    {
        #region [ Constructor ]

        /// <summary></summary>
        /// <param name="connStr"></param>
        public AzureBlobRepository( string connStr )
        {
            this.ConnectionString = connStr;
        }

        #endregion [ Constructor ]

        #region [ Properties ]

        /// <summary>Azure Storage connection string</summary>
        public string ConnectionString { get; set; } = String.Empty;

        /// <summary>A delegate function that takes a file extension and returns the correct MIME Type.</summary>
        /// <remarks>You can suuply your own to enforce limited file types</remarks>
        public Func<string, string> GetMimeType { get; set; } = (ext) => "text/plain";

        #endregion [ Properties ]

        #region [ Methods ]

        /// <summary>Check to see if a blob already exists in the specified container</summary>
        /// <param name="containerName"></param>
        /// <param name="blobName">File or blob name to check for</param>
        /// <returns></returns>
        /// <remarks>Remember to include a file extension</remarks>
        public bool BlobExists( string containerName, string blobName )
        {
            // blob container name - can we set a default somehow
            if ( containerName == null || blobName == null )
                throw new System.ArgumentNullException( "Arguments can not be null." );

            containerName = containerName.Trim();
            blobName = blobName.Trim();

            if ( containerName == String.Empty || blobName == String.Empty )
                return false;

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return false;

            // Get a reference to the blob name
            BlobClient blob = container.GetBlobClient( blobName );
            exists = blob.Exists();

            return exists.Value;
        }

        /// <summary>Reads a blob and returns it as UTF8 text</summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public string GetTextBlob( string containerName, string blobName )
        {
            string contents = String.Empty;

            // validate input
            if ( String.IsNullOrWhiteSpace( containerName ) || String.IsNullOrWhiteSpace( blobName ) )
                return contents;

            containerName = containerName.Trim();
            blobName = blobName.Trim();

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return contents;

            // set options
            BlobOpenReadOptions op = new BlobOpenReadOptions( false );

            // read the blob to an array - BUG the stream could be longer than Int.Max
            BlobClient blob = container.GetBlobClient( blobName );
            using Stream stream = blob.OpenRead( op );
            byte[] data = new byte[stream.Length];
            stream.Read( data, 0, data.Length );
            stream.Close();

            // get the properties
            BlobProperties props = blob.GetProperties().Value;

            contents = Encoding.UTF8.GetString( data );

            return contents;
        }

        /// <summary>Gets a blob from Azure Storage as just raw bytes with metadata</summary>
        /// <param name="containerName">container name</param>
        /// <param name="blobName">blob name</param>
        /// <returns>Wrapped raw bytes with some metadata</returns>
        public RawFileWrapper GetRawBlob( string containerName, string blobName )
        {
            RawFileWrapper results = new RawFileWrapper();

            // validate input
            if ( String.IsNullOrWhiteSpace( containerName ) || String.IsNullOrWhiteSpace( blobName ) )
                return results;

            containerName = containerName.Trim();
            blobName = blobName.Trim();

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return results;

            // set options
            BlobOpenReadOptions op = new BlobOpenReadOptions( false );

            // read the blob to an array
            BlobClient blob = container.GetBlobClient( blobName );
            using Stream stream = blob.OpenRead( op );
            results.Data = new byte[stream.Length];
            stream.Read( results.Data, 0, results.Data.Length );
            stream.Close();

            // get the properties
            BlobProperties props = blob.GetProperties().Value;

            if ( props == null )
                return results;

            results.ContentType = props.ContentType;

            // get a filename
            if ( props.Metadata.ContainsKey( "filename" ) )
                results.Filename = props.Metadata["filename"].ToString();
            else
                results.Filename = blob.Name;

            return results;
        }

        /// <summary>Get a list of All blobs in the specified contaier</summary>
        /// <param name="containerName">container name</param>
        /// <returns>A list of blob names</returns>
        /// <remarks>Still need to determine what we need back for each blob</remarks>
        public List<string> ListBlobs( string containerName, string pattern = null )
        {
            List<string> blobs = new List<string>();

            // blob container name - can we set a default somehow
            if ( String.IsNullOrWhiteSpace( containerName ) )
                return new List<string>();

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return new List<string>();

            Pageable<BlobItem> results = null;
            if ( String.IsNullOrWhiteSpace( pattern ) )
                results = container.GetBlobs();
            else
                results = container.GetBlobs( prefix: pattern.Trim() );

            IEnumerator<BlobItem> enu = results?.GetEnumerator();
            while ( enu.MoveNext() )
            {
                blobs.Add( enu.Current.Name );
            };

            return blobs;
        }

        /// <summary>From a stream, loads a blob into storage</summary>
        /// <param name="file">file stream to upload</param>
        /// <param name="containerName">container name</param>
		/// <param name="fileName">Original filename to store in the metadata</param>
        /// <param name="blobName">The optional explicit name to give to the blob, otherwise it's random</param>
        /// <returns></returns>
        public async Task<string> StreamBlob( Stream file, string containerName, string fileName, string blobName = null )
        {
            // validate - can add a file type test here as well
            if ( file == null || file.Length < 1 || String.IsNullOrWhiteSpace( containerName ) )
                return null;

            string ext = Path.GetExtension( fileName );
            string contentType = this.GetMimeType( ext );

            // pick a new random name if non was supplied
            if ( String.IsNullOrWhiteSpace( blobName ) )
            {
                Randomizer rnd = new Randomizer();
                blobName = $"{rnd.RandomCode( 11 )}{ext}";
            }
            else
                blobName = blobName.Trim();

            // reset the pointer
            file.Position = 0;

            // Get a reference to a share and then create it
            BlobContainerClient client = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = client.Exists();
            if ( !exists.Value )
                return null;

            // Upload local file
            Dictionary<string, string> meta = new Dictionary<string, string>()
            {
				// original filename
				{ "filename", !String.IsNullOrWhiteSpace(fileName) ? fileName : blobName }
                // optional description
            };

            // set options
            BlobUploadOptions op = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType }, Metadata = meta };

            BlobClient blob = client.GetBlobClient( blobName );
            Response<BlobContentInfo> resp = await blob.UploadAsync( file, op );

            // clear the memory stream
            file.Dispose();

            // return the URL
            return blob.Uri.ToString();
        }

        #endregion [ Methods ]
    }
}

