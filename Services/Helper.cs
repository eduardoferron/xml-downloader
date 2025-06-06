﻿using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Fiscalapi.XmlDownloader.Helpers;
using Fiscalapi.XmlDownloader.Models;
using Fiscalapi.XmlDownloader.Models.SatModels.Complements.Payments.Pago20;
using Fiscalapi.XmlDownloader.Models.SatModels.Complements.TFD;
using Fiscalapi.XmlDownloader.Models.SatModels.Invoicing.Cfdi40;
using Fiscalapi.XmlDownloader.Services.Authenticate;
using Fiscalapi.XmlDownloader.Services.Common;
using Fiscalapi.XmlDownloader.Services.Download;
using Fiscalapi.XmlDownloader.Services.Query;
using Fiscalapi.XmlDownloader.Services.Verify;
using static System.Convert;

namespace Fiscalapi.XmlDownloader.Services
{
    public static class Helper
    {
        #region Consts

        private const string SatFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        #endregion


        #region Endpoints

        private static IEnumerable<Endpoint> Endpoints { get; } = new List<Endpoint>
        {
            new Endpoint
            {
                EndPointName = EndPointName.Authenticate,
                EndPointType = EndPointType.OrdinaryCfdi,
                Uri = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/Autenticacion/Autenticacion.svc",
                SoapAction = "http://DescargaMasivaTerceros.gob.mx/IAutenticacion/Autentica"
            },
            new Endpoint
            {
                EndPointName = EndPointName.Query,
                EndPointType = EndPointType.OrdinaryCfdi,
                Uri = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc",
                SoapAction = "http://DescargaMasivaTerceros.sat.gob.mx/ISolicitaDescargaService/SolicitaDescarga"
            },
			new Endpoint
			{
				EndPointName = EndPointName.QueryEmitter,
				EndPointType = EndPointType.OrdinaryCfdi,
				Uri = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc",
				SoapAction = "http://DescargaMasivaTerceros.sat.gob.mx/ISolicitaDescargaService/SolicitaDescargaEmitidos"
			},
			new Endpoint
			{
				EndPointName = EndPointName.QueryReceiver,
				EndPointType = EndPointType.OrdinaryCfdi,
				Uri = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc",
				SoapAction = "http://DescargaMasivaTerceros.sat.gob.mx/ISolicitaDescargaService/SolicitaDescargaRecibidos"
			},
			new Endpoint
			{
				EndPointName = EndPointName.QueryFolio,
				EndPointType = EndPointType.OrdinaryCfdi,
				Uri = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc",
				SoapAction = "http://DescargaMasivaTerceros.sat.gob.mx/ISolicitaDescargaService/SolicitaDescargaFolio"
			},
			new Endpoint
            {
                EndPointName = EndPointName.Verify,
                EndPointType = EndPointType.OrdinaryCfdi,
                Uri = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/VerificaSolicitudDescargaService.svc",
                SoapAction =
                    "http://DescargaMasivaTerceros.sat.gob.mx/IVerificaSolicitudDescargaService/VerificaSolicitudDescarga"
            },
            new Endpoint
            {
                EndPointName = EndPointName.Download,
                EndPointType = EndPointType.OrdinaryCfdi,
                Uri = "https://cfdidescargamasiva.clouda.sat.gob.mx/DescargaMasivaService.svc",
                SoapAction = "http://DescargaMasivaTerceros.sat.gob.mx/IDescargaMasivaTercerosService/Descargar"
            },
            //Retenciones
            new Endpoint
            {
                EndPointName = EndPointName.Authenticate,
                EndPointType = EndPointType.RetentionCfdi,
                Uri = "https://retendescargamasivasolicitud.clouda.sat.gob.mx/Autenticacion/Autenticacion.svc",
                SoapAction = null
            },
            new Endpoint
            {
                EndPointName = EndPointName.Query,
                EndPointType = EndPointType.RetentionCfdi,
                Uri = "https://retendescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc",
                SoapAction = null
            },
            new Endpoint
            {
                EndPointName = EndPointName.Verify,
                EndPointType = EndPointType.RetentionCfdi,
                Uri = "https://retendescargamasivasolicitud.clouda.sat.gob.mx/VerificaSolicitudDescargaService.svc",
                SoapAction = null
            },
            new Endpoint
            {
                EndPointName = EndPointName.Download,
                EndPointType = EndPointType.RetentionCfdi,
                Uri = "https://retendescargamasiva.clouda.sat.gob.mx/DescargaMasivaService.svc",
                SoapAction = null
            }
        };


