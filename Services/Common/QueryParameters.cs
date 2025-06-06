namespace Fiscalapi.XmlDownloader.Services.Common
{
    public class QueryParameters
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DownloadType DownloadType { get; set; } // Emitted | Received
        public RequestType RequestType { get; set; } // Cfdi | Meta,

        public string? Folio { get; set; }
        public string? EmitterRfc { get; set; }

        public string? ReceiverRfc { get; set; }
        public string[]? ReceiverRfcs { get; set; }
    }
}