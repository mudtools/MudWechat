// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「长信息与短链」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/service/api/"/>
/// → 服务号二维码 → 长信息与短链。深链均为**逐页核验过的真实页面**（2026-10-07）。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>适用范围</b>：官方表为「小程序 ✔ / 服务号 仅认证 / 小游戏 ✔」（SDK 不做账号类型本地闸）。</item>
/// <item><b>容量与时效上限</b>：<c>long_data</c> <b>不超过 4KB</b>；<c>expire_seconds</c>
/// <b>最大值 2592000 秒（30 天）</b>，默认 2592000 —— 两处越界由官方
/// <c>9410010</c>/<c>9410011</c> 表达，SDK <b>不做本地拦截</b>（沿用「越界由官方 errcode 表达」的既存口径，
/// 规避 <c>long_data</c> 按字节还是字符计长的歧义）。</item>
/// <item><b>频率限制</b>：官方两页均<b>无频率章节、无频次数值</b>（SDK 不编造）。</item>
/// <item><b>第三方平台</b>：两页均支持代商家调用（<b>权限集 id 3、17</b>）——按 M0-R3 裁决只在 XML 记录，不扩实现面。</item>
/// <item><b>官方文档缺陷（照录）</b>：两页「注意事项」原文均为「本接口无特殊注意事项」，
/// 但参数层存在 4KB / 30 天硬上限与两条专属错误码；<c>fetchShorten</c> 页 <c>47003</c> 的描述原文为
/// 「模板参数不准确」（本接口无模板概念，疑为通用文案残留）。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShortLink", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpShortLinkService
{
    /// <summary>
    /// 长信息转短链。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/qrcode/shorten/api_genshortkey.html"/>
    /// （官方接口英文名 <c>genShortKey</c>；云调用方法 <c>officialAccount.shorten.gen</c>）。
    /// </summary>
    /// <param name="request">转换请求（<c>long_data</c> 必填 ≤4KB；<c>expire_seconds</c> 最大 2592000 秒）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>短 key（<c>short_key</c>，15 字节 base62）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"long_data": …, "expire_seconds": …}</c>。</para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>44002</c>（empty post data）/ <c>47001</c>（data format error）/
    /// <c>47003</c>（没有传入 <c>long_data</c>）/ <c>9410010</c>（long_data 长度超过限制 4KB）/
    /// <c>9410011</c>（expire_seconds 超过限制 30 天）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/shorten/gen")]
    Task<MpShortenGenResponse> GenerateShortKeyAsync(
        [Body] MpShortenGenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 短链转长信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/qrcode/shorten/api_fetchshorten.html"/>
    /// （官方接口英文名 <c>fetchShorten</c>；云调用方法 <c>officialAccount.shorten.fetch</c>）。
    /// </summary>
    /// <param name="request">还原请求（<c>short_key</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>长信息（<c>long_data</c>）/ 创建时间戳（<c>create_time</c>）/ 剩余过期秒数（<c>expire_seconds</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"short_key": …}</c>。</para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>44002</c> / <c>47001</c> / <c>47003</c> /
    /// <c>9410012</c>（short_key 不存在 / 已过期 / 不属于本账号）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/shorten/fetch")]
    Task<MpShortenFetchResponse> FetchShortKeyAsync(
        [Body] MpShortenFetchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 长链接转短链接（<b>旧版，官方已停维</b>）。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Account_Management/URL_Shortener.html"/>
    /// （官方接口英文名 <c>shortUrl</c>）。
    /// </summary>
    /// <param name="request">转换请求（<c>action</c> 固定 <c>long2short</c>；<c>long_url</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>短链接（<c>short_url</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>与 <see cref="GenerateShortKeyAsync"/> 的关系（官方两代接口，并存勿合并）</b>：本端点是旧版
    /// <c>/cgi-bin/shorturl</c>——官方已公告停止维护，仅存量链接兼容场景使用；新接入一律用新版
    /// <c>/cgi-bin/shorten/gen</c>。旧版转换的长链须已通过 ICP 备案。
    /// </para>
    /// <para>官方错误码：<c>40002</c> / <c>40005</c>（本页错误码表照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/shorturl")]
    Task<MpShortUrlResponse> GetShortUrlAsync(
        [Body] MpShortUrlRequest request,
        CancellationToken cancellationToken = default);
}
