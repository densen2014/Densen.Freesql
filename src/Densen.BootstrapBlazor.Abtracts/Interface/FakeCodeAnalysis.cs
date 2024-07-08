// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************


#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER
namespace BootstrapBlazor.Components;

//
// 摘要:
//     Specifies that an output is not null even if the corresponding type allows it.
//     Specifies that an input argument was not null when the call returns.
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
public sealed class NotNullAttribute : Attribute
{
    //
    // 摘要:
    //     Initializes a new instance of the System.Diagnostics.CodeAnalysis.NotNullAttribute
    //     class.
    //public NotNullAttribute();
}
#endif
