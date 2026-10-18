// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 商家默认退货地址（官方 <c>Address</c> / <c>AddressInfo</c> 结构体，读写两侧同形）。
/// </summary>
/// <remarks>
/// <para><b>必填集在读写两侧不同</b>（官方逐页参数表原文差异，本类只建模字段、不复述必填集）：
/// 请求侧（<c>account/update_info</c>）<c>province</c> / <c>city</c> / <c>town</c> 标「是」，
/// 应答侧（<c>account/get_info</c>）同三支标「否」⇒ 读回的对象可能缺这三段，宿主不得假定回读即回显。</para>
/// <para><b><c>town</c> 语义两页不一致</b>：官方 get_info 说明为「乡镇」、update_info 示例注释为「区县」
/// （键名恒为 <c>town</c>）⇒ 照原文保留键名，语义差异只在注释留档，不做归一。</para>
/// <para><b>敏感面</b>：<c>receiver_name</c>（收件人姓名）/ <c>tel_number</c>（收件人手机号）/
/// <c>detailed_address</c>（详细地址）为个人信息，<b>不得</b>写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAddressInfo
{
    /// <summary>收件人姓名（<c>receiver_name</c>，string；读写两侧均必填）。敏感，禁落日志。</summary>
    [JsonPropertyName("receiver_name")]
    public string? ReceiverName { get; set; }

    /// <summary>详细收货地址（<c>detailed_address</c>，string；读写两侧均必填）。敏感，禁落日志。</summary>
    [JsonPropertyName("detailed_address")]
    public string? DetailedAddress { get; set; }

    /// <summary>收件人手机号（<c>tel_number</c>，string；读写两侧均必填）。敏感，禁落日志。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>国家（<c>country</c>，string；两侧均为选填）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>省份（<c>province</c>，string；写侧必填、读侧标选填）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>城市（<c>city</c>，string；写侧必填、读侧标选填）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>乡镇 / 区县（<c>town</c>，string；写侧必填、读侧标选填，两页语义描述不一致）。</summary>
    [JsonPropertyName("town")]
    public string? Town { get; set; }
}
