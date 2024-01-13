// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using MiniExcelLibs;
using MiniSoftware;

namespace Densen.Service;

/// <summary>
/// MiniExcel/MiniWord 导入导出服务类
/// </summary>
public class ImportExportsMiniService : IImportExport
{

    /// <summary>
    /// 通用导出
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="items"></param>
    /// <param name="exportType"></param>
    /// <param name="templatePath">* MiniWord 必须指定模板路径,否则出错</param>
    /// <returns></returns>
    public Task<string> Export<T>(string filePath, List<T>? items = null, ExportType exportType = ExportType.MiniExcel, string? templatePath = null) where T : class, new()
    {
        items ??= [];
        switch (exportType)
        {
            case ExportType.Word:
            case ExportType.MiniWord:
                if (string.IsNullOrEmpty(templatePath))
                {
                    throw new ArgumentNullException(nameof(templatePath), "MiniWord 必须指定模板路径,否则出错");
                }
                MiniWord.SaveAsByTemplate(filePath + ".docx", templatePath, items);
                return Task.FromResult(filePath + ".docx");
            default:
                MiniExcel.SaveAs(filePath + ".xlsx", items, overwriteFile: true);
                return Task.FromResult(filePath + ".xlsx");
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
    public async Task<ExportResult> Export2Stream<T>(List<T>? items = null, ExportType exportType = ExportType.MiniExcel, string? templatePath = null, string? fileName = null) where T : class, new()
    {
        var memoryStream = new MemoryStream();
        fileName ??= "";

        items ??= [];
        switch (exportType)
        {
            case ExportType.Word:
            case ExportType.MiniWord:
                MiniWord.SaveAsByTemplate(memoryStream, templatePath, items);
                memoryStream.Seek(0, SeekOrigin.Begin);
                fileName += ".docx";
                break;
            default:
                await memoryStream.SaveAsAsync(items);
                memoryStream.Seek(0, SeekOrigin.Begin);
                fileName += ".xlsx";
                break;
        }

        return new ExportResult()
        {
            FileName = fileName,
            Stream = memoryStream
        };

    }

}
