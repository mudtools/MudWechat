// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取可推广的商品详情请求（官方 <c>promoter/get_promote_product_detail</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetPromoteProductDetailRequest
{
    /// <summary>获取或设置机构商品所属小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置商品的计划类型（官方 <c>plan_type</c>，必填，1：定向计划 2：公开计划 3：机构定向计划 4：机构普通计划，见 <see cref="ChannelsPromoterPlanTypes"/>）。</summary>
    [JsonPropertyName("plan_type")]
    public int PlanType { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置是否返回商品可用的机构券（官方 <c>get_available_coupon</c>，填 true 返回）。</summary>
    [JsonPropertyName("get_available_coupon")]
    public bool? GetAvailableCoupon { get; set; }

    /// <summary>获取或设置商品所属供货机构 appid（官方 <c>head_supplier_appid</c>，<c>plan_type</c> 为 3 和 4 时必传）。</summary>
    [JsonPropertyName("head_supplier_appid")]
    public string? HeadSupplierAppId { get; set; }

    /// <summary>获取或设置用户自定义关联参数（官方 <c>custom_token</c>，长度要求 &lt;=128；仅商品卡分享直接成交生效，间接锁客不生效，当前不支持二维码和短链；直接成交时会在订单详情中返回）。</summary>
    [JsonPropertyName("custom_token")]
    public string? CustomToken { get; set; }
}