        public static Endpoint GetEndPoint(EndPointName name, EndPointType type)
        {
            return Endpoints.FirstOrDefault(x => x.EndPointName == name && x.EndPointType == type) ?? new Endpoint();
        }

        public static Endpoint GetAuthenticateEndPoint()
        {
            return GetEndPoint(EndPointName.Authenticate, EndPointType.OrdinaryCfdi);
        }

        public static Endpoint GetQueryEndPoint()
        {
            return GetEndPoint(EndPointName.Query, EndPointType.OrdinaryCfdi);
        }

		public static Endpoint GetQueryEmitterEndPoint()
		{
			return GetEndPoint(EndPointName.QueryEmitter, EndPointType.OrdinaryCfdi);
		}

		public static Endpoint GetQueryReceiverEndPoint()
		{
			return GetEndPoint(EndPointName.QueryReceiver, EndPointType.OrdinaryCfdi);
		}

		public static Endpoint GetQueryFolioEndPoint()
		{
			return GetEndPoint(EndPointName.QueryFolio, EndPointType.OrdinaryCfdi);
		}

		public static Endpoint GetVerifyEndPoint()
        {
            return GetEndPoint(EndPointName.Verify, EndPointType.OrdinaryCfdi);
        }

        public static Endpoint GetDownloadEndPoint()
        {
            return GetEndPoint(EndPointName.Download, EndPointType.OrdinaryCfdi);
        }

        public static List<Endpoint> GetAllEndPoints(EndPointType type)
        {
            return Endpoints.Where(x => x.EndPointType == type).ToList();
        }

        #endregion


        #region DateTimeSAT

        public static string ToSatFormat(this DateTime dateTime)
        {
            return dateTime.ToString(SatFormat);
        }

        #endregion


        /// <summary>
        /// Remove horizontal spaces at beginning, carriage return (CR), Line Feed (LF) and xml declaration on its own line.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>cleaned str</returns>
        public static string Clean(this string str)
        {
            #region Comments

            /*
             * Coincidencias Basicas
               .       - Cualquier Caracter, excepto nueva linea
               \d      - Cualquier Digitos (0-9)
               \D      - No es un Digito (0-9)
               \w      - Caracter de Palabra (a-z, A-Z, 0-9, _)
               \W      - No es un Caracter de Palabra.
               \s      - Espacios de cualquier tipo. (espacio, tab, nueva linea)
               \S      - No es un Espacio, Tab o nueva linea.
               
               Limites
               \b      - Limite de Palabra
               \B      - No es un Limite de Palabra
               ^       - Inicio de una cadena de texto
               $       - Final de una cadena de texto
               
               Cuantificadores:
               *       - 0 o Más
               +       - 1 o Más
               ?       - 0 o Uno
               {3}     - Numero Exacto
               {3,4}   - Rango de Numeros (Minimo, Maximo)
               
               Conjuntos de Caracteres
               []      - Caracteres dentro de los brackets
               [^ ]    - Caracteres que NO ESTAN dentro de los brackets
               
               Grupos
               ( )     - Grupo
               |       - Uno u otro
             */

            #endregion

            // A: remove horizontal spaces at beginning
            str = Regex.Replace(str, @"^\s*", string.Empty, RegexOptions.Multiline).TrimStart();


            // B: remove horizontal spaces + optional CR + LF
            str = Regex.Replace(str, @"\s*\r?\n", string.Empty, RegexOptions.Multiline).TrimEnd();

            // C: xml declaration on its own line
            str = str.Replace(@"?><", @$"?>{Environment.NewLine}<");

            return string.IsNullOrEmpty(str) ? string.Empty : str;
        }


