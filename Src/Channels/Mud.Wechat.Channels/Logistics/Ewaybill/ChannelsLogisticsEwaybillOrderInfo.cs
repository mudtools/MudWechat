// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>电子面单详情（官方 <c>order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillOrderInfo
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>ewaybill_acct_id</c>）。</summary>
    [JsonPropertyName("ewaybill_acct_id")]
    public string? EwaybillAcctId { get; set; }

    /// <summary>获取或设置寄件人（官方 <c>sender</c>）。</summary>
    [JsonPropertyName("sender")]
    public ChannelsLogisticsEwaybillAddress? Sender { get; set; }

    /// <summary>获取或设置收件人（官方 <c>receiver</c>）。</summary>
    [JsonPropertyName("receiver")]
    public ChannelsLogisticsEwaybillAddress? Receiver { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>ec_order_list</c>）。</summary>
    [JsonPropertyName("ec_order_list")]
    public List<ChannelsLogisticsEwaybillEcOrder>? EcOrderList { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>）。</summary>
    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置电子面单状态（官方 <c>status</c>）：1 下单成功；2 取消；3 已打单；4 已发货。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置轨迹信息（官方 <c>path_info</c>）。</summary>
    [JsonPropertyName("path_info")]
    public List<ChannelsLogisticsEwaybillPath>? PathInfo { get; set; }

    /// <summary>获取或设置订单类型（官方 <c>order_type</c>）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置月结账号（官方 <c>monthly_card</c>）。</summary>
    [JsonPropertyName("monthly_card")]
    public string? MonthlyCard { get; set; }

    /// <summary>获取或设置保价等增值服务（官方 <c>order_vas_list</c>）。</summary>
    [JsonPropertyName("order_vas_list")]
    public List<ChannelsLogisticsEwaybillVas>? OrderVasList { get; set; }

    /// <summary>获取或设置温层等补充字段（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsLogisticsEwaybillExtInfo? ExtInfo { get; set; }
}
