// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 订单内地址（官方 <c>AddressInfo</c>，无经纬度）。
/// </summary>
/// <remarks>
/// <para><b>与地址簿结构不同</b>：本结构<b>不含</b> <c>lat</c> / <c>lng</c>，</para>
/// <para>与 <see cref="WxaProductAddressInfo"/> 分开建模。</para>
/// <para><b>敏感值</b>：姓名 / 电话 / 详细地址禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderAddressInfo
{
    /// <summary>收件人姓名（敏感）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>邮编。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>省份。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>城市。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>区县。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>详细地址（敏感）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>收货地址国家码。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>收件人手机号（敏感）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>门牌号。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }
}
