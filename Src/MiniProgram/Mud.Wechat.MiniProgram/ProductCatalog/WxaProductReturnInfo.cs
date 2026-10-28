// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 退货地址信息（官方 <c>return_info</c>）。
/// </summary>
/// <remarks>
/// <para>含退货地址与公司地址，均携带收件人姓名 / 电话 ⇒ 敏感个人信息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductReturnInfo
{
    /// <summary>退货地址（必填）。</summary>
    [JsonPropertyName("address_info")]
    public WxaProductReturnAddress? AddressInfo { get; set; }

    /// <summary>邮箱（必填）。</summary>
    [JsonPropertyName("mail")]
    public string? Mail { get; set; }

    /// <summary>公司地址（必填，结构同退货地址）。</summary>
    [JsonPropertyName("company_address")]
    public WxaProductReturnAddress? CompanyAddress { get; set; }
}
