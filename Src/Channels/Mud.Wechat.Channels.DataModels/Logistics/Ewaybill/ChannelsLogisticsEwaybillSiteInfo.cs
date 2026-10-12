// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>网点信息（官方 <c>account_list[].site_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSiteInfo
{
    /// <summary>获取或设置快递公司编码（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置网点运营状态（官方 <c>site_status</c>）：1 正常；其他值则不正常。</summary>
    [JsonPropertyName("site_status")]
    public int? SiteStatus { get; set; }

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置网点名字（官方 <c>site_name</c>）。</summary>
    [JsonPropertyName("site_name")]
    public string? SiteName { get; set; }

    /// <summary>获取或设置地址信息（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public ChannelsLogisticsEwaybillSiteAddress? Address { get; set; }

    /// <summary>获取或设置联系信息（官方 <c>contact</c>）。</summary>
    [JsonPropertyName("contact")]
    public ChannelsLogisticsEwaybillSiteContact? Contact { get; set; }

    /// <summary>获取或设置地址全名（官方 <c>site_fullname</c>）。</summary>
    [JsonPropertyName("site_fullname")]
    public string? SiteFullname { get; set; }
}
