// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

namespace Densen.DataAcces;

/// <summary>
/// 配置类
/// </summary>
public class FreeSqlServiceOptions
{
    public FreeSqlServiceOptions() { }

    public FreeSqlServiceOptions(bool ConfigEntityPropertyImage)
    {
        this.ConfigEntityPropertyImage = ConfigEntityPropertyImage;
    }

    /// <summary>
    /// 获得/设置 自定义Enum支持和Image支持
    /// </summary>
    public bool ConfigEntityPropertyImage { get; set; }
    public bool DistributeTrace { get; set; }
}
