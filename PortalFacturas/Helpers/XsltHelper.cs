//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Text;
//using System.Xml;
//using System.Xml.Xsl;

//using PortalFacturas.Properties;

//using ZXing;
//using ZXing.CoreCompat.Rendering;
//using ZXing.PDF417;
//using ZXing.PDF417.Internal;

//namespace PortalFacturas.Helpers
//{
//    public class XsltHelper
//    {
//        private readonly XslCompiledTransform xlstFile;
//        private readonly XsltArgumentList xlstArgs = null!;

//        public XsltHelper()
//        {
//            xlstFile = new XslCompiledTransform();
//            xlstArgs = new XsltArgumentList();
//            using StringReader sr = new(Resources.Custodium);
//            XmlReaderSettings settings = new() { Async = true };
//            using XmlReader xr = XmlReader.Create(sr, settings);
//            xlstFile.Load(xr);
//        }



//        private XmlNode LoadXmlDocument(XmlDocument xml, string doc)
//        {
//            xml.LoadXml(doc);
//            XmlNodeList nodeTED = xml.GetElementsByTagName("TED");
//            return nodeTED.Count != 1 ? throw new Exception("LoadXmlDocument") : nodeTED[0];
//        }

//        public byte[] TransformarXslt(byte[] inputXml)
//        {
//            XmlDocument doc = new();
//            XmlNode ted = LoadXmlDocument(doc, Encoding.UTF8.GetString(inputXml));
//            try
//            {
//                xlstArgs.Clear();
//                BarcodeWriter<Bitmap> barcodeWriter =
//                    new()
//                    {
//                        Format = BarcodeFormat.PDF_417,
//                        Options = new PDF417EncodingOptions
//                        {
//                            ErrorCorrection = PDF417ErrorCorrectionLevel.L6,
//                            Height = 3,
//                            Width = 9,
//                            Compaction = Compaction.BYTE,
//                            Margin = 6
//                        },
//                        Renderer = new BitmapRenderer()
//                    };
//                using StringReader sr = new(Encoding.UTF8.GetString(inputXml));
//                using XmlReader xr = XmlReader.Create(sr);
//                using StringWriter sw = new();
//                using MemoryStream ms = new();
//                using Bitmap bitmap = new(barcodeWriter.Write(ted.OuterXml));
//                bitmap.Save(ms, ImageFormat.Png);
//                string SigBase64 = Convert.ToBase64String(ms.GetBuffer());
//                xlstArgs.AddParam("TedTimbre", "", "data:image/png;base64," + SigBase64);
//                xlstFile.Transform(xr, xlstArgs, sw);
//                return Encoding.UTF8.GetBytes(sw.ToString());
//            }
//            catch (Exception)
//            {
//                // throw new Exception();
//            }
//            return null!;
//        }
//    }
//}
