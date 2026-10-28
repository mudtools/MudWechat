// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「营销子域」（8 端点：买赠活动 activity/add、activity/del、activity/stop +
// 限时抢购 limiteddiscounttask/add、update、delete、stop、list/get）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。
//
// 官方文档矛盾照录：activity/del 的 activity_id 为 number、activity/stop 的 activity_id 为 string
// （原文即如此），SDK 分别建模为 long / string，不做统一。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>限时抢购 SKU（官方 <c>limited_discount_skus</c> 数组元素，<c>limiteddiscounttask/add</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsLimitedDiscountSku
{
    /// <summary>获取或设置参与抢购的商品 ID 下，不同规格（SKU）的商品信息（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 SKU 的抢购价格（官方 <c>sale_price</c>，必填，必须小于原价；原价为 1 分钱的商品无法创建抢购任务）。
    /// </summary>
    [JsonPropertyName("sale_price")]
    public long SalePrice { get; set; }

    /// <summary>获取或设置参与抢购的商品库存（官方 <c>sale_stock</c>，必填，必须小于等于现有库存）。</summary>
    [JsonPropertyName("sale_stock")]
    public long SaleStock { get; set; }
}
