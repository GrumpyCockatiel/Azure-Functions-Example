using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Raydreams.API.Example
{
    /// <summary>Uploads binary data to Blob Storage</summary>
    public class GetFileFunction : BaseFunction
    {
        public GetFileFunction( IDataGateway gate ) : base( gate )
        { }

        [Function( "GetFile" )]
        public async Task<HttpResponseData> Run( [HttpTrigger( AuthorizationLevel.Anonymous, "get", Route = "v1/file/{blobName}" )] HttpRequestData req, string blobName, FunctionContext ctx )
        {
            ILogger logger = ctx.GetLogger( "API" );
            logger.LogInformation( $"{GetType().Name} triggered." );

            if ( String.IsNullOrWhiteSpace( blobName ) )
                return req.BadResponse( "The Blob Name with extension to get is required" );

            APIResult<RawFileWrapper> results = new APIResult<RawFileWrapper>() { ResultCode = APIResultType.Unknown };

            try
            {
                var reqData = await ctx.GetHttpRequestDataAsync();
                var file = reqData.Body;

                // upload the file - container name is hard coded in config for now
                results.ResultObject = await this.Gateway.GetFile( blobName.Trim() );
            }
            catch ( Exception exp )
            {
                return req.ReponseError( results, exp, logger );
            }

            if ( results.ResultObject.IsValid )
                results.ResultCode = APIResultType.Success;

            return req.BlobResponse( results.ResultObject );
        }
    }

}