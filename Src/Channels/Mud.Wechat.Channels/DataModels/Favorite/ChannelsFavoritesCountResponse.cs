// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Favorite;

/// <summary>
/// 获取店铺收藏的人数（<c>GetFavoritesCount</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方参数表仅 <c>access_token</c> 一个 URL 参数）；
/// 官方响应示例整体不带 <c>errcode</c>/<c>errmsg</c>（成功形态），判错由 <see cref="ChannelsResponse.IsSuccess"/> 兜底。
/// 官方业务限制：使用离线数据进行统计，结果可能存在延迟（官方原文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Favorite")]
public class ChannelsFavoritesCountResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺首页收藏用户数（官方 <c>favor_uv_acc_shop_homepage</c>）。</summary>
    [JsonPropertyName("favor_uv_acc_shop_homepage")]
    public int? FavorUvAccShopHomepage { get; set; }

    /// <summary>获取或设置订单详情页收藏用户数（官方 <c>favor_uv_acc_order_detail</c>）。</summary>
    [JsonPropertyName("favor_uv_acc_order_detail")]
    public int? FavorUvAccOrderDetail { get; set; }

    /// <summary>获取或设置商品详情页收藏用户数（官方 <c>favor_uv_acc_product_detail</c>）。</summary>
    [JsonPropertyName("favor_uv_acc_product_detail")]
    public int? FavorUvAccProductDetail { get; set; }

    /// <summary>获取或设置其他场景收藏用户数（官方 <c>favor_uv_acc_other_scene</c>）。</summary>
    [JsonPropertyName("favor_uv_acc_other_scene")]
    public int? FavorUvAccOtherScene { get; set; }

    /// <summary>获取或设置所有收藏用户数（官方 <c>favor_uv_acc_all</c>）。</summary>
    [JsonPropertyName("favor_uv_acc_all")]
    public int? FavorUvAccAll { get; set; }
}
