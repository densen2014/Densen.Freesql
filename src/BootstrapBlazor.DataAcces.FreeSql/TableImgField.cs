// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;

namespace AmeBlazor.Components;

/// <summary>
/// 附加行内操作按钮
/// </summary>
public class RowButtonField
{
    public string? Title { get; set; }

    public Color Color { get; set; } = Color.Primary;

    public string Icon { get; set; } = "fa-solid fa-pen";
    /// <summary>
    /// 获得/设置 识别完成回调方法,返回 Model 集合
    /// </summary>
    public Func<object, Task>? CallbackFunc { get; set; }
}
