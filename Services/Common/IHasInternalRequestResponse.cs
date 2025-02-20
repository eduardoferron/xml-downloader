using Fiscalapi.XmlDownloader.SoapClient;

namespace Fiscalapi.XmlDownloader.Services.Common;

public interface IHasInternalRequestResponse
{
    public InternalRequest? InternalRequest { get; set; }
    public InternalResponse? InternalResponse { get; set; }
}