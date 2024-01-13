// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Magicodes.ExporterAndImporter.Excel;
using Magicodes.ExporterAndImporter.Html;
using Magicodes.ExporterAndImporter.Pdf;
using Magicodes.ExporterAndImporter.Word;

namespace Densen.Service;

/// <summary>
/// 通用导入导出服务类
/// </summary>
public class ImportExportsService : IImportExport
{
    public ImportExportsMiniService MiniExpoet = new ImportExportsMiniService();

    /// <summary>
    /// 通用导出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="items"></param>
    /// <param name="exportType"></param>
    /// <param name="templatePath">* MiniWord 必须指定模板路径,否则出错</param>
    /// <returns></returns>
    public async Task<string> Export<T>(string filePath, List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null) where T : class, new()
    {
        items ??= [];
        switch (exportType)
        {
            case ExportType.Excel:
                var exporter = new ExcelExporter();
                var result = await exporter.Export(filePath + ".xlsx", items);
                return result.FileName;
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
            default:
                return await MiniExpoet.Export(filePath, items, exportType, templatePath);
        }
    }


    /// <summary>
    /// 导出到流
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="exportType"></param>
    /// <param name="templatePath">* MiniWord 必须指定模板路径,否则出错</param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<ExportResult> Export2Stream<T>(List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null, string? fileName = null) where T : class, new()
    {
        MemoryStream memoryStream;
        fileName ??= "";
        items ??= [];
        switch (exportType)
        {
            case ExportType.Excel:
                var exporter = new ExcelExporter();
                var result = await exporter.ExportAsByteArray(items);
                memoryStream = new MemoryStream(result);
                fileName += ".xlsx";
                break;
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
            default:
                return await MiniExpoet.Export2Stream(items, exportType, templatePath, fileName);
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
