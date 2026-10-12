// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>收货/发货地址信息（官方 <c>address_info</c> 与 <c>delivery_address</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <para>
/// <b>共用 DTO 裁决</b>：①官方原文「<c>delivery_address</c> 字段与 <c>address_info</c> 相同」⇒ 同一 DTO；
/// ②代发单与供货单两侧地址字段集一致（供货单参数表未列 <c>national_code</c>，官方示例包含且代发侧
/// 参数表声明「已废弃」⇒ 统一登记，供旧报文兼容）⇒ 共用本 DTO。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryAddress
{
    /// <summary>获取或设置收货人姓名（官方 <c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置邮编（官方 <c>postal_code</c>）。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>获取或设置省份（官方 <c>province_name</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置城市（官方 <c>city_name</c>）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置区（官方 <c>county_name</c>）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail_info</c>）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>获取或设置国家码（官方 <c>national_code</c>，官方已废弃，请勿使用）。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>获取或设置联系方式（官方 <c>tel_number</c>）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置门牌号码（官方 <c>house_number</c>）。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }

    /// <summary>获取或设置虚拟发货订单联系方式（官方 <c>virtual_order_tel_number</c>，<c>deliver_method = 1</c> 时返回）。</summary>
    [JsonPropertyName("virtual_order_tel_number")]
    public string? VirtualOrderTelNumber { get; set; }

    /// <summary>获取或设置额外的联系方式信息（官方 <c>tel_number_ext_info</c>，虚拟号码相关）。</summary>
    [JsonPropertyName("tel_number_ext_info")]
    public ChannelsTelNumberExtInfo? TelNumberExtInfo { get; set; }

    /// <summary>获取或设置是否使用虚拟号码（官方 <c>use_tel_number</c>：0 不使用，1 使用）。</summary>
    [JsonPropertyName("use_tel_number")]
    public int? UseTelNumber { get; set; }

    /// <summary>获取或设置标识当前店铺下一个唯一的用户收货地址（官方 <c>hash_code</c>）。</summary>
    [JsonPropertyName("hash_code")]
    public string? HashCode { get; set; }
}
