// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 地址信息（官方 <c>AddressInfo</c> 表；收货地址、默认退货地址与写侧地址三处共用同一类型）。
/// </summary>
/// <remarks>
/// <para><b>本类型整体属敏感个人信息</b>：<c>receiver_name</c>（姓名）、<c>tel_number</c>（手机号）、
/// <c>detailed_address</c>（详细住址）三支组合可定位到具体自然人，
/// <b>不得写入日志、遥测或异常消息</b>，也不得进埋点属性或链路标签。</para>
/// <para><b>必填集由配送方式决定</b>：官方写侧表把姓名 / 详细地址 / 手机号列成必填，四级区划（国家、省、市、乡镇）为选填。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopAddressInfo
{
    /// <summary>receiver_name（官方类型 <c>string</c>）：收件人姓名（敏感个人信息，不得进日志）。</summary>
    [JsonPropertyName("receiver_name")]
    public string? ReceiverName { get; set; }

    /// <summary>detailed_address（官方类型 <c>string</c>）：详细收货地址信息（敏感个人信息，不得进日志）。</summary>
    [JsonPropertyName("detailed_address")]
    public string? DetailedAddress { get; set; }

    /// <summary>tel_number（官方类型 <c>string</c>）：收件人手机号码（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>country（官方类型 <c>string</c>）：国家。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>province（官方类型 <c>string</c>）：省份。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>city（官方类型 <c>string</c>）：城市。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>town（官方类型 <c>string</c>）：乡镇。</summary>
    [JsonPropertyName("town")]
    public string? Town { get; set; }
}
