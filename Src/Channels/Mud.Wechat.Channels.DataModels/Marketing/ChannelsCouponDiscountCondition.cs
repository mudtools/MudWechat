// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>优惠券优惠门槛（官方 <c>discount_condition</c> 对象）。</summary>
/// <remarks>官方契约：<c>product_cnt</c> 与 <c>product_price</c> 不能同时设置；商品券 <c>product_ids</c> 必填，最多 200 个。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponDiscountCondition
{
    /// <summary>获取或设置商品件数门槛（官方 <c>product_cnt</c>，选填，不能和价格门槛同时设置）。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }

    /// <summary>获取或设置商品价格门槛（官方 <c>product_price</c>，选填，单位：分，不能和件数门槛同时设置）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>获取或设置商品 id 列表（官方 <c>product_ids</c>，选填；商品券必填，最多 200 个）。</summary>
    [JsonPropertyName("product_ids")]
    public List<string>? ProductIds { get; set; }
}
