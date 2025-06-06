using Fiscalapi.Credentials.Common;
using Fiscalapi.Credentials.Core;
using Fiscalapi.XmlDownloader.Models;
using Fiscalapi.XmlDownloader.Services;
using Fiscalapi.XmlDownloader.Services.Common;

namespace Fiscalapi.XmlDownloader.Builder;

public class SoapEnvelopeBuilder
{
    private readonly ICredential credential;

    public SoapEnvelopeBuilder(ICredential credential)
    {
        this.credential = credential;
    }

    public string BuildAuthenticate(DateTimePeriod? tokenPeriod = null, string securityTokenId = "")
    {
        tokenPeriod ??= DateTimePeriod.CreateTokenPeriod();

        securityTokenId = string.IsNullOrEmpty(securityTokenId) ? CreateXmlSecurityTokenId() : securityTokenId;
        var certB64 = credential.Certificate.RawDataBytes.ToBase64String();


        var toDigest =
            @$"<u:Timestamp xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" u:Id=""_0"">
                <u:Created>{tokenPeriod.StartDateSat}</u:Created>
                <u:Expires>{tokenPeriod.EndDateSat}</u:Expires>
            </u:Timestamp>";


        var digest = credential.CreateHash(toDigest.Clean());


        var canonicalSignedInfo = CreateCanonicalSignedInfoXml(digest);

        var signature = credential.SignData(canonicalSignedInfo.Clean()).ToBase64String();


        var soapEnvelope =
            @$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
	             <s:Header>
		            <o:Security s:mustUnderstand=""1"" xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
			            <u:Timestamp u:Id=""_0"">
				            <u:Created>{tokenPeriod.StartDateSat}</u:Created>
				            <u:Expires>{tokenPeriod.EndDateSat}</u:Expires>
			            </u:Timestamp>
                        <o:BinarySecurityToken u:Id=""{securityTokenId}"" ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">
                        {certB64}
                        </o:BinarySecurityToken>
                        <Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                           {canonicalSignedInfo}
                            <SignatureValue>{signature}</SignatureValue>
				            <KeyInfo>
					            <o:SecurityTokenReference>
						            <o:Reference ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" URI=""#{securityTokenId}""/>
					            </o:SecurityTokenReference>
				            </KeyInfo>
                        </Signature>
		            </o:Security>
	             </s:Header>
                <s:Body>
		            <Autentica xmlns=""http://DescargaMasivaTerceros.gob.mx""/>
	            </s:Body>
            </s:Envelope>";

        return soapEnvelope.Clean();
    }


