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

/// <summary>订单搜索条件（官方 <c>order/search</c> 的 <c>search_condition</c> 对象）。</summary>
/// <remarks>官方要求「搜索条件下的参数必须至少设置一个字段，否则接口报错」（守卫锁定）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSearchOrderCondition
{
    /// <summary>获取或设置售后存在性过滤（官方 <c>on_aftersale_order_exist</c>：0=无售后订单，1=有售后订单）。</summary>
    [JsonPropertyName("on_aftersale_order_exist")]
    public int? OnAftersaleOrderExist { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置商品标题关键词（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置收件人（官方 <c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>
    /// 获取或设置收件人电话（官方 <c>tel_number</c>，【已废弃】请使用 <see cref="TelNumberLast4"/>）。
    /// </summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，只搜一个订单时使用）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置买家备注（官方 <c>customer_notes</c>）。</summary>
    [JsonPropertyName("customer_notes")]
    public string? CustomerNotes { get; set; }

    /// <summary>获取或设置收件人电话后四位（官方 <c>tel_number_last4</c>）。</summary>
    [JsonPropertyName("tel_number_last4")]
    public string? TelNumberLast4 { get; set; }

    /// <summary>获取或设置申请修改地址审核中（官方 <c>address_under_review</c>）。</summary>
    [JsonPropertyName("address_under_review")]
    public bool? AddressUnderReview { get; set; }

    /// <summary>获取或设置礼物单号（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }
}
