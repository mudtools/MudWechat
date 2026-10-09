// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「二维码 / 链接」域 SDK —— <b>JSON 通道</b>（5 端点：URL Link / URL Scheme / ShortLink）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API，2026-10-09 逐页核验）：
/// 生成 URL Link <c>qrcode-link/url-link/api_generateurllink.html</c>、
/// 查询 URL Link <c>qrcode-link/url-link/api_queryurllink.html</c>、
/// 生成 URL Scheme <c>qrcode-link/url-scheme/api_generatescheme.html</c>、
/// 查询 URL Scheme <c>qrcode-link/url-scheme/api_queryscheme.html</c>、
/// 生成 ShortLink <c>qrcode-link/short-link/api_generateshortlink.html</c>。
/// </para>
/// <para>
/// <b>双通道分工（对齐公众号线素材域先例，勿合并）</b>：本域 8 端点中，
/// <b>URL Link / URL Scheme / ShortLink 五端点响应为 JSON</b>（本接口，走生成管线）；
/// <b>小程序码三端点响应为图片二进制流</b>（失败时才是 JSON）⇒ 落独立请求形态
/// <c>IWxaCodeService</c>（Content-Type 分支判错，不进 JSON 反序列化管线）。两者同挂
/// <c>QrCodeLink</c> 注册组。
/// </para>
/// <para>
/// <b>勿与公众号端点混淆（MP-X6）</b>：<c>/wxa/genwxashortlink</c>（<c>page_url</c> → <c>link</c>）与
/// 公众号 <c>/cgi-bin/shorten/gen</c>（<c>long_data</c> ≤ 4KB）是<b>不同端点、不同语义</b>，并存不构成重复。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "QrCodeLink", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaQrCodeLinkService
{
    /// <summary>
    /// 生成 URL Link（小程序加密链接）。官方文档：<c>api_generateurllink.html</c>。
    /// </summary>
    /// <param name="request">生成请求（<c>path</c> 必填），见 <see cref="WxaUrlLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生成的链接（<c>url_link</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/generate_urllink</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>数量与配额（官方原文要点）</b>：加密 URL Link 与明文 URL Scheme 的<b>单天访问次数合计</b>上限
    /// （官方以 <c>quota_info.remain_visit_quota</c> 经查询接口暴露剩余额度）；
    /// 生成有效链接总数亦有上限，超限报错。均<b>不做本地拦截</b>（由官方错误码表达）。
    /// </para>
    /// <para><b>有效期</b>：<c>expire_type</c> / <c>expire_interval</c> / <c>expire_time</c> 三选一表达，
    /// 取值语义与上限以官方页面为准（SDK 不编造数值，也不做本地校验）。</para>
    /// </remarks>
    [Post("/wxa/generate_urllink")]
    Task<WxaUrlLinkResponse> GenerateUrlLinkAsync(
        [Body] WxaUrlLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 URL Link 信息。官方文档：<c>api_queryurllink.html</c>（2026-10-09 逐字段核验）。
    /// </summary>
    /// <param name="request">查询请求（<c>url_link</c> 与 <c>query_type</c>），见 <see cref="WxaQueryUrlLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>链接配置（<c>url_link_info</c>）或剩余额度（<c>quota_info</c>），见 <see cref="WxaQueryUrlLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/query_urllink</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b><c>query_type</c> 双义</b>：<c>0</c>（默认）= 查询 <c>url_link</c> 配置；
    /// <c>1</c> = 查询<b>当天剩余访问次数</b>（此时 <c>url_link</c> 无需传，响应填 <c>quota_info</c>）。
    /// </para>
    /// </remarks>
    [Post("/wxa/query_urllink")]
    Task<WxaQueryUrlLinkResponse> QueryUrlLinkAsync(
        [Body] WxaQueryUrlLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成 URL Scheme（小程序加密 scheme 码）。官方文档：<c>api_generatescheme.html</c>。
    /// </summary>
    /// <param name="request">生成请求（<c>jump_wxa</c>），见 <see cref="WxaSchemeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生成的 scheme（<c>openlink</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/generatescheme</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>配额</b>：与加密 URL Link 共用「单天访问次数」限额（见 <see cref="QuerySchemeAsync"/> 的 <c>quota_info</c>）。</para>
    /// </remarks>
    [Post("/wxa/generatescheme")]
    Task<WxaSchemeResponse> GenerateSchemeAsync(
        [Body] WxaSchemeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 URL Scheme 信息。官方文档：<c>api_queryscheme.html</c>（2026-10-09 逐字段核验）。
    /// </summary>
    /// <param name="request">查询请求（<c>scheme</c> / <c>query_type</c>），见 <see cref="WxaQuerySchemeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>scheme 配置（<c>scheme_info</c>）或剩余额度（<c>quota_info</c>），见 <see cref="WxaQuerySchemeResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/queryscheme</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><c>query_type</c> 双义同 <see cref="QueryUrlLinkAsync"/>（<c>1</c> 时填 <c>quota_info.remain_visit_quota</c>）。</para>
    /// </remarks>
    [Post("/wxa/queryscheme")]
    Task<WxaQuerySchemeResponse> QuerySchemeAsync(
        [Body] WxaQuerySchemeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成 ShortLink（小程序短链）。官方文档：<c>api_generateshortlink.html</c>。
    /// </summary>
    /// <param name="request">生成请求（<c>page_url</c> 必填），见 <see cref="WxaShortLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>生成的短链（<c>link</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/genwxashortlink</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b><c>page_url</c> 必须是配置了 <c>sitemap.json</c> 的页面</b>（官方原文），否则生成失败；
    /// 短链<b>不支持 <c>query</c> 参数</b>（与 URL Link / Scheme 的本质差异）。
    /// </para>
    /// <para><b>时效</b>：<c>is_permanent</c> 为长期有效开关；短期有效时由
    /// <c>expire_type</c> / <c>expire_interval</c> / <c>expire_time</c> 表达（上限以官方页面为准）。</para>
    /// </remarks>
    [Post("/wxa/genwxashortlink")]
    Task<WxaShortLinkResponse> GenerateShortLinkAsync(
        [Body] WxaShortLinkRequest request,
        CancellationToken cancellationToken = default);
}
