using Fiscalapi.XmlDownloader.Builder;
using Fiscalapi.XmlDownloader.Services.Authenticate;
using Fiscalapi.XmlDownloader.Services.Common;
using Fiscalapi.XmlDownloader.SoapClient;

namespace Fiscalapi.XmlDownloader.Services.Query
{
    public class QueryService
    {
        private readonly SoapEnvelopeBuilder soapEnvelopeBuilder;

        public QueryService(SoapEnvelopeBuilder soapEnvelopeBuilder)
        {
            this.soapEnvelopeBuilder = soapEnvelopeBuilder;
        }

        public async Task<QueryResult> Query(string startDate, string endDate, string? emitterRfc, string? receiverRfc,
            string requestType, string downloadType, AuthenticateResult token)
        {
            var rawRequest = soapEnvelopeBuilder.BuildQuery(
                startDate,
                endDate,
                requestType,
                downloadType,
                emitterRfc,
                receiverRfc);


            var endpoint = Helper.GetQueryEndPoint();


            var internalRequest = new InternalRequest
            {
                Url = endpoint.Uri,
                SoapAction = endpoint.SoapAction,
                RawRequest = rawRequest,
                HttpMethod = HttpMethod.Post,
                EndPointName = EndPointName.Query,
                Token = token
            };


            var internalResponse = await InternalHttpClient.SendAsync(internalRequest);


            var result = Helper.GetQueryResult(internalResponse?.RawResponse);
            result.InternalRequest = internalRequest;
            result.InternalResponse = internalResponse;

            return result;
        }
    }
}