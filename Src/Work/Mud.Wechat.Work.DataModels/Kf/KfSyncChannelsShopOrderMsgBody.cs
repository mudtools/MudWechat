// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服同步消息的视频号订单消息体（<c>/cgi-bin/kf/sync_msg</c> 的 channels_shop_order 消息）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncChannelsShopOrderMsgBody
{
    /// <summary>
    /// 获取或设置订单 ID。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 获取或设置商品标题。
    /// </summary>
    [JsonPropertyName("product_titles")]
    public string? ProductTitles { get; set; }

    /// <summary>
    /// 获取或设置商品价格描述。
    /// </summary>
    [JsonPropertyName("price_wording")]
    public string? PriceWording { get; set; }

    /// <summary>
    /// 获取或设置订单状态。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置订单缩略图 URL。
    /// </summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 获取或设置店铺名称。
    /// </summary>
    [JsonPropertyName("shop_nickname")]
    public string? ShopNickname { get; set; }
}
