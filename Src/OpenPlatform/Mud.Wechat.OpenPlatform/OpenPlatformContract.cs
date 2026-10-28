// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>
/// 微信开放平台「第三方平台」（component）的<b>官方契约常量</b>（第三方平台凭证链）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本产品线为何独立</b>：开放平台的 <c>component_access_token</c> /
/// <c>authorizer_access_token</c> 与自建形态的公众号 / 小程序 <c>access_token</c>
/// <b>不是同一套令牌</b>，且令牌域在既有产品线里是<b>单槽</b>（同一 <c>TokenType</c>）——
/// 一旦混入，两种令牌会互相覆盖，表现为「偶发 40001」这类最难定位的形态。
/// 故本线独立成包、独立令牌管理器（设计方案明文要求）。
/// </para>
/// <para>
/// <b>官方来源（2026-10-09 逐字核验）</b>：
/// 《令牌（component_access_token）》
/// <see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/ThirdParty/token/component_access_token.html"/>。
/// </para>
/// <para>
/// <b>核验中值得留档的官方原文</b>：① 请求体<b>三个字段全必填</b>
/// （<c>component_appid</c> / <c>component_appsecret</c> / <c>component_verify_ticket</c>）；
/// ② 令牌有效期<b>2 小时</b>，官方建议「在令牌快过期时（比如 1 小时 50 分）重新调用接口获取」
/// ⇒ 相当于<b>提前 10 分钟</b>刷新；③ <c>component_verify_ticket</c> 是<b>微信后台推送</b>的票据
/// （不由本 SDK 请求获取）⇒ 必须有外部写入端口，这是本线最关键的架构约束。
/// </para>
/// </remarks>
public static class OpenPlatformContract
{
    /// <summary>开放平台 API 基址（官方原文的主域名）。</summary>
    public const string ApiBaseUrl = "https://api.weixin.qq.com";

    /// <summary>获取第三方平台令牌的请求路径（官方原文）。</summary>
    public const string ComponentTokenPath = "/cgi-bin/component/api_component_token";

    /// <summary>令牌有效期（秒）：官方原文「每个令牌的有效期为 2 小时」⇒ 7200。</summary>
    public const int ComponentTokenLifetimeSeconds = 7200;

    /// <summary>
    /// 建议的提前刷新窗口（秒）：官方原文建议「在令牌快过期时（比如 <b>1 小时 50 分</b>）
    /// 重新调用接口获取」⇒ 距失效提前 <b>600</b> 秒（10 分钟）刷新。
    /// </summary>
    /// <remarks>
    /// <b>为何要提前而不是到期再刷</b>：令牌在<b>使用瞬间</b>才被官方校验，
    /// 若等到 <c>ExpiresAt</c> 才刷，临界期内的在途请求会用一把已失效的令牌 —— 且这类失败
    /// 表现为「偶发」而非「全挂」，排查成本极高。
    /// </remarks>
    public const int RecommendedRefreshLeadSeconds = 600;

    /// <summary>请求体字段：第三方平台 appid（官方 <c>component_appid</c>，必填）。</summary>
    public const string ComponentAppIdField = "component_appid";

    /// <summary>请求体字段：第三方平台 appsecret（官方 <c>component_appsecret</c>，必填）。</summary>
    public const string ComponentAppSecretField = "component_appsecret";

    /// <summary>请求体字段：微信后台推送的票据（官方 <c>component_verify_ticket</c>，必填）。</summary>
    public const string ComponentVerifyTicketField = "component_verify_ticket";

    /// <summary>应答字段：第三方平台令牌（官方 <c>component_access_token</c>）。</summary>
    public const string ComponentAccessTokenField = "component_access_token";

    /// <summary>应答字段：有效期秒数（官方 <c>expires_in</c>）。</summary>
    public const string ExpiresInField = "expires_in";

    // ==================== 授权流程（预授权码 / 换取授权信息 / 刷新授权方令牌） ====================

    /// <summary>获取预授权码的请求路径（官方原文）。</summary>
    public const string PreAuthCodePath = "/cgi-bin/component/api_create_preauthcode";

