// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 收货 / 发货地址（V1 <c>address_info</c>，官方 <c>AddressInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>经纬度是可选增强</b>：<c>lat</c> / <c>lng</c> 仅在地址侧出现，订单详情内的地址结构不含它们</para>
/// <para>⇒ 本类与 <see cref="WxaProductOrderAddressInfo"/> 分开建模。</para>
/// <para><b>敏感值</b>：姓名、电话、详细地址属个人信息 ⇒ 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductAddressInfo
{
    /// <summary>联系人姓名（必填，敏感）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>邮编。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>国标收货地址第一级地址（必填）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>国标收货地址第二级地址（必填）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>国标收货地址第三级地址（必填）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>详细地址（必填，敏感）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>收货地址国家码。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>收件人手机号（敏感）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>纬度。</summary>
    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    /// <summary>经度。</summary>
    [JsonPropertyName("lng")]
    public double? Lng { get; set; }

    /// <summary>门牌号。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }
}
