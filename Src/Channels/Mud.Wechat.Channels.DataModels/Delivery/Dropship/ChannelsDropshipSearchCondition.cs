// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>代发单搜索条件（官方 <c>search_condition</c> 对象，<c>dropship/search</c>）。</summary>
/// <remarks>
/// <b>共用裁决（负向）</b>：与供货单搜索条件（<c>supplyorder/search</c> 的 <c>search_condition</c>）
/// 字段集当前一致，但供货侧官方示例已出现表外字段（<c>tel_last4</c>/<c>user_name</c>）预告两形态可独立演化
/// ⇒ 按端点分建，不做跨端点共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipSearchCondition
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，可根据订单号反查代发单）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>，选填，见 <see cref="ChannelsDropshipOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置代发单创建时间范围（官方 <c>create_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置运单号（官方 <c>waybill_id</c>，选填，只可独立查询）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置订单下单时间范围（官方 <c>order_create_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("order_create_time_range")]
    public ChannelsDeliveryTimeRange? OrderCreateTimeRange { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>product_code</c>，选填）。</summary>
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>spu_name</c>，选填）。</summary>
    [JsonPropertyName("spu_name")]
    public string? SpuName { get; set; }

    /// <summary>获取或设置供货商名称（官方 <c>supplier_appname</c>，选填）。</summary>
    [JsonPropertyName("supplier_appname")]
    public string? SupplierAppname { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_notes</c>，选填）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }

    /// <summary>获取或设置代发单更新时间范围（官方 <c>update_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}
