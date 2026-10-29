// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>共享账号发起方信息（官方 <c>account_list[].share</c>，acct_type 为共享账号时有效）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillShare
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置网点名字（官方 <c>site_name</c>）。</summary>
    [JsonPropertyName("site_name")]
    public string? SiteName { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>acct_id</c>）。</summary>
    [JsonPropertyName("acct_id")]
    public string? AcctId { get; set; }

    /// <summary>获取或设置发起共享方店铺名（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置共享 id（官方 <c>share_id</c>）。</summary>
    [JsonPropertyName("share_id")]
    public string? ShareId { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>）。</summary>
    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    /// <summary>获取或设置直营型月结账号（官方 <c>monthly_card</c>）。</summary>
    [JsonPropertyName("monthly_card")]
    public string? MonthlyCard { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}