    public string BuildQuery(string startDate, string endDate, string requestType, string downloadType,
        string? emitterRfc, string? receiverRfc)
    {
        var signerRfc = credential.Certificate.Rfc;


        var toDigestXml = downloadType.Equals(DownloadType.Emitted.ToString())
            ? @$"<des:SolicitaDescarga xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"">
	            <des:solicitud RfcSolicitante=""{signerRfc}"" FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType} RfcEmisor=""{emitterRfc}"">
	            </des:solicitud>
            </des:SolicitaDescarga>"
            : @$"<des:SolicitaDescarga xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"">
	            <des:solicitud RfcSolicitante=""{signerRfc}"" FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType}"">
		            <des:RfcReceptores>
			            <des:RfcReceptor>{receiverRfc}</des:RfcReceptor>
		            </des:RfcReceptores>
	            </des:solicitud>
            </des:SolicitaDescarga>";


        var digestValue = credential.CreateHash(toDigestXml.Clean());

        var canonicalSignedInfo = CreateCanonicalSignedInfoXml(digestValue);

        var signatureValue = credential.SignData(canonicalSignedInfo.Clean()).ToBase64String();

        var signatureXml = CreateSignatureXml(digestValue, signatureValue);


        var rawRequest = downloadType.Equals(DownloadType.Emitted.ToString())
            ? @$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">
	                <s:Header/>
	                <s:Body>
		                <des:SolicitaDescarga>
			                <des:solicitud RfcSolicitante=""{signerRfc}"" FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType}"" RfcEmisor=""{emitterRfc}"">
				                {signatureXml}
			                </des:solicitud>
		                </des:SolicitaDescarga>
	                </s:Body>
                </s:Envelope>"
            : @$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">
	                <s:Header/>
	                <s:Body>
		                <des:SolicitaDescarga>
			                <des:solicitud RfcSolicitante=""{signerRfc}"" FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType}"" >
				                <des:RfcReceptores>
					                <des:RfcReceptor>{receiverRfc}</des:RfcReceptor>
				                </des:RfcReceptores>
				                {signatureXml}
			                </des:solicitud>
		                </des:SolicitaDescarga>
	                </s:Body>
                </s:Envelope>";


        return rawRequest.Clean();
    }

	public string BuildQueryEmitter(string startDate, string endDate, string requestType, string? emitterRfc, string[]? receiverRfc)
	{
		var infoReceivers = string.Empty;
		if (receiverRfc != null && receiverRfc.Length > 0)
		{
			foreach (var receiver in receiverRfc) 
			{
				infoReceivers += $"<des:RfcReceptor>{receiver}</des:RfcReceptor>";
			}
			infoReceivers = $"<des:RfcReceptores>{infoReceivers}</des:RfcReceptores>";
		}

		var toDigestXml = 
			@$"<des:SolicitaDescargaEmitidos xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"">
	            <des:solicitud FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType}"" RfcEmisor=""{emitterRfc}"">
		            {infoReceivers}
	            </des:solicitud>
            </des:SolicitaDescargaEmitidos>";


		var digestValue = credential.CreateHash(toDigestXml.Clean());

		var canonicalSignedInfo = CreateCanonicalSignedInfoXml(digestValue);

		var signatureValue = credential.SignData(canonicalSignedInfo.Clean()).ToBase64String();

		var signatureXml = CreateSignatureXml(digestValue, signatureValue);

		var rawRequest =
			@$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">
	            <s:Header/>
	            <s:Body>
		            <des:SolicitaDescargaEmitidos>
			            <des:solicitud FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType}"" RfcEmisor=""{emitterRfc}"">
				            {infoReceivers}
				            {signatureXml}
			            </des:solicitud>
		            </des:SolicitaDescargaEmitidos>
	            </s:Body>
            </s:Envelope>";

		return rawRequest.Clean();
	}


	public string BuildQueryReceiver(string startDate, string endDate, string requestType, string? receiverRfc)
	{
		var toDigestXml =
			@$"<des:SolicitaDescargaRecibidos xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"">
	            <des:solicitud EstadoComprobante=""Vigente"" FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType}"" RfcReceptor=""{receiverRfc}"">
	            </des:solicitud>
            </des:SolicitaDescargaRecibidos>";


		var digestValue = credential.CreateHash(toDigestXml.Clean());

		var canonicalSignedInfo = CreateCanonicalSignedInfoXml(digestValue);

		var signatureValue = credential.SignData(canonicalSignedInfo.Clean()).ToBase64String();

		var signatureXml = CreateSignatureXml(digestValue, signatureValue);

		var rawRequest =
			@$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">
	            <s:Header/>
	            <s:Body>
		            <des:SolicitaDescargaRecibidos>
			            <des:solicitud EstadoComprobante=""Vigente"" FechaInicial=""{startDate}"" FechaFinal=""{endDate}"" TipoSolicitud=""{requestType}"" RfcReceptor=""{receiverRfc}"">
				            {signatureXml}
			            </des:solicitud>
		            </des:SolicitaDescargaRecibidos>
	            </s:Body>
            </s:Envelope>";

		return rawRequest.Clean();
	}

	public string BuildQueryFolio(string folio)
	{
		//var signerRfc = credential.Certificate.Rfc;

		var toDigestXml =
			@$"<des:SolicitaDescargaFolio xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"">
	            <des:solicitud Folio=""{folio}"">
	            </des:solicitud>
            </des:SolicitaDescargaFolio>";


		var digestValue = credential.CreateHash(toDigestXml.Clean());

		var canonicalSignedInfo = CreateCanonicalSignedInfoXml(digestValue);

		var signatureValue = credential.SignData(canonicalSignedInfo.Clean()).ToBase64String();

		var signatureXml = CreateSignatureXml(digestValue, signatureValue);

		var rawRequest =
			@$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">
	            <s:Header/>
	            <s:Body>
		            <des:SolicitaDescargaFolio>
			            <des:solicitud Folio=""{folio}"">
				            {signatureXml}
			            </des:solicitud>
		            </des:SolicitaDescargaFolio>
	            </s:Body>
            </s:Envelope>";

		return rawRequest.Clean();
	}


	public string BuildVerify(string requestUuid)
    {
        var signerRfc = credential.Certificate.Rfc;

        var toDigestXml =
            @$"<des:VerificaSolicitudDescarga xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"">
	                <des:solicitud IdSolicitud=""{requestUuid}"" RfcSolicitante=""{signerRfc}""/>
            </des:VerificaSolicitudDescarga>";


        var digestValue = credential.CreateHash(toDigestXml.Clean());

        var canonicalSignedInfo = CreateCanonicalSignedInfoXml(digestValue);

        var signatureValue = credential.SignData(canonicalSignedInfo.Clean()).ToBase64String();

        var signatureXml = CreateSignatureXml(digestValue, signatureValue);


        var rawRequest =
            @$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">
	            <s:Header/>
	            <s:Body>
		            <des:VerificaSolicitudDescarga>
			            <des:solicitud IdSolicitud=""{requestUuid}"" RfcSolicitante=""{signerRfc}"">
				            {signatureXml}
			            </des:solicitud>
		            </des:VerificaSolicitudDescarga>
	            </s:Body>
            </s:Envelope>";


        return rawRequest.Clean();
    }

    public string? BuildDownload(string packageId)
    {
        var signerRfc = credential.Certificate.Rfc;

        var toDigestXml =
            @$"<des:PeticionDescargaMasivaTercerosEntrada xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"">
	                <des:peticionDescarga IdPaquete=""{packageId}"" RfcSolicitante=\""{signerRfc}""></des:peticionDescarga>
            </des:PeticionDescargaMasivaTercerosEntrada>";


        var digestValue = credential.CreateHash(toDigestXml.Clean());

        var canonicalSignedInfo = CreateCanonicalSignedInfoXml(digestValue);

        var signatureValue = credential.SignData(canonicalSignedInfo.Clean()).ToBase64String();

        var signatureXml = CreateSignatureXml(digestValue, signatureValue);


        var rawRequest =
            @$"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">
	                <s:Header/>
	                <s:Body>
		                <des:PeticionDescargaMasivaTercerosEntrada>
			                <des:peticionDescarga IdPaquete=""{packageId}"" RfcSolicitante=""{signerRfc}"">
				                {signatureXml}	
			                </des:peticionDescarga>
		                </des:PeticionDescargaMasivaTercerosEntrada>
	                </s:Body>
            </s:Envelope>";

        return rawRequest.Clean();
    }


    #region Builder Helpers

    private static string CreateXmlSecurityTokenId()
    {
        return $"uuid-{Guid.NewGuid().ToString()}-4";
    }

    private static string CreateCanonicalSignedInfoXml(string digest)
    {
        var xml =
            @$"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
	                <CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>                                             
	                <SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>
	                <Reference URI=""#_0"">
		                <Transforms>
			                <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
		                </Transforms>
		                <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
		                <DigestValue>{digest}</DigestValue>
	                </Reference>
              </SignedInfo>";

        return xml;
    }


    private string CreateSignatureXml(string digestValue, string signatureValue)
    {
        var xml =
            @$"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
					<SignedInfo>
						<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>
						<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>
                        <Reference URI=""#_0"">
							<Transforms>
								<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>
							</Transforms>
							<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>
							<DigestValue>{digestValue}</DigestValue>
						</Reference>
					</SignedInfo>
					<SignatureValue>{signatureValue}</SignatureValue>
					<KeyInfo>
						<X509Data>
							<X509IssuerSerial>
								<X509IssuerName>{credential.Certificate.Issuer}</X509IssuerName>
								<X509SerialNumber>{credential.Certificate.SerialNumber}</X509SerialNumber>
							</X509IssuerSerial>
							<X509Certificate>{credential.Certificate.RawDataBytes.ToBase64String()}</X509Certificate>
						</X509Data>
					</KeyInfo>
			</Signature>";


        return xml.Clean();
    }

    #endregion
}