// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟商品活动信息（官方 <c>product/getactivity</c> 的 <c>product_activity_list</c> 数组元素）。</summary>
/// <remarks>
/// <para>官方约束：<c>activity_sub_type</c> 目前仅支持 <c>3</c>（品牌日）与 <c>4</c>（通用货补），其他活动类型不返回；
/// 官方变更日志 2026-03-27 优化库存描述 ⇒ <c>stock_mode</c> 为 <c>0</c> 活动库存、<c>1</c> 商品自营库存。</para>
/// <para>与小店商家侧的限时折扣 / 赠品活动 DTO（Product 域 <c>Activity/</c>）字段集完全不同 ⇒ 分建。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProductActivity
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置 sku ID（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>获取或设置活动价（官方 <c>activity_price</c>，单位：分）。</summary>
    [JsonPropertyName("activity_price")]
    public long? ActivityPrice { get; set; }

    /// <summary>获取或设置平台补贴金额（官方 <c>platform_subsidy</c>，单位：分）。</summary>
    [JsonPropertyName("platform_subsidy")]
    public long? PlatformSubsidy { get; set; }

    /// <summary>获取或设置平台补贴件数（官方 <c>platform_subsidy_cnt</c>）。</summary>
    [JsonPropertyName("platform_subsidy_cnt")]
    public long? PlatformSubsidyCnt { get; set; }

    /// <summary>获取或设置活动子类型（官方 <c>activity_sub_type</c>：3 品牌日 / 4 通用货补）。</summary>
    [JsonPropertyName("activity_sub_type")]
    public int? ActivitySubType { get; set; }

    /// <summary>获取或设置活动结束时间（官方 <c>activity_end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("activity_end_time")]
    public long? ActivityEndTime { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>activity_name</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>获取或设置库存模式（官方 <c>stock_mode</c>：0 活动库存 / 1 商品自营库存）。</summary>
    [JsonPropertyName("stock_mode")]
    public int? StockMode { get; set; }

    /// <summary>获取或设置库存（官方 <c>stock_number</c>）。</summary>
    [JsonPropertyName("stock_number")]
    public long? StockNumber { get; set; }
}