        public static AuthenticateResult GetAuthenticateResult(string? rawResponse)
        {
            var token = new AuthenticateResult();

            // AuthenticateResult
            if (string.IsNullOrEmpty(rawResponse))
                return token;

            var xml = new XmlDocument();
            xml.LoadXml(rawResponse);


            var created = xml?.DocumentElement?.GetElementsByTagName("u:Created")[0]?.InnerText;

            var expires = xml?.DocumentElement?.GetElementsByTagName("u:Expires")[0]?.InnerText;

            var autenticaResult = xml?.DocumentElement?.GetElementsByTagName("AutenticaResult")[0]?.InnerText;


            if (created is null | expires is null | autenticaResult is null)
            {
                var faultcode = xml?.DocumentElement?.GetElementsByTagName("faultcode")[0]?.InnerText;
                var faultstring = xml?.DocumentElement?.GetElementsByTagName("faultstring")[0]?.InnerText;

                token.ErrorMessage = $"{faultcode}: {faultstring}";
                return token;
            }


            token.ValidFrom = ToDateTime(created);
            token.ValidTo = ToDateTime(expires);
            token.Value = $"WRAP access_token=\"{HttpUtility.UrlDecode(autenticaResult)}\"";
            token.IsSuccess = true;


            return token;
        }

        public static QueryResult GetQueryResult(string? rawResponse)
        {
            var result = new QueryResult();

            if (string.IsNullOrEmpty(rawResponse)) return result;


            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawResponse);


            result.StatusCode = xmlDoc.GetElementsByTagName("SolicitaDescargaResult")[0]
                ?.Attributes?["CodEstatus"]
                ?.Value;

            result.Message =
                xmlDoc.GetElementsByTagName("SolicitaDescargaResult")[0]?.Attributes?["Mensaje"]?.Value;

            result.RequestUuid = xmlDoc.GetElementsByTagName("SolicitaDescargaResult")[0]
                ?.Attributes?["IdSolicitud"]
                ?.Value;


            if (result.StatusCode is null || result.Message is null || result.RequestUuid is null)
                return result;

            result.IsSuccess = string.Equals(result.StatusCode, "5000") && result.RequestUuid is not null;

            return result;
        }

		public static QueryResult GetQueryEmitterResult(string? rawResponse)
		{
			var result = new QueryResult();

			if (string.IsNullOrEmpty(rawResponse)) return result;


			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(rawResponse);


			result.StatusCode = xmlDoc.GetElementsByTagName("SolicitaDescargaEmitidosResult")[0]
				?.Attributes?["CodEstatus"]
				?.Value;

			result.Message =
				xmlDoc.GetElementsByTagName("SolicitaDescargaEmitidosResult")[0]?.Attributes?["Mensaje"]?.Value;

			result.RequestUuid = xmlDoc.GetElementsByTagName("SolicitaDescargaEmitidosResult")[0]
				?.Attributes?["IdSolicitud"]
				?.Value;


			if (result.StatusCode is null || result.Message is null || result.RequestUuid is null)
				return result;

			result.IsSuccess = string.Equals(result.StatusCode, "5000") && result.RequestUuid is not null;

			return result;
		}

		public static QueryResult GetQueryReceiverResult(string? rawResponse)
		{
			var result = new QueryResult();

			if (string.IsNullOrEmpty(rawResponse)) return result;


			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(rawResponse);


			result.StatusCode = xmlDoc.GetElementsByTagName("SolicitaDescargaRecibidosResult")[0]
				?.Attributes?["CodEstatus"]
				?.Value;

			result.Message =
				xmlDoc.GetElementsByTagName("SolicitaDescargaRecibidosResult")[0]?.Attributes?["Mensaje"]?.Value;

			result.RequestUuid = xmlDoc.GetElementsByTagName("SolicitaDescargaRecibidosResult")[0]
				?.Attributes?["IdSolicitud"]
				?.Value;


			if (result.StatusCode is null || result.Message is null || result.RequestUuid is null)
				return result;

			result.IsSuccess = string.Equals(result.StatusCode, "5000") && result.RequestUuid is not null;

			return result;
		}

