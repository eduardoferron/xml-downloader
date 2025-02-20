using System.Net;
using Fiscalapi.XmlDownloader.Services.Common;

namespace Fiscalapi.XmlDownloader.SoapClient
{
    public class InternalResponse
    {
        public bool IsSuccessStatusCode { get; set; }
        public string? ReasonPhrase { get; set; }
        public string? RawResponse { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public EndPointName EndPointName { get; set; }
        public InternalRequest? InternalRequest { get; set; }
    }
}