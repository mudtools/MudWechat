// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 供货商视角搜索代发单条件（官方 <c>search_condition</c>，<c>dropship/supplier/search</c>）。
/// </summary>
/// <remarks>
/// 与商家侧 <see cref="ChannelsDropshipSearchCondition"/> 的官方参数表差异：本视角为 <c>shop_appid</c>/<c>shop_appname</c>
/// （商家侧为 <c>supplier_appid</c>/<c>supplier_appname</c>）⇒ 各建一型。<c>waybill_id</c> 官方限定「只可独立查询」，
/// SDK 不拦截该组合，由官方错误码表达。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDropshipSearchCondition
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，可根据订单号反查代发单）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置商家 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppid { get; set; }

    /// <summary>获取或设置商家名称（官方 <c>shop_appname</c>）。</summary>
    [JsonPropertyName("shop_appname")]
    public string? ShopAppname { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>：100 待发货 / 199 部分发货 / 200 已发货 / 300 已取消）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置代发单创建时间范围（官方 <c>create_time_range</c>，秒级时间戳，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置运单号（官方 <c>waybill_id</c>，<b>只可独立查询</b>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置订单下单时间范围（官方 <c>order_create_time_range</c>，秒级时间戳，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("order_create_time_range")]
    public ChannelsDeliveryTimeRange? OrderCreateTimeRange { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>product_code</c>）。</summary>
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>spu_name</c>）。</summary>
    [JsonPropertyName("spu_name")]
    public string? SpuName { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置代发单更新时间范围（官方 <c>update_time_range</c>，秒级时间戳，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}