    /// <summary>换取授权信息的请求路径（官方原文）。</summary>
    public const string QueryAuthPath = "/cgi-bin/component/api_query_auth";

    /// <summary>获取 / 刷新授权方接口调用令牌的请求路径（官方原文）。</summary>
    public const string AuthorizerTokenPath = "/cgi-bin/component/api_authorizer_token";

    /// <summary>
    /// 平台令牌在 URL 上的查询参数名。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>⚠️ 官方页面之间存在自相矛盾（照录并择一，勿静默「统一」）</b>：
    /// 「预授权码」页与「获取/刷新接口调用令牌」页均写作
    /// <c>?component_access_token=…</c>，而「获取授权信息」（<c>api_query_auth</c>）页
    /// 写成 <c>?access_token=…</c>。本仓统一采用<b>多数页面的 <c>component_access_token</c></b>
    /// （且它与本线令牌名一致）；若实际联调发现官方只认另一种拼写，须<b>同批</b>
    /// 修改本常量与相关用例，不要在两处各写一份。
    /// </para>
    /// <para>
    /// <b>安全提示（与 MUD005 同类）</b>：令牌走 URL 查询参数是<b>官方契约强制</b>的，
    /// 无法改为 Header；因此<b>任何日志 / 遥测 / 异常消息都不得打印完整请求 URL</b>
    /// （令牌会随 URL 一起泄露）。本线实现里对 URL 只做「不含令牌」的形态记录。
    /// </para>
    /// </remarks>
    public const string ComponentAccessTokenQueryName = "component_access_token";

    /// <summary>预授权码有效期（秒）：官方原文「每个预授权码有效期为 1800 秒」。</summary>
    public const int PreAuthCodeLifetimeSeconds = 1800;

    /// <summary>授权方接口调用令牌有效期（秒）：官方原文「authorizer_access_token 有效期为 2 小时」。</summary>
    public const int AuthorizerTokenLifetimeSeconds = 7200;

    /// <summary>
    /// 授权方接口调用令牌的<b>本仓</b>提前刷新窗口（秒）。
    /// </summary>
    /// <remarks>
    /// <b>⚠️ 这不是官方契约</b>：官方对授权方令牌只要求「缓存 authorizer_access_token，
    /// 避免获取/刷新接口调用令牌的 API 调用触发<b>每日限额</b>」，<b>未</b>规定任何提前量。
    /// 本仓取 600 秒（与平台令牌一致）属<b>经验值</b> —— 明确标注以免后来者把它当官方事实引用。
    /// </remarks>
    public const int AuthorizerTokenRefreshLeadSeconds = 600;

    /// <summary>请求体字段：预授权码（官方 <c>pre_auth_code</c>）。</summary>
    public const string PreAuthCodeField = "pre_auth_code";

    /// <summary>请求体字段：授权码（官方 <c>authorization_code</c>，由授权回调 URI 的 URL 参数给出）。</summary>
    public const string AuthorizationCodeField = "authorization_code";

    /// <summary>应答字段：授权信息对象（官方 <c>authorization_info</c>）。</summary>
    public const string AuthorizationInfoField = "authorization_info";

    /// <summary>应答字段：授权方 appid（官方 <c>authorizer_appid</c>）。</summary>
    public const string AuthorizerAppIdField = "authorizer_appid";

    /// <summary>应答字段：授权方接口调用令牌（官方 <c>authorizer_access_token</c>）。</summary>
    public const string AuthorizerAccessTokenField = "authorizer_access_token";

    /// <summary>应答字段：授权方刷新令牌（官方 <c>authorizer_refresh_token</c>）。</summary>
    public const string AuthorizerRefreshTokenField = "authorizer_refresh_token";

    /// <summary>官方错误码字段（官方 <c>errcode</c>）。</summary>
    public const string ErrCodeField = "errcode";

    /// <summary>官方错误描述字段（官方 <c>errmsg</c>）。</summary>
    public const string ErrMsgField = "errmsg";
}
