using Magicodes.ExporterAndImporter.Excel;
using Magicodes.ExporterAndImporter.Html;
using Magicodes.ExporterAndImporter.Pdf;
using Magicodes.ExporterAndImporter.Word;
using MiniExcelLibs;
using MiniSoftware;

namespace Densen.Service;

/// <summary>
/// 通用导入导出服务类
/// </summary>
public class ImportExportsService : IImportExport
{

    /// <summary>
    /// 通用导出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="items"></param>
    /// <param name="exportType"></param>
    /// <param name="templatePath">模板路径</param>
    /// <returns></returns>
    public async Task<string> Export<T>(string filePath, List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null) where T : class, new()
    {
        items = items ?? new List<T>();
        switch (exportType)
        {
            case ExportType.Pdf:
                var exporterPdf = new PdfExporter();
                var resultPdf = await exporterPdf.ExportListByTemplate(filePath + ".pdf", items, htmlTemplate: templatePath);
                return resultPdf.FileName;
            case ExportType.Word:
                var exporterWord = new WordExporter();
                var resultWord = await exporterWord.ExportListByTemplate(filePath + ".docx", items, htmlTemplate: templatePath);
                return resultWord.FileName;
            case ExportType.Html:
                var exporterHtml = new HtmlExporter();
                var resultHtml = await exporterHtml.ExportListByTemplate(filePath + ".html", items, htmlTemplate: templatePath);
                return resultHtml.FileName;
            case ExportType.MiniExcel:
                MiniExcel.SaveAs(filePath + ".xlsx", items, overwriteFile: true);
                return filePath + ".xlsx";
            case ExportType.MiniWord:
                MiniWord.SaveAsByTemplate(filePath + ".docx", templatePath, items);
                return filePath + ".docx";
            default:
                var exporter = new ExcelExporter();
                var result = await exporter.Export(filePath + ".xlsx", items);
                return result.FileName;
        }
    }


    public  async Task<ExportResult> Export2Stream<T>(List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null, string? fileName = null) where T : class, new()
    {
        var memoryStream = new MemoryStream();
        fileName = fileName ?? "";

        items = items ?? new List<T>();
        switch (exportType)
        {
            case ExportType.Pdf:
                var exporterPdf = new PdfExporter();
                var resultPdf = await exporterPdf.ExportBytesByTemplate(items, templatePath);
                memoryStream = new MemoryStream(resultPdf);
                fileName += ".pdf";
                break;
            case ExportType.Word:
                var exporterWord = new WordExporter();
                var resultWord = await exporterWord.ExportBytesByTemplate(items, templatePath);
                memoryStream = new MemoryStream(resultWord);
                fileName += ".docx";
                break;
            case ExportType.Html:
                var exporterHtml = new HtmlExporter();
                var resultHtml = await exporterHtml.ExportBytesByTemplate(items, templatePath);
                memoryStream = new MemoryStream(resultHtml);
                fileName += ".html";
                break;
            case ExportType.MiniExcel:
                await memoryStream.SaveAsAsync(items);
                memoryStream.Seek(0, SeekOrigin.Begin);
                fileName += ".xlsx";
                break;
            case ExportType.MiniWord:
                MiniWord.SaveAsByTemplate(memoryStream, templatePath, items);
                fileName += ".docx";
                break;
            default:
                var exporter = new ExcelExporter();
                var result = await exporter.ExportAsByteArray(items);
                memoryStream = new MemoryStream(result);
                fileName += ".xlsx";
                break;
        }

        return new ExportResult()
        {
            FileName = fileName,
            Stream = memoryStream
        };

    }

    public static async Task<(IEnumerable<T>? items, string error)> ImportFormExcel<T>(string filePath) where T : class, new()
    {
        IExcelImporter Importer = new ExcelImporter();
        var import = await Importer.Import<T>(filePath);
        if (import.Data == null)
        {
            return (null, import.Exception.Message);
        }
        return (import.Data!.ToList(), "");
    }

}
