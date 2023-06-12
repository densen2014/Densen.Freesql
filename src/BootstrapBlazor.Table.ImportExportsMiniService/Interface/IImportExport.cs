// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

namespace Densen.Service;

public enum ExportType
{
    Excel,
    Pdf,
    Word,
    Html,
    MiniExcel,
    MiniWord,
}
public class ExportResult
{
    public string? FileName { get; set; }
    public Stream? Stream { get; set; }
}

public interface IImportExport
{

    /// <summary>
    /// 通用导出接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="items"></param>
    /// <param name="exportType"></param>
    /// <param name="templatePath">* MiniWord 必须指定模板路径,否则出错</param>
    /// <returns></returns>
    Task<string> Export<T>(string filePath, List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null) where T : class, new();

    Task<ExportResult> Export2Stream<T>(List<T>? items = null, ExportType exportType = ExportType.Excel, string? templatePath = null, string? fileName = null) where T : class, new();

}
