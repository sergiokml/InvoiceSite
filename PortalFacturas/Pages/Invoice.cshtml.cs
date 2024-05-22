using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cve.CenLib.Models;
using Cve.GraphLib.Interfaces;
using LibreDteDotNet.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalFacturas.Helpers;
using PortalFacturas.Interfaces;
using Refit;

namespace PortalFacturas.Pages;

[Microsoft.AspNetCore.Authorization.Authorize]
public class InvoiceModel : PageModel
{
    private readonly IPdfApi convertToPdfService;
    private readonly ISharePointService _sp;
    private readonly MyConvertHtml convertHtml = new();

    [BindProperty(SupportsGet = true)]
    public int Folio { get; set; }

    [BindProperty]
    public string Mensaje { get; set; }

    public InvoiceModel(IPdfApi convertToPdfService, ISharePointService sp)
    {
        this.convertToPdfService = convertToPdfService;
        _sp = sp;
    }

    public void OnGet()
    {
        //await OnGetHtmlDocAsync(render);
    }

    private Dte BuscarInst(int render)
    {
        List<Instruction> temp = SessionHelperExtension.GetObjectFromJson<List<Instruction>>(
            HttpContext.Session,
            "Instrucciones"
        );

        try
        {
            return temp.SelectMany(store => store.DteAsociados)
                .Where(address => address.Id == render)
                .FirstOrDefault();
        }
        catch (Exception)
        {
            throw new Exception("");
        }
    }

    //Html
    public async Task<ActionResult> OnGetHtmlDocAsync(string render)
    {
        try
        {
            Dte dte = BuscarInst(Convert.ToInt32(render));
            if (dte.EmissionErpA != "0")
            {
                byte[] bytes = await _sp.DownloadFile(dte.EmissionErpA);
                XDocument respnseXml = XDocument.Parse(Encoding.UTF8.GetString(bytes));
                string b = respnseXml.Descendants().First(p => p.Name.LocalName == "DTE").ToString();
                string html = convertHtml.GenerateHtmlContent(b, "80",
                        new DateTime(2014, 08, 22));
                //MemoryStream memoryStream = new(html);
                return new ContentResult { Content = html, ContentType = "text/html" };
                //return new FileStreamResult(memoryStream, "text/html");
            }
            else
            {
                Mensaje = $"El documento no ha sido subido al Drive.";
            }
        }
        catch (Exception ex)
        {
            Mensaje = $"No se puede mostrar el documento: {ex.Message}";
        }
        return Page();
    }

    //XmlDoc
    public async Task<ActionResult> OnGetXmlDocAsync(int render)
    {
        try
        {
            Dte dte = BuscarInst(render);
            if (dte.EmissionErpA != "0")
            {
                byte[] bytes = await _sp.DownloadFile(dte.EmissionErpA);
                return File(bytes, "application/xml", $"{GetFileName(dte)}.xml");
            }
            else
            {
                Mensaje = $"El documento no ha sido subido al Drive.";
            }
        }
        catch (Exception ex)
        {
            Mensaje = $"No se puede mostrar el documento: {ex.Message}";
        }
        return Page();
    }

    //PdfDoc
    public async Task<ActionResult> OnGetPdfDocAsync(int render)
    {
        try
        {
            Dte dte = BuscarInst(Convert.ToInt32(render));
            if (dte.EmissionErpA != "0")
            {
                byte[] bytes = await _sp.DownloadFile(dte.EmissionErpA);
                XDocument respnseXml = XDocument.Parse(Encoding.UTF8.GetString(bytes));

                string b = respnseXml.Descendants().First(p => p.Name.LocalName == "DTE").ToString();

                string html = convertHtml.GenerateHtmlContent(b, "80",
                       new DateTime(2014, 08, 22));

                ModelConvert jsondoc = new() { Html = html, FileName = $"{GetFileName(dte)}.pdf" };

                ApiResponse<PdfApiModel> res = await convertToPdfService.UploadToConvert(jsondoc);
                if (res.IsSuccessStatusCode)
                {
                    using HttpClient client = new();
                    byte[] pdfBytes = await client.GetByteArrayAsync(res.Content.FileUrl);
                    return File(pdfBytes, "application/pdf", $"{GetFileName(dte)}.pdf");
                    //return Redirect(res.Content.FileUrl);
                }
                else
                {
                    Mensaje = $"No se pudo convertir a PDF.{res.Error.Message}";
                }
            }
            else
            {
                Mensaje = "El documento no ha sido subido al Drive.";
            }
        }
        catch (Exception ex)
        {
            Mensaje = $"No se puede mostrar el documento: {ex.Message}";
        }
        return Page();
    }

    private string GetFileName(Dte dte)
    {
        string filename = string.Empty;
        if (dte.Type == 1) //33
        {
            filename = $"{TempData["EmisorID"]}_33_{dte.Folio}";
        }
        else if (dte.Type == 2) //61
        {
            filename = $"{TempData["EmisorID"]}_61_{dte.Folio}";
        }
        TempData.Keep("EmisorID");
        return filename;
    }
}
