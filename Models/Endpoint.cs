using Fiscalapi.XmlDownloader.Services.Common;

namespace Fiscalapi.XmlDownloader.Models
{
    public class Endpoint
    {
        public EndPointName EndPointName { get; set; }
        public EndPointType EndPointType { get; set; }
        public string Uri { get; set; } = string.Empty;
        public string SoapAction { get; set; } = string.Empty;
    }
}