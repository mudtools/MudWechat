// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>面单账号信息（官方 <c>account_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillAccount
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置面单账号类型（官方 <c>acct_type</c>）：0 普通账号；1 共享账号。</summary>
    [JsonPropertyName("acct_type")]
    public int? AcctType { get; set; }

    /// <summary>获取或设置快递公司类型（官方 <c>company_type</c>）：1 加盟型；2 直营型。</summary>
    [JsonPropertyName("company_type")]
    public int? CompanyType { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>）。</summary>
    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>acct_id</c>，每绑定一个物流商网点/月结账号分配一个）。</summary>
    [JsonPropertyName("acct_id")]
    public string? AcctId { get; set; }

    /// <summary>获取或设置面单账号状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置面单余额（官方 <c>available</c>）。</summary>
    [JsonPropertyName("available")]
    public long? Available { get; set; }

    /// <summary>获取或设置累积已取单（官方 <c>allocated</c>）。</summary>
    [JsonPropertyName("allocated")]
    public long? Allocated { get; set; }

    /// <summary>获取或设置累积已回收（官方 <c>recycled</c>）。</summary>
    [JsonPropertyName("recycled")]
    public long? Recycled { get; set; }

    /// <summary>获取或设置累计已取消（官方 <c>cancel</c>）。</summary>
    [JsonPropertyName("cancel")]
    public long? Cancel { get; set; }

    /// <summary>获取或设置月结账号（官方 <c>monthly_card</c>，company_type 为直营型时有效）。</summary>
    [JsonPropertyName("monthly_card")]
    public string? MonthlyCard { get; set; }

    /// <summary>获取或设置网点信息（官方 <c>site_info</c>）。</summary>
    [JsonPropertyName("site_info")]
    public ChannelsLogisticsEwaybillSiteInfo? SiteInfo { get; set; }

    /// <summary>获取或设置共享账号发起方信息（官方 <c>share</c>，acct_type 为共享账号时有效）。</summary>
    [JsonPropertyName("share")]
    public ChannelsLogisticsEwaybillShare? Share { get; set; }

    /// <summary>获取或设置绑定的发货地址信息（官方 <c>sender_address</c>）。</summary>
    [JsonPropertyName("sender_address")]
    public ChannelsLogisticsEwaybillSenderAddress? SenderAddress { get; set; }

    /// <summary>获取或设置查询库存的返回码（官方 <c>balance_retcode</c>）。</summary>
    [JsonPropertyName("balance_retcode")]
    public int? BalanceRetcode { get; set; }

    /// <summary>获取或设置查询库存的返回信息（官方 <c>balance_retmsg</c>）。</summary>
    [JsonPropertyName("balance_retmsg")]
    public string? BalanceRetmsg { get; set; }

    /// <summary>获取或设置查询库存失败时快递公司返回的详情（官方 <c>delivery_msg</c>）。</summary>
    [JsonPropertyName("delivery_msg")]
    public string? DeliveryMsg { get; set; }
}
