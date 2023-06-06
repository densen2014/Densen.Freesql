using MiniExcelLibs;
using MiniSoftware;

namespace Densen.Service;

/// <summary>
/// MiniExcel/MiniWord 导入导出服务类
/// </summary>
public class ImportExportsMiniService: IImportExport
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
    public Task<string> Export<T>(string filePath, List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null) where T : class, new()
    {
        items = items ?? new List<T>();
        switch (exportType)
        {
            case ExportType.MiniExcel:
            default:
                MiniExcel.SaveAs(filePath + ".xlsx", items, overwriteFile: true);
                return Task .FromResult ( filePath + ".xlsx");
            case ExportType.MiniWord:
                MiniWord.SaveAsByTemplate(filePath + ".docx", templatePath, items);
                return Task.FromResult(filePath + ".docx");
        }
    }


    public async Task<ExportResult> Export2Stream<T>(List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null, string? fileName = null) where T : class, new()
    {
        var memoryStream = new MemoryStream();
        fileName = fileName ?? "";

        items = items ?? new List<T>();
        switch (exportType)
        {
            case ExportType.MiniExcel:
            default:
                await memoryStream.SaveAsAsync(items);
                memoryStream.Seek(0, SeekOrigin.Begin);
                fileName += ".xlsx";
                break;
            case ExportType.MiniWord:
                MiniWord.SaveAsByTemplate(memoryStream, templatePath, items);
                fileName += ".docx";
                break;
        }

        return new ExportResult()
        {
            FileName = fileName,
            Stream = memoryStream
        };

    }

}
