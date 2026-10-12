// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「订单主文档」DTO（order/get、order/list/get、order/search、order/price/update、
// order/address/update、order/addressmodify/*、order/merchantnotes/update、order/presentnote/add、
// order/presentsuborder/get、order/preshipmentchangesku/*、order/blindboxitem/set 等核心端点）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>收货地址（官方 <c>order/address/update</c> 的 <c>user_address</c> 与
/// <c>sensitiveinfo/decode</c> 的 <c>address_info</c> 共用字段集）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderAddress
{
    /// <summary>获取或设置收货人姓名（官方 <c>user_name</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置邮编（官方 <c>postal_code</c>）。</summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>获取或设置省份（官方 <c>province_name</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置城市（官方 <c>city_name</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置区（官方 <c>county_name</c>）。</summary>
    [JsonPropertyName("county_name")]
    public string? CountyName { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail_info</c>，订单 deliver_method=0 必填）。</summary>
    [JsonPropertyName("detail_info")]
    public string? DetailInfo { get; set; }

    /// <summary>获取或设置国家码（官方 <c>national_code</c>）。</summary>
    [JsonPropertyName("national_code")]
    public string? NationalCode { get; set; }

    /// <summary>获取或设置普通订单联系方式（官方 <c>tel_number</c>，订单 deliver_method=0 必填；虚拟号场景为脱敏号码）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置门牌号码（官方 <c>house_number</c>）。</summary>
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }

    /// <summary>获取或设置虚拟商品订单联系方式（官方 <c>virtual_order_tel_number</c>，虚拟商品订单必填，deliver_method=1）。</summary>
    [JsonPropertyName("virtual_order_tel_number")]
    public string? VirtualOrderTelNumber { get; set; }
}
