// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 提交基础信息请求体（<c>POST /product/register/submit_basicinfo</c>）。
/// </summary>
/// <remarks>
/// <para>三支顶层键全必填；<c>return_info</c> 含收件人姓名与电话，属敏感个人信息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductBasicInfoRequest
{
    /// <summary>小程序 ID（必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>名称信息（必填）。</summary>
    [JsonPropertyName("name_info")]
    public WxaProductNameInfo? NameInfo { get; set; }

    /// <summary>退货地址（必填）。</summary>
    [JsonPropertyName("return_info")]
    public WxaProductReturnInfo? ReturnInfo { get; set; }
}
