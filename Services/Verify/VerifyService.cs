using Fiscalapi.XmlDownloader.Builder;
using Fiscalapi.XmlDownloader.Services.Authenticate;
using Fiscalapi.XmlDownloader.Services.Common;
using Fiscalapi.XmlDownloader.SoapClient;

namespace Fiscalapi.XmlDownloader.Services.Verify
{
    public class VerifyService
    {
        private readonly SoapEnvelopeBuilder soapEnvelopeBuilder;

        public VerifyService(SoapEnvelopeBuilder envelopeBuilder)
        {
            soapEnvelopeBuilder = envelopeBuilder;
        }


        public async Task<VerifyResult> Verify(string requestUuid, AuthenticateResult token)
        {
            var rawRequest = soapEnvelopeBuilder.BuildVerify(requestUuid);


            var endpoint = Helper.GetVerifyEndPoint();


            var internalRequest = new InternalRequest
            {
                Url = endpoint.Uri,
                SoapAction = endpoint.SoapAction,
                RawRequest = rawRequest,
                HttpMethod = HttpMethod.Post,
                EndPointName = EndPointName.Verify,
                Token = token
            };


            var internalResponse = await InternalHttpClient.SendAsync(internalRequest);


            var result = Helper.GetVerifyResult(internalResponse?.RawResponse);
            result.InternalRequest = internalRequest;
            result.InternalResponse = internalResponse;

            return result;
        }
    }
}