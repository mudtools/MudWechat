// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>优惠券折扣条件（官方 <c>discount_condition</c> 对象）。</summary>
/// <remarks>官方参数表仅标注 <c>product_ids</c> 为 array、未给元素类型；按本线全域商品 id 字符串形态建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCouponDiscountCondition
{
    /// <summary>获取或设置优惠券使用条件——满 x 件商品可用（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }

    /// <summary>获取或设置优惠券使用条件——价格满 x 可用（官方 <c>product_price</c>，单位：分）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>获取或设置优惠券使用条件——指定商品 id 可用（官方 <c>product_ids</c>）。</summary>
    [JsonPropertyName("product_ids")]
    public List<string>? ProductIds { get; set; }
}
