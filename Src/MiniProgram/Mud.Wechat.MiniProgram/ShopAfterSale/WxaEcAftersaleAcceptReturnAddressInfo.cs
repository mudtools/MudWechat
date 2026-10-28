// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 同意退货的收货地址（<c>acceptreturn</c> 专用，区县键为 <c>town</c>）。
/// </summary>
/// <remarks>
/// <para><b>必填集</b>：<c>receiver_name</c> / <c>detailed_address</c> / <c>tel_number</c> / <c>province</c> /</para>
/// <para><c>city</c> / <c>town</c> 均必填，仅 <c>country</c> 选填。</para>
/// <para><b>敏感值</b>：姓名、电话、详细地址禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleAcceptReturnAddressInfo
{
    /// <summary>收件人姓名（必填，敏感）。</summary>
    [JsonPropertyName("receiver_name")]
    public string? ReceiverName { get; set; }

    /// <summary>详细收货地址（必填，敏感）。</summary>
    [JsonPropertyName("detailed_address")]
    public string? DetailedAddress { get; set; }

    /// <summary>收件人电话（必填，敏感）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>国家（选填）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>省份（必填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>城市（必填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>区县（必填）。</summary>
    [JsonPropertyName("town")]
    public string? Town { get; set; }
}
