using Fiscalapi.XmlDownloader.Services.Common;
using Fiscalapi.XmlDownloader.SoapClient;

namespace Fiscalapi.XmlDownloader.Services.Query
{
    public class QueryResult : Result, IHasSuccessResponse, IHasInternalRequestResponse
    {
        public string? RequestUuid { get; set; }
        public bool IsSuccess { get; set; }
        public InternalRequest? InternalRequest { get; set; }
        public InternalResponse? InternalResponse { get; set; }
    }
}