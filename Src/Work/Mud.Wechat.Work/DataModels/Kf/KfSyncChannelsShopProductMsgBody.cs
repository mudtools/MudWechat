// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服同步消息的视频号商品消息体（<c>/cgi-bin/kf/sync_msg</c> 的 channels_shop_product 消息）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncChannelsShopProductMsgBody
{
    /// <summary>
    /// 获取或设置商品 ID。
    /// </summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>
    /// 获取或设置商品头图 URL（官方字段名为 head_img）。
    /// </summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }

    /// <summary>
    /// 获取或设置商品标题。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置商品价格（单位：分）。
    /// </summary>
    [JsonPropertyName("sales_price")]
    public long? SalesPrice { get; set; }

    /// <summary>
    /// 获取或设置店铺名称。
    /// </summary>
    [JsonPropertyName("shop_nickname")]
    public string? ShopNickname { get; set; }

    /// <summary>
    /// 获取或设置店铺头像 URL（官方字段名为 shop_head_img）。
    /// </summary>
    [JsonPropertyName("shop_head_img")]
    public string? ShopHeadImg { get; set; }
}
