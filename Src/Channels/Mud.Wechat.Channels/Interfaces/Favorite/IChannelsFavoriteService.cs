// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Favorite;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「收藏管理」域 SDK（1 端点：店铺收藏人数统计）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域范围</b>：本域仅店铺收藏统计 1 端点（商品收藏如官方后续开放再按域内增量补端点）。
/// </para>
/// <para>
/// <b>数据时效（官方契约，勿当实时数）</b>：官方原文「此接口使用离线数据进行统计，结果可能存在延迟」。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Favorite", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsFavoriteService
{
    /// <summary>
    /// 获取店铺收藏的人数（<c>GetFavoritesCount</c>，含各渠道收藏人数拆分）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 各渠道收藏人数（<c>favor_uv_acc_shop_homepage</c> 店铺首页 / <c>favor_uv_acc_order_detail</c> 订单详情页 /
    /// <c>favor_uv_acc_product_detail</c> 商品详情页 / <c>favor_uv_acc_other_scene</c> 其他场景 /
    /// <c>favor_uv_acc_all</c> 所有场景合计）。
    /// </returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/favorite/shopfavorite/api_getfavoritescount"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方参数表仅 <c>access_token</c> 一个 URL 参数）。</para>
    /// <para>
    /// 官方业务限制：<b>此接口使用离线数据进行统计，结果可能存在延迟</b>（官方原文）；
    /// 本接口无特殊注意事项、无特殊错误码（走通用错误码）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>官方适用范围：仅「微信小店」账号类型可调用（小店供货商等其他账号类型官方未放行）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/favorites/count/get")]
    Task<ChannelsFavoritesCountResponse> GetFavoritesCountAsync(CancellationToken cancellationToken = default);
}
