// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
//using Magicodes.ExporterAndImporter.Excel;
//using OfficeOpenXml.Table;
using System.ComponentModel;

namespace BlazorApp1.Data;

//[ExcelImporter(IsLabelingError = true)]
//[ExcelExporter(Name = "导入商品中间表", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)]
public class WeatherForecast
{
    [Column(IsIdentity = true)]
    [DisplayName("序号")]
    public int ID { get; set; }

    [DisplayName("日期")]
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    //[DisplayName("备注")]
    //public string? Remarks { get; set; }

    [DisplayName("备注表")]
    [Navigate("ID")]
    public virtual Remarks? Remark { get; set; }
    //在 本实体 查找 ID 属性，与 Remarks 主键 关联
}
