// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>网点地址信息（官方 <c>account_list[].site_info.address</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSiteAddress
{
    /// <summary>获取或设置城市编码（官方 <c>city_code</c>）。</summary>
    [JsonPropertyName("city_code")]
    public string? CityCode { get; set; }

    /// <summary>获取或设置城市名（官方 <c>city_name</c>）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置国家编码（官方 <c>country_code</c>）。</summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail_address</c>）。</summary>
    [JsonPropertyName("detail_address")]
    public string? DetailAddress { get; set; }

    /// <summary>获取或设置区县编码（官方 <c>district_code</c>）。</summary>
    [JsonPropertyName("district_code")]
    public string? DistrictCode { get; set; }

    /// <summary>获取或设置区县名（官方 <c>district_name</c>）。</summary>
    [JsonPropertyName("district_name")]
    public string? DistrictName { get; set; }

    /// <summary>获取或设置省编码（官方 <c>province_code</c>）。</summary>
    [JsonPropertyName("province_code")]
    public string? ProvinceCode { get; set; }

    /// <summary>获取或设置省份名（官方 <c>province_name</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置街道编码（官方 <c>street_code</c>）。</summary>
    [JsonPropertyName("street_code")]
    public string? StreetCode { get; set; }

    /// <summary>获取或设置街道名（官方 <c>street_name</c>）。</summary>
    [JsonPropertyName("street_name")]
    public string? StreetName { get; set; }
}
