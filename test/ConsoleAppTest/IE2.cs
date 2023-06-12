// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

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


public partial class Program2
{

    public class Foo
    {
        public decimal UnitPrice1 { get; set; } = 1;
        public decimal? UnitPrice2 { get; set; } = null;
        public decimal UnitPrice3 { get; set; } = 1;
        public decimal UnitPrice4 { get; set; } = 1;
        public decimal UnitPrice5 { get; set; } = 1;
        public decimal UnitPrice6 { get; set; } = 1;
        public decimal UnitPrice7 { get; set; } = 1;
        public decimal UnitPrice8 { get; set; } = 1;
        public decimal UnitPrice9 { get; set; } = 1;
        public decimal UnitPrice10 { get; set; } = 1;
        public decimal UnitPriceD1 { get; set; } = 1;
        public decimal? UnitPriceD2 { get; set; } = null;
        public decimal UnitPriceD3 { get; set; } = 1;
        public decimal UnitPriceD4 { get; set; } = 1;
        public decimal UnitPriceD5 { get; set; } = 1;
        public decimal UnitPriceD6 { get; set; } = 1;
        public decimal UnitPriceD7 { get; set; } = 1;
        public decimal UnitPriceD8 { get; set; } = 1;
        public decimal UnitPriceD9 { get; set; } = 1;
        public decimal UnitPriceD10 { get; set; } = 1;
        public decimal UnitPriceC1 { get; set; } = 1;
        public decimal? UnitPriceC2 { get; set; } = null;
        public decimal UnitPriceC3 { get; set; } = 1;
        public decimal UnitPriceC4 { get; set; } = 1;
        public decimal UnitPriceC5 { get; set; } = 1;
        public decimal UnitPriceC6 { get; set; } = 1;
        public decimal UnitPriceC7 { get; set; } = 1;
        public decimal UnitPriceC8 { get; set; } = 1;
        public decimal UnitPriceC9 { get; set; } = 1;
        public decimal UnitPriceC10 { get; set; } = 1;
        public decimal UnitPriceB1 { get; set; } = 1;
        public decimal? UnitPriceB2 { get; set; } = null;
        public decimal UnitPriceB3 { get; set; } = 1;
        public decimal UnitPriceB4 { get; set; } = 1;
        public decimal UnitPriceB5 { get; set; } = 1;
        public decimal UnitPriceB6 { get; set; } = 1;
        public decimal UnitPriceB7 { get; set; } = 1;
        public decimal UnitPriceB8 { get; set; } = 1;
        public decimal UnitPriceB9 { get; set; } = 1;
        public decimal UnitPriceB10 { get; set; } = 1;
        public decimal UnitPriceA1 { get; set; } = 1;
        public decimal? UnitPriceA2 { get; set; } = null;
        public decimal UnitPriceA3 { get; set; } = 1;
        public decimal UnitPriceA4 { get; set; } = 1;
        public decimal UnitPriceA5 { get; set; } = 1;
        public decimal UnitPriceA6 { get; set; } = 1;
        public decimal UnitPriceA7 { get; set; } = 1;
        public decimal UnitPriceA8 { get; set; } = 1;
        public decimal UnitPriceA9 { get; set; } = 1;
        public decimal UnitPriceA10 { get; set; } = 1;

    }

    public static async Task 测试IE()
    {
        var sw = Stopwatch.StartNew();
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "xxxx");
        var items = new List<Foo>()  ;
        for (int i = 0; i < 20000; i++)
        {
            items.Add(new Foo() { UnitPrice1 =i+1});
        }
        Console.WriteLine("生成数据" + sw.Elapsed.TotalSeconds);
        sw.Restart();


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
