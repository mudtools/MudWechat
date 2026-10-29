// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>电子面单取号 / 预取号请求共用形态（<c>ewaybill/biz/order/create</c> / <c>precreate</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillCreateOrderRequest
{
    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>，全局唯一 id；create 时从预取号获取或自定义，数据内容要求 Uint64 以 String 传递）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填，经获取快递公司列表查询）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置网点编码（官方 <c>site_code</c>）。</summary>
    [JsonPropertyName("site_code")]
    public string? SiteCode { get; set; }

    /// <summary>获取或设置物流账号编码（官方 <c>ewaybill_acct_id</c>，必填，从 ewaybill_getacct 获取）。</summary>
    [JsonPropertyName("ewaybill_acct_id")]
    public string EwaybillAcctId { get; set; } = string.Empty;

    /// <summary>获取或设置寄件人（官方 <c>sender</c>，必填，传明文）。</summary>
    [JsonPropertyName("sender")]
    public ChannelsLogisticsEwaybillAddress? Sender { get; set; }

    /// <summary>获取或设置收件人（官方 <c>receiver</c>，必填，传小店订单内用户信息即可；供货商代发单无需上传）。</summary>
    [JsonPropertyName("receiver")]
    public ChannelsLogisticsEwaybillAddress? Receiver { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>ec_order_list</c>，必填）。</summary>
    [JsonPropertyName("ec_order_list")]
    public List<ChannelsLogisticsEwaybillEcOrder>? EcOrderList { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置面单主体 ID（官方 <c>shop_id</c>，必填，每个商家分配的唯一 shop_id）。</summary>
    [JsonPropertyName("shop_id")]
    public string ShopId { get; set; } = string.Empty;

    /// <summary>获取或设置退货地址（官方 <c>return_address</c>，create 专用）。</summary>
    [JsonPropertyName("return_address")]
    public ChannelsLogisticsEwaybillAddress? ReturnAddress { get; set; }

    /// <summary>获取或设置模板 id（官方 <c>template_id</c>，如需获取打印报文则填；无需后台模板可直接传 template_type 如 'single'）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置支持的类型（官方 <c>order_type</c>，枚举默认为 1，加盟型可不填）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置保价等增值服务（官方 <c>order_vas_list</c>）。</summary>
    [JsonPropertyName("order_vas_list")]
    public List<ChannelsLogisticsEwaybillVas>? OrderVasList { get; set; }

    /// <summary>获取或设置温层等补充字段（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsLogisticsEwaybillExtInfo? ExtInfo { get; set; }

    /// <summary>获取或设置预约上门取件、子母件等发货信息字段（官方 <c>delivery_info</c>）。</summary>
    [JsonPropertyName("delivery_info")]
    public ChannelsLogisticsEwaybillDeliveryInfo? DeliveryInfo { get; set; }
}
