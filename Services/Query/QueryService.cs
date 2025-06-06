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

		public async Task<QueryResult> QueryEmitter(string startDate, string endDate, string? emitterRfc, string[]? receiverRfc,
			string requestType, AuthenticateResult token)
		{
			var rawRequest = soapEnvelopeBuilder.BuildQueryEmitter(
				startDate,
				endDate,
				requestType,
				emitterRfc,
				receiverRfc);


			var endpoint = Helper.GetQueryEmitterEndPoint();


			var internalRequest = new InternalRequest
			{
				Url = endpoint.Uri,
				SoapAction = endpoint.SoapAction,
				RawRequest = rawRequest,
				HttpMethod = HttpMethod.Post,
				EndPointName = EndPointName.QueryEmitter,
				Token = token
			};


			var internalResponse = await InternalHttpClient.SendAsync(internalRequest);


			var result = Helper.GetQueryEmitterResult(internalResponse?.RawResponse);
			result.InternalRequest = internalRequest;
			result.InternalResponse = internalResponse;

			return result;
		}

		public async Task<QueryResult> QueryReceiver(string startDate, string endDate, string? receiverRfc,
			string requestType, AuthenticateResult token)
		{
			var rawRequest = soapEnvelopeBuilder.BuildQueryReceiver(
				startDate,
				endDate,
				requestType,
				receiverRfc);


			var endpoint = Helper.GetQueryReceiverEndPoint();


			var internalRequest = new InternalRequest
			{
				Url = endpoint.Uri,
				SoapAction = endpoint.SoapAction,
				RawRequest = rawRequest,
				HttpMethod = HttpMethod.Post,
				EndPointName = EndPointName.QueryReceiver,
				Token = token
			};


			var internalResponse = await InternalHttpClient.SendAsync(internalRequest);


			var result = Helper.GetQueryReceiverResult(internalResponse?.RawResponse);
			result.InternalRequest = internalRequest;
			result.InternalResponse = internalResponse;

			return result;
		}

		public async Task<QueryResult> QueryFolio(string folio, AuthenticateResult token)
		{
			var rawRequest = soapEnvelopeBuilder.BuildQueryFolio(folio);


			var endpoint = Helper.GetQueryFolioEndPoint();


			var internalRequest = new InternalRequest
			{
				Url = endpoint.Uri,
				SoapAction = endpoint.SoapAction,
				RawRequest = rawRequest,
				HttpMethod = HttpMethod.Post,
				EndPointName = EndPointName.QueryFolio,
				Token = token
			};


			var internalResponse = await InternalHttpClient.SendAsync(internalRequest);


			var result = Helper.GetQueryFolioResult(internalResponse?.RawResponse);
			result.InternalRequest = internalRequest;
			result.InternalResponse = internalResponse;

			return result;
		}


	}
}