		public static QueryResult GetQueryFolioResult(string? rawResponse)
		{
			var result = new QueryResult();

			if (string.IsNullOrEmpty(rawResponse)) return result;


			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(rawResponse);


			result.StatusCode = xmlDoc.GetElementsByTagName("SolicitaDescargaFolioResult")[0]
				?.Attributes?["CodEstatus"]
				?.Value;

			result.Message =
				xmlDoc.GetElementsByTagName("SolicitaDescargaFolioResult")[0]?.Attributes?["Mensaje"]?.Value;

			result.RequestUuid = xmlDoc.GetElementsByTagName("SolicitaDescargaFolioResult")[0]
				?.Attributes?["IdSolicitud"]
				?.Value;


			if (result.StatusCode is null || result.Message is null || result.RequestUuid is null)
				return result;

			result.IsSuccess = string.Equals(result.StatusCode, "5000") && result.RequestUuid is not null;

			return result;
		}

		public static VerifyResult GetVerifyResult(string? rawResponse)
        {
            var result = new VerifyResult();

            if (string.IsNullOrEmpty(rawResponse)) return result;


            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawResponse);


            result.StatusCode = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0]
                ?.Attributes?["CodEstatus"]?.Value;

            result.Message = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0]
                ?.Attributes?["Mensaje"]?.Value;

            result.CfdiQty = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0]
                ?.Attributes?["NumeroCFDIs"]?.Value;

            result.CodeStatusRequest = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0]
                ?.Attributes?["CodigoEstadoSolicitud"]?.Value;

            result.StatusRequest = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0]
                ?.Attributes?["EstadoSolicitud"]?.Value;


            // 1	Aceptada
            // 2	En proceso
            // 3	Terminada
            // 4	Error
            // 5	Rechazada
            // 6	Vencida
            if (result.StatusRequest is not "3") return result;


            var packageList = xmlDoc.GetElementsByTagName("IdsPaquetes");

            //add all packageids from raw response to result object
            for (var i = 0; i < packageList.Count; i++)
                result.PackagesIds.Add(packageList[i]?.InnerXml ?? string.Empty);

            //Revove empty packages (if exist)
            result.PackagesIds.RemoveAll(x => x.Equals(string.Empty));

            //Ensure has cfdi and status "terminada".
            result.IsSuccess = result.StatusRequest is "3" && result.PackagesIds.Count > 0;

            return result;
        }

        public static DownloadResult GetDownloadResult(string? packageId, string? rawResponse)
        {
            var result = new DownloadResult();

            if (string.IsNullOrEmpty(rawResponse)) return result;


            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawResponse);


            result.StatusCode = xmlDoc.GetElementsByTagName("h:respuesta")[0]
                ?.Attributes?["CodEstatus"]?.Value;

            result.Message = xmlDoc.GetElementsByTagName("h:respuesta")[0]
                ?.Attributes?["Mensaje"]?.Value;

            result.PackageId = packageId;

            result.PackageBase64 = xmlDoc.GetElementsByTagName("Paquete")[0]?.InnerXml;


            //Ensure has cfdi and status "terminada".
            result.IsSuccess = result.PackageId is not null && !string.IsNullOrEmpty(result.PackageBase64);


            return result;
        }


        #region Logging

        public static void SaveLog(string strContent)
        {
            SaveLog("info", new Exception(strContent));
        }

        public static void SaveLog(string strTitle, Exception ex)
        {
            try
            {
                var filePath = Path.Combine(Settings.LogsDirectory, DateTime.Now.ToString("yyyyMMdd") + ".txt");

                if (!Directory.Exists(Settings.LogsDirectory))
                    Directory.CreateDirectory(Settings.LogsDirectory);


                if (!File.Exists(filePath))
                {
                    var FsCreate = new FileStream(filePath, FileMode.Create);
                    FsCreate.Close();
                    FsCreate.Dispose();
                }

                var FsWrite = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                var SwWrite = new StreamWriter(FsWrite);

                var strContent = ex.ToString();

                SwWrite.WriteLine("{0}{1}[{2:HH:mm:ss}]{3}", "--------------------------------", strTitle, DateTime.Now,
                    "--------------------------------");
                SwWrite.Write(strContent);
                SwWrite.WriteLine(Environment.NewLine);
                SwWrite.WriteLine(" ");
                SwWrite.Flush();
                SwWrite.Close();
            }
            catch
            {
                // ignored
            }
        }

        #endregion


        #region Serialization

        #region Json

        public static string SerializeToJsonString<T>(T t)
        {
            var jsonString = JsonSerializer.Serialize(t, new JsonSerializerOptions {WriteIndented = true});
            return jsonString;
        }

        public static async Task SerializeToJsonJsonFile<T>(T t, string fullPath)
        {
            await using var createStream = File.Create(fullPath);
            await JsonSerializer.SerializeAsync(createStream, t, new JsonSerializerOptions {WriteIndented = true});
            await createStream.DisposeAsync();
        }

        public static async Task<T?> DeserializeFromJsonFile<T>(string fullPath)
        {
            await using var openStream = File.OpenRead(fullPath);
            var result = await JsonSerializer.DeserializeAsync<T>(openStream);

            return result;
        }


        public static T? DeserializeFromJsonString<T>(string stringJson)
        {
            return JsonSerializer.Deserialize<T>(stringJson);
        }

        #endregion

        #region Xml

        internal static Comprobante? Deserialize(string xmlPath)
        {
            var cfdiInfo = GetInvoiceVersion(xmlPath);

            var xmlSerializer = GetSerializer(cfdiInfo);

            using var xmlReader = XmlReader.Create(xmlPath);

            var comprobante = xmlSerializer.Deserialize(xmlReader) as Comprobante;

            if (comprobante?.Complemento[0].Any is null)
                return comprobante;

            SerializeComplements(comprobante);

            return comprobante;
        }

        private static void SerializeComplements(Comprobante comprobante)
        {
            foreach (var complement in comprobante.Complemento[0].Any)
            {
                if (complement.Prefix.Contains("tfd"))
                {
                    comprobante.TimbreFiscalDigital =
                        DeserializeComplement<TimbreFiscalDigital>(complement.OuterXml);
                }
                else if (complement.Prefix.Contains("pago20"))
                {
                    comprobante.Pago20 =
                        DeserializeComplement<Pagos>(complement.OuterXml);
                }
                else if (complement.Prefix.Contains("pago10"))
                {
                    comprobante.Pago10 =
                        DeserializeComplement<Models.SatModels.Complements.Payments.Pago10.Pagos>(complement.OuterXml);
                }
            }
        }

        private static XmlSerializer GetSerializer(CfdiInfo cfdiInfo)
        {
            var xmlSerializer = new XmlSerializer(typeof(Comprobante),
                new XmlRootAttribute
                {
                    IsNullable = false,
                    Namespace = cfdiInfo.CfdiVersion.Equals("3.3") ? Settings.RootNameSpace33 : Settings.RootNameSpace40
                });
            return xmlSerializer;
        }

        private static CfdiInfo GetInvoiceVersion(string xmlFullPath)
        {
            var xml = new XmlDocument();
            xml.Load(xmlFullPath);


            var cfdiInfo = new CfdiInfo
            {
                CfdiVersion = xml.GetElementsByTagName("cfdi:Comprobante")[0]?.Attributes?["Version"]?.Value ?? "4.0",
                CfdiType = xml.GetElementsByTagName("cfdi:Comprobante")[0]?.Attributes?["TipoDeComprobante"]?.Value ??
                           "I"
            };


            return cfdiInfo;
        }

        private static T? DeserializeComplement<T>(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using var stringReader = new StringReader(xml);

            var complement = (T) xmlSerializer.Deserialize(stringReader);

            return complement;
        }

        #endregion

        #endregion
    }
}