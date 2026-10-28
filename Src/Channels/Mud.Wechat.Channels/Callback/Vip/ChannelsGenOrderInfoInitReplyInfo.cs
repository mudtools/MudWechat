// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>「下单页获取初始会员权益」应答的 info 结构（官方字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsGenOrderInfoInitReplyInfo
{
    /// <summary>获取或设置会员优惠总金额（分）。</summary>
    [JsonPropertyName("vip_discounted_price")]
    public long VipDiscountedPrice { get; set; }

    /// <summary>获取或设置商品列表（逐商品拆解会员优惠金额，官方必填）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsGenOrderInfoInitReplyProduct>? ProductInfos { get; set; }

    /// <summary>获取或设置是否叠加使用商家优惠（0 叠加，1 不叠加、仅用小程序优惠；默认 0）。</summary>
    [JsonPropertyName("unuse_shop_discount")]
    public long? UnuseShopDiscount { get; set; }
}
