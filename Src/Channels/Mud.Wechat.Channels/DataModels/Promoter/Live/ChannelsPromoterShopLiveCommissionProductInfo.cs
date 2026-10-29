// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>小店关联账号直播自营商品信息（官方 <c>product_list</c> 数组项，<c>promoter/get_shop_live_commission_product_list</c>）。</summary>
/// <remarks>官方参数表较带货者侧多 <c>commission_rate</c> ⇒ 与 <see cref="ChannelsPromoterLiveCommissionProductInfo"/> 分建。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterShopLiveCommissionProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>product_name</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>获取或设置商品的图片 url（官方 <c>product_img_url</c>）。</summary>
    [JsonPropertyName("product_img_url")]
    public string? ProductImgUrl { get; set; }

    /// <summary>获取或设置商品售价（官方 <c>product_price</c>，单位：分）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>获取或设置机构预估可得佣金金额（官方 <c>predict_commission_amount</c>，官方小店侧参数表未标注单位，示例为分）。</summary>
    [JsonPropertyName("predict_commission_amount")]
    public long? PredictCommissionAmount { get; set; }

    /// <summary>获取或设置商品的佣金比例（官方 <c>commission_rate</c>）。</summary>
    [JsonPropertyName("commission_rate")]
    public long? CommissionRate { get; set; }
}
