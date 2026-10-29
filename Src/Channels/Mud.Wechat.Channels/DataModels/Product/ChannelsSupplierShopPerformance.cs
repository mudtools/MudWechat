// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>货主店铺经营表现（官方 <c>supplier_shop_performance</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSupplierShopPerformance
{
    /// <summary>获取或设置 24h 揽收率（官方 <c>accept_rate_24h</c>）。</summary>
    [JsonPropertyName("accept_rate_24h")]
    public long? AcceptRate24h { get; set; }

    /// <summary>获取或设置 48h 揽收率（官方 <c>accept_rate_48h</c>）。</summary>
    [JsonPropertyName("accept_rate_48h")]
    public long? AcceptRate48h { get; set; }

    /// <summary>获取或设置近 30 天代发数量（官方 <c>ship_count_30d</c>）。</summary>
    [JsonPropertyName("ship_count_30d")]
    public long? ShipCount30d { get; set; }

    /// <summary>获取或设置近 7 天代发数量（官方 <c>ship_count_7d</c>）。</summary>
    [JsonPropertyName("ship_count_7d")]
    public long? ShipCount7d { get; set; }

    /// <summary>获取或设置铺货分销商数量（官方 <c>distribute_product_count</c>）。</summary>
    [JsonPropertyName("distribute_product_count")]
    public long? DistributeProductCount { get; set; }

    /// <summary>获取或设置店铺准时发货率（官方 <c>on_time_ship_rate</c>）。</summary>
    [JsonPropertyName("on_time_ship_rate")]
    public long? OnTimeShipRate { get; set; }

    /// <summary>获取或设置商家身份（官方 <c>merchant_identity</c>）。</summary>
    [JsonPropertyName("merchant_identity")]
    public string? MerchantIdentity { get; set; }
}
