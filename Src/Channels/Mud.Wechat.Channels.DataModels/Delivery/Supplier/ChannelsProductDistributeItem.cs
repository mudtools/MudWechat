// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>按商品自动分配设置项（官方 <c>list</c> 数组元素，<c>set_product_distribute</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsProductDistributeItem
{
    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，必填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string SupplierAppid { get; set; } = string.Empty;

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>
    /// 获取或设置该商品是否自动分配给该供货商（官方 <c>is_distribute</c>，必填）——
    /// 官方原文：当 <c>is_distribute = false</c> 时取消分配给供货商。
    /// </summary>
    [JsonPropertyName("is_distribute")]
    public bool IsDistribute { get; set; }

    /// <summary>
    /// 获取或设置主赠品同步模式（官方 <c>apply_to_gift</c>，选填，默认同步）——
    /// 官方原文：当 <c>apply_to_gift = true</c> 时强制同步主品自动分配关系给赠品；
    /// 此参数不填时不变更主赠品同步模式。单独为赠品设置代发关系时须设为 <c>false</c>。
    /// </summary>
    [JsonPropertyName("apply_to_gift")]
    public bool? ApplyToGift { get; set; }
}
