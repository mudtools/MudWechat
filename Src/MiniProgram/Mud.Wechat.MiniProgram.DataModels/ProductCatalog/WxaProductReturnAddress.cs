// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 收货 / 公司地址（官方 <c>address_info</c> / <c>company_address</c>）。
/// </summary>
/// <remarks>
/// <para>省 / 市 / 区必填，<c>detail_info</c> 选填；含收件人姓名与电话，属敏感个人信息，不得写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductReturnAddress
{
    /// <summary>收货人姓名（敏感）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>邮政编码。</summary>
    [JsonPropertyName("postal_code")]
    public int? PostalCode { get; set; }

    /// <summary>省份（如 广东省 北京市）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>城市（如 广州市）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>区县（如 海珠区）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>详细地址。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>国家码。</summary>
    [JsonPropertyName("national_code")]
    public int? NationalCode { get; set; }

    /// <summary>电话号（敏感）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }
}
