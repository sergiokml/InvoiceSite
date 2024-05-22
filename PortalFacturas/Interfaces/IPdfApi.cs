using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Refit;

namespace PortalFacturas.Interfaces;

public interface IPdfApi
{

    [Headers("Authorization: 3135c383-6882-408a-a965-2212131d3a48")]
    [Post("/chrome/pdf/html/")]
    Task<ApiResponse<PdfApiModel>> UploadToConvert([Body(BodySerializationMethod.Serialized)] ModelConvert data);
}


public class ModelConvert
{
    public string Html { get; set; }
    public string FileName { get; set; }



}


public class PdfApiModel
{
    public string ResponseId { get; set; }
    public float MbOut { get; set; }
    public float Cost { get; set; }
    public float Seconds { get; set; }
    public object Error { get; set; }
    public bool Success { get; set; }
    public string FileUrl { get; set; }
}
