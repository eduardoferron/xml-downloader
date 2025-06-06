using Fiscalapi.Credentials.Core;
using Fiscalapi.XmlDownloader.Builder;
using Fiscalapi.XmlDownloader.Models;
using Fiscalapi.XmlDownloader.Models.SatModels.Invoicing.Cfdi40;
using Fiscalapi.XmlDownloader.Packaging;
using Fiscalapi.XmlDownloader.Services.Authenticate;
using Fiscalapi.XmlDownloader.Services.Common;
using Fiscalapi.XmlDownloader.Services.Download;
using Fiscalapi.XmlDownloader.Services.Query;
using Fiscalapi.XmlDownloader.Services.Verify;

namespace Fiscalapi.XmlDownloader.Services;

public class XmlService
{
    private readonly ICredential credential;
    private readonly SoapEnvelopeBuilder soapEnvelopeBuilder;


    /// <summary>
    /// Gets or sets the token, after the call to the authenticate method
    /// </summary>
    private AuthenticateResult? Token { get; set; }

    public XmlService(ICredential credential, AuthenticateResult? token = null)
    {
        this.credential = credential;
        soapEnvelopeBuilder = new SoapEnvelopeBuilder(credential);

        if (token is not null)
            Token = token;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>Token</returns>
    public async Task<AuthenticateResult> GetCurrentToken()
    {
        if (Token is null || !Token.IsValid())
            Token = await AuthenticateAsync();

        return Token;
    }

    public async Task<AuthenticateResult> AuthenticateAsync()
    {
        var service = new AuthenticateService(soapEnvelopeBuilder);
        Token = await service.Authenticate();

        return Token;
    }

    public async Task<QueryResult> Query(string startDate, string endDate, string? emitterRfc, string? receiverRfc,
        string requestType, string downloadType, AuthenticateResult token)
    {
        var service = new QueryService(soapEnvelopeBuilder);


        var queryResult = await service.Query(
            startDate,
            endDate,
            emitterRfc,
            receiverRfc,
            requestType,
            downloadType,
            token);


        return queryResult;
    }

    public async Task<QueryResult> Query(string startDate, string endDate, string? emitterRfc, string? receiverRfc,
        string requestType, string downloadType)
    {
        var token = await GetCurrentToken();


        var service = new QueryService(soapEnvelopeBuilder);


        var queryResult = await service.Query(
            startDate,
            endDate,
            emitterRfc,
            receiverRfc,
            requestType,
            downloadType,
            token);


        return queryResult;
    }

    public async Task<QueryResult> Query(QueryParameters parameters, AuthenticateResult token)
    {
        var service = new QueryService(soapEnvelopeBuilder);


        var queryResult = await service.Query(
            parameters.StartDate.ToSatFormat(),
            parameters.EndDate.ToSatFormat(),
            parameters.EmitterRfc,
			parameters.ReceiverRfc,
			parameters.RequestType.ToString(),
            parameters.DownloadType.ToString(),
            token);


        return queryResult;
    }

    public async Task<QueryResult> Query(QueryParameters parameters)
    {
        var token = await GetCurrentToken();

        var service = new QueryService(soapEnvelopeBuilder);


        var queryResult = await service.Query(
            parameters.StartDate.ToSatFormat(),
            parameters.EndDate.ToSatFormat(),
            parameters.EmitterRfc,
            parameters.ReceiverRfc,
            parameters.RequestType.ToString(),
            parameters.DownloadType.ToString(),
            token);


        return queryResult;
    }

	public async Task<QueryResult> QueryEmitter(string startDate, string endDate, string? emitterRfc, string[]? receiverRfc,
		string requestType, AuthenticateResult token)
	{
		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryEmitter(
			startDate,
			endDate,
			emitterRfc,
			receiverRfc,
			requestType,
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryEmitter(string startDate, string endDate, string? emitterRfc, string[]? receiverRfc,
		string requestType)
	{
		var token = await GetCurrentToken();

		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryEmitter(
			startDate,
			endDate,
			emitterRfc,
			receiverRfc,
			requestType,
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryEmitter(QueryParameters parameters, AuthenticateResult token)
	{
		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryEmitter(
			parameters.StartDate.ToSatFormat(),
			parameters.EndDate.ToSatFormat(),
			parameters.EmitterRfc,
			parameters.ReceiverRfcs,
			parameters.RequestType.ToString(),
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryEmitter(QueryParameters parameters)
	{
		var token = await GetCurrentToken();

		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryEmitter(
			parameters.StartDate.ToSatFormat(),
			parameters.EndDate.ToSatFormat(),
			parameters.EmitterRfc,
			parameters.ReceiverRfcs,
			parameters.RequestType.ToString(),
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryReceiver(string startDate, string endDate, string? receiverRfc,
		string requestType, AuthenticateResult token)
	{
		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryReceiver(
			startDate,
			endDate,
			receiverRfc,
			requestType,
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryReceiver(string startDate, string endDate, string? receiverRfc,
		string requestType)
	{
		var token = await GetCurrentToken();

		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryReceiver(
			startDate,
			endDate,
			receiverRfc,
			requestType,
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryReceiver(QueryParameters parameters, AuthenticateResult token)
	{
		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryReceiver(
			parameters.StartDate.ToSatFormat(),
			parameters.EndDate.ToSatFormat(),
			parameters.ReceiverRfc,
			parameters.RequestType.ToString(),
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryReceiver(QueryParameters parameters)
	{
		var token = await GetCurrentToken();

		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryReceiver(
			parameters.StartDate.ToSatFormat(),
			parameters.EndDate.ToSatFormat(),
			parameters.ReceiverRfc,
			parameters.RequestType.ToString(),
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryFolio(string folio, AuthenticateResult token)
	{
		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryFolio(
			folio,
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryFolio(string folio)
	{
		var token = await GetCurrentToken();

		var service = new QueryService(soapEnvelopeBuilder);

		var queryResult = await service.QueryFolio(
			folio,
			token);

		return queryResult;
	}

	public async Task<QueryResult> QueryFolio(QueryParameters parameters, AuthenticateResult token)
	{
		var service = new QueryService(soapEnvelopeBuilder);

		if (!string.IsNullOrEmpty(parameters.Folio))
		{
			var queryResult = await service.QueryFolio(
			parameters.Folio,
			token);

			return queryResult;
		}

		return new QueryResult() { 
			IsSuccess = false,
			Message = "Folio can not be null",
			StatusCode = "301"
		};
	}

	public async Task<QueryResult> QueryFolio(QueryParameters parameters)
	{
		var token = await GetCurrentToken();

		var service = new QueryService(soapEnvelopeBuilder);

		if (!string.IsNullOrEmpty(parameters.Folio))
		{
			var queryResult = await service.QueryFolio(
			parameters.Folio,
			token);

			return queryResult;
		}

		return new QueryResult()
		{
			IsSuccess = false,
			Message = "Folio can not be null",
			StatusCode = "301"
		};
	}

	public async Task<VerifyResult> Verify(string? requestUuid, AuthenticateResult token)
    {
        if (string.IsNullOrEmpty(requestUuid))
            throw new ArgumentNullException(nameof(requestUuid));


        var verifyService = new VerifyService(soapEnvelopeBuilder);
        var result = await verifyService.Verify(requestUuid, token);

        return result;
    }

    public async Task<VerifyResult> Verify(string? requestUuid)
    {
        if (string.IsNullOrEmpty(requestUuid))
            throw new ArgumentNullException(nameof(requestUuid));


        var token = await GetCurrentToken();
        var verifyService = new VerifyService(soapEnvelopeBuilder);
        var result = await verifyService.Verify(requestUuid, token);

        return result;
    }


    public async Task<DownloadResult> Download(string? packageId)
    {
        if (string.IsNullOrEmpty(packageId))
            throw new ArgumentNullException(nameof(packageId));


        var token = await GetCurrentToken();

        var downloadService = new DownloadService(soapEnvelopeBuilder);
        var downloadResult = await downloadService.Download(packageId, token);

        return downloadResult;
    }


    public async Task<List<MetadataItem>> GetMetadataAsync(DownloadResult result)
    {
        var metadata = await PackageManager.GetMetadataAsync(result);

        return metadata;
    }


    public async Task<List<Comprobante>> GetCfdisAsync(DownloadResult result)
    {
        var cfdis = await PackageManager.GetCfdisAsync(result);

        return cfdis;
    }
}