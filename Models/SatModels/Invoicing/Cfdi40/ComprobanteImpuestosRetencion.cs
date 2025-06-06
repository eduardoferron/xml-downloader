﻿//***************************************************************************************
// <Author>                                                                             *
//     Jesús Mendoza Jaurez.                                                            *
//     mendoza.git@gmail.com                                                            *
//     https://github.com/mendozagit                                                    *
//                                                                                      *
//     Los cambios en este archivo podrían causar un comportamiento incorrecto.         *
//     Este código no ofrece ningún tipo de garantía, se generó para ayudar a la        *
//     Comunidad open source, siéntanse libre de utilizarlo, sin ninguna garantía.      *
//     Nota: Mantenga este comentario para respetar al autor.                           *
// </Author>                                                                            *
//***************************************************************************************

using System.Xml.Serialization;

namespace Fiscalapi.XmlDownloader.Models.SatModels.Invoicing.Cfdi40
{
    [Serializable]

    public class ComprobanteImpuestosRetencion
    {
        private string impuestoField;

        private decimal importeField;


        [XmlAttribute]
        public string Impuesto
        {
            get { return impuestoField; }
            set { impuestoField = value; }
        }


        [XmlAttribute]
        public decimal Importe
        {
            get { return importeField; }
            set { importeField = value; }
        }
    }
}