// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 预录入门店信息的单条记录（<c>batchcreateretail</c> 请求数组元素）。
/// </summary>
/// <remarks>
/// <para><b>条件必填</b>：<c>retail_type</c> 选「其他」时 <c>sub_retail_type</c> 必填；
/// <c>business_type</c> 含「其他」时 <c>other_business_type</c> 必填。</para>
/// <para><b>敏感面</b>：手机号、统一社会信用代码与法人信息一律禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessRetailInfoCreate
{
    /// <summary>门店联系人手机号（必填）。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }

    /// <summary>门店名称（必填，1~100 字符）。</summary>
    [JsonPropertyName("retail_name")]
    public string? RetailName { get; set; }

    /// <summary>门店类型（选填，官方未发布枚举）。</summary>
    [JsonPropertyName("retail_type")]
    public string? RetailType { get; set; }

    /// <summary>门店子类型（<c>retail_type</c> 为「其他」时必填）。</summary>
    [JsonPropertyName("sub_retail_type")]
    public string? SubRetailType { get; set; }

    /// <summary>门店地址·省（必填）。</summary>
    [JsonPropertyName("address_province")]
    public string? AddressProvince { get; set; }

    /// <summary>门店地址·市（必填）。</summary>
    [JsonPropertyName("address_city")]
    public string? AddressCity { get; set; }

    /// <summary>门店地址·区/县（必填）。</summary>
    [JsonPropertyName("address_region")]
    public string? AddressRegion { get; set; }

    /// <summary>门店地址·街道与门牌（必填）。</summary>
    [JsonPropertyName("address_street")]
    public string? AddressStreet { get; set; }

    /// <summary>统一社会信用代码（选填，敏感）。</summary>
    [JsonPropertyName("registration_number")]
    public string? RegistrationNumber { get; set; }

    /// <summary>营业执照名称（选填）。</summary>
    [JsonPropertyName("biz_name")]
    public string? BizName { get; set; }

    /// <summary>法人姓名（选填，敏感）。</summary>
    [JsonPropertyName("corporation_name")]
    public string? CorporationName { get; set; }

    /// <summary>纬度（选填，官方为浮点数）。</summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>经度（选填，官方为浮点数）。</summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>经营类目（选填，官方未发布枚举）。</summary>
    [JsonPropertyName("business_type")]
    public List<int>? BusinessType { get; set; }

    /// <summary>其它经营类目（<c>business_type</c> 含「其他」时必填）。</summary>
    [JsonPropertyName("other_business_type")]
    public string? OtherBusinessType { get; set; }
}
