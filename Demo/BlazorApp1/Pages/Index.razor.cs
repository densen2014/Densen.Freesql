// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AmeBlazor.Components;
using BootstrapBlazor.Components;
using Densen.Models.ids;

namespace BlazorApp1.Pages;

public partial class Index
{
    List<RowButtonField>? list;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        list = list?? new List<RowButtonField>(){
                            new RowButtonField() {
                                Title ="例1",
                                Color = Color.Success,
                                CallbackFunc =  升级,
                            },
                            new RowButtonField() {
                                Title ="例2" ,
                                Color = Color.Info,
                                CallbackFunc =  升级,
                            }
                        };
    }

    private async Task<bool> 升级(object obj)
    {
        var item = obj as AspNetRoles;
        await ToastService.Information(obj.ToString(), item?.Name);
        return true;
    }


}
