// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Densen.Service;
using FreeSql.DataAnnotations;
using Magicodes.ExporterAndImporter.Excel;
using Magicodes.ExporterAndImporter.Html;
using Magicodes.ExporterAndImporter.Pdf;
using Magicodes.ExporterAndImporter.Word;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Task = System.Threading.Tasks.Task;


public partial class Program
{

    public class Foo
    {
        [Column(IsIdentity = true)]
        public int ID { get; set; }

        public decimal UnitPrice1 { get; set; } = 1;
        public decimal? UnitPrice2 { get; set; } = null;
        public decimal UnitPrice3 { get; set; } = 1;
        public decimal UnitPrice4 { get; set; } = 1;
        public decimal UnitPrice5 { get; set; } = 1; 

    }

    public static async Task 测试IE()
    {
        var sw = Stopwatch.StartNew();
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test", "xxxx");
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        var items = new List<Foo>()  ;
        for (int i = 0; i < 10; i++)
        {
            items.Add(new Foo() { UnitPrice1 =i+1});
        }
        Console.WriteLine("生成数据" + sw.Elapsed.TotalSeconds);
        sw.Restart();

        var exporter2 = new ImportExportsService();

        Console.WriteLine(await exporter2.Export(filePath+"mini", items, ExportType.MiniExcel));
        Console.WriteLine(await exporter2.Export(filePath, items, ExportType.Excel));
        Console.WriteLine(await exporter2.Export(filePath, items, ExportType.Word));
        Console.WriteLine(await exporter2.Export(filePath, items, ExportType.Pdf));
        Console.WriteLine(await exporter2.Export(filePath, items, ExportType.Html));

        Console.WriteLine((await exporter2.Export2Stream(  items, ExportType.MiniExcel)).FileName);
        Console.WriteLine((await exporter2.Export2Stream(items, ExportType.Excel)).FileName);
        //Console.WriteLine((await exporter2.Export2Stream(items, ExportType.Word)).FileName);
        //Console.WriteLine((await exporter2.Export2Stream(items, ExportType.Pdf)).FileName);
        //Console.WriteLine((await exporter2.Export2Stream(items, ExportType.Html)).FileName);

        return;

        try
        {
            MiniExcel.SaveAs(filePath + ".MiniExcel.xlsx", items, overwriteFile: true);
            Console.WriteLine(sw.Elapsed.TotalSeconds);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        sw.Restart();

        try
        {
            var exporter = new ExcelExporter();
            var result = await exporter.Export(filePath + ".xlsx", items);
            Console.WriteLine(result.FileName);
            Console.WriteLine(sw.Elapsed.TotalSeconds);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        sw.Restart();

        try
        {
            var exporterWord = new WordExporter();
            var resultWord = await exporterWord.ExportListByTemplate(filePath + ".docx", items);
            Console.WriteLine(resultWord.FileName);
            Console.WriteLine(sw.Elapsed.TotalSeconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        sw.Restart();

        try
        {
            var exporterHtml = new HtmlExporter();
            var resultHtml = await exporterHtml.ExportListByTemplate(filePath + ".html", items);
            Console.WriteLine(resultHtml.FileName);
            Console.WriteLine(sw.Elapsed.TotalSeconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        sw.Restart();

        try
        {
            var exporterPdf = new PdfExporter();
            var resultPdf = await exporterPdf.ExportListByTemplate(filePath + ".pdf", items);
            Console.WriteLine(resultPdf.FileName);
            Console.WriteLine(sw.Elapsed.TotalSeconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        sw.Restart();
    }

}
