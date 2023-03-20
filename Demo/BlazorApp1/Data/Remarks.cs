using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;
using System.ComponentModel;

namespace BlazorApp1.Data;

[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)]
public class Remarks
{
    [DisplayName("序号")]
    public int ID { get; set; }

    public string? Remark { get; set; }
}

class Cagetory
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid ParentId { get; set; }

    [Column (IsIgnore = true)]
    public string SubName {
        get=> subName??(Childs?.FirstOrDefault()?.Name??"");
        set{
            subName = value;
            if (Childs?.FirstOrDefault()!=null) Childs.FirstOrDefault().Name = value;
        }
    }
    string subName;

    [Navigate(nameof(ParentId))]
    public List<Cagetory> Childs { get; set; }

}
