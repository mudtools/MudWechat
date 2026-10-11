// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Shop;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「店铺管理」域 SDK（4 端点：店铺基本信息 / H5 链接 / 二维码 / 微信口令）。
/// </summary>
/// <remarks>
/// <para>
/// <b>企业微信参数传播面（官方契约）</b>：H5 链接 / 二维码 / 微信口令三端点均支持传入企业微信参数
/// （<c>wecom_corp_id</c> / <c>wecom_user_id</c>，经「获取关联账号企微id」接口获取）——
/// 带企微参数的链接 / 二维码 / 口令下单后，订单会在成交来源展示该企业。
/// </para>
/// <para>
/// <b>路由前缀</b>：本域 4 端点全部走 <c>/channels/ec/basics/*</c> 主干前缀。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Shop", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsShopService
{
    /// <summary>
    /// 获取店铺基本信息（<c>mmecapi_basicinfo</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺信息（<c>info</c>：店铺名称 / 头像 URL / 店铺类型 / 店铺状态 / 原始 ID / 是否本地生活小店 / 开店时间戳）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_mmecapi_basicinfo"/></para>
    /// <para>官方契约：<b>GET</b> + 空请求体（官方参数表仅 <c>access_token</c> 一个 URL 参数）。</para>
    /// <para>官方业务限制：本接口无特殊注意事项；店铺状态与类型取值见
    /// <see cref="ChannelsShopStatuses"/> / <see cref="ChannelsShopSubjectTypes"/>；
    /// <c>open_timestamp</c> 仅店铺状态为 open_finished 时返回。</para>
    /// <para>官方错误码：<c>43001</c>（需要使用 HTTP GET）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/channels/ec/basics/info/get")]
    Task<ChannelsShopBasicInfoResponse> GetShopInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺 H5 链接（<c>getshoph5url</c>，支持传入企业微信参数）。
    /// </summary>
    /// <param name="request">请求体（<c>wecom_corp_id</c>/<c>wecom_user_id</c> 均选填，经「获取关联账号企微id」接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺 H5 链接（<c>shop_h5url</c>；带企微参数下单后成交来源展示该企业）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_getshoph5url"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；与店铺口令端点的请求体字段集一致 ⇒ 共用请求 DTO
    /// <see cref="ChannelsShopWecomLinkRequest"/>（裁决理由见守卫 <c>ChannelsShopContractGuards</c> CS5）。</para>
    /// <para>官方错误码：<c>10021404</c>（企业微信未绑定店铺）/ <c>10021405</c>（该成员不是该企业微信成员）/
    /// <c>10021406</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/shop/h5url/get")]
    Task<ChannelsShopH5UrlResponse> GetShopH5UrlAsync(
        [Body] ChannelsShopWecomLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺二维码（<c>getshopqrcode</c>，支持传入企业微信参数）。
    /// </summary>
    /// <param name="request">请求体（<c>qrcode_type</c> 必填，见 <see cref="ChannelsShopQrcodeTypes"/>；<c>wecom_corp_id</c>/<c>wecom_user_id</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺二维码链接（<c>shop_qrcode</c>；带企微参数下单后成交来源展示该企业）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_getshopqrcode"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；<c>qrcode_type</c> 官方必填（1 二维码 / 2 标准物料 / 3 送礼物物料），
    /// 与 H5 链接 / 口令请求体字段集不同 ⇒ 独立请求 DTO（不共用）。</para>
    /// <para>官方错误码：<c>10021404</c>（企业微信未绑定店铺）/ <c>10021405</c>（该成员不是该企业微信成员）/
    /// <c>10021406</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/shop/qrcode/get")]
    Task<ChannelsShopQrcodeResponse> GetShopQrcodeAsync(
        [Body] ChannelsShopQrcodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺口令（<c>getshoptaglink</c>，支持传入企业微信参数）。
    /// </summary>
    /// <param name="request">请求体（<c>wecom_corp_id</c>/<c>wecom_user_id</c> 均选填，经「获取关联账号企微id」接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺微信口令（<c>shop_taglink</c>，形如 <c>#微信小店://微信小店/xxx</c>；带企微参数下单后成交来源展示该企业）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_getshoptaglink"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；与店铺 H5 链接端点的请求体字段集一致 ⇒ 共用请求 DTO
    /// <see cref="ChannelsShopWecomLinkRequest"/>（裁决理由见守卫 <c>ChannelsShopContractGuards</c> CS5）。</para>
    /// <para>官方错误码：<c>10021404</c>（企业微信未绑定店铺）/ <c>10021405</c>（该成员不是该企业微信成员）/
    /// <c>10021406</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/shop/taglink/get")]
    Task<ChannelsShopTagLinkResponse> GetShopTagLinkAsync(
        [Body] ChannelsShopWecomLinkRequest request,
        CancellationToken cancellationToken = default);
}
