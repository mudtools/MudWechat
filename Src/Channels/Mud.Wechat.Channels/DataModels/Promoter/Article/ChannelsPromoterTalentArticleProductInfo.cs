// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>带货者文章挂载的商品信息（官方 <c>product_info_list</c> 数组项，<c>promoter/get_talent_biz_article_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterTalentArticleProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>product_name</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>获取或设置商品图片（官方 <c>product_img_url</c>）。</summary>
    [JsonPropertyName("product_img_url")]
    public string? ProductImgUrl { get; set; }

    /// <summary>获取或设置机构预计可得佣金（官方 <c>predict_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("predict_commission_amount")]
    public long? PredictCommissionAmount { get; set; }

    /// <summary>获取或设置商品最低价格（官方 <c>product_mini_price</c>，单位：分）。</summary>
    [JsonPropertyName("product_mini_price")]
    public long? ProductMiniPrice { get; set; }
}
