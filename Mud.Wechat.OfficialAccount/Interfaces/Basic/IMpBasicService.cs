// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「基础接口」域 SDK
/// （获取微信 API 服务器 IP + 获取微信推送服务器 IP + 网络通信检测，即官方「服务端 API → 基础接口」
/// 分组下的三个业务端点；同分组的「获取接口调用凭据」「获取稳定版接口调用凭据」两个令牌签发端点在
/// Abstractions 的 Authentication 注册组承载）。
/// </summary>
/// <remarks>
/// <para>
/// <b>M0 范围边界（显式声明，禁止静默不做）</b>：本域三个端点官方<b>均支持</b>第三方平台令牌——
/// 第三方平台可用 <c>component_access_token</c> 自调用，服务商获得任意权限集授权后可用
/// <c>authorizer_access_token</c> 代商家调用。本 SDK 的 M0 仅覆盖<b>自建形态</b>
/// （<c>AppId</c> + <c>AppSecret</c> 换取的 <c>access_token</c>），第三方平台令牌形态
/// （依赖 <c>component_verify_ticket</c> 回调与授权编排）不在本里程碑范围。
/// </para>
/// <para>
/// <b>结构性差异（与企微「基础接口」域对比）</b>：公众号无「自建 / 套件 / 代开发」三类应用形态
/// ⇒ <b>本接口即注册接口</b>（标 <c>RegistryGroupName</c> 但<b>不</b>标 <c>IsAbstract</c>；
/// <c>IsAbstract</c> 的接口不参与 DI 注册，无子接口时会使客户端无法解析），
/// 不存在 <c>_Internal</c> / <c>_ThirdParty</c> / <c>_Provider</c> 子接口。
/// </para>
/// <para>
/// 令牌路由键 <see cref="MpTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多公众号基座按当前应用上下文（AppKey）路由；走「普通通道」还是「稳定版通道」由
/// <c>MpAppConfig.UseStableToken</c> 在注册期决定，<b>不体现在本接口声明上</b>。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信公众号官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>MpException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Basic", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IMpBasicService
{
    /// <summary>
    /// 获取微信 API 服务器 IP。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信 API 服务器 IP 列表（失败时官方返回 <c>errcode</c>/<c>errmsg</c> 且 <c>ip_list</c> 缺省）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/base/api_getapidomainip.html"/></para>
    /// <para>
    /// 官方业务约束：出口 IP 及入口 IP 可能变动，官方<b>建议每天请求 1 次</b>以更新 IP 列表；
    /// 不建议长期使用旧 IP 列表作为 <c>api.weixin.qq.com</c> 的访问入口（单点故障）；使用固定 IP 访问时
    /// 需注意运营商适配，跨运营商访问高峰期可能丢包。
    /// </para>
    /// <para>官方错误码：<c>40013</c>（invalid appid）+ 通用错误码。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/get_api_domain_ip")]
    Task<MpGetApiDomainIpResponse> GetApiDomainIpAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取微信推送服务器 IP。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信推送服务器 IP 列表（失败时官方返回 <c>errcode</c>/<c>errmsg</c> 且 <c>ip_list</c> 缺省）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/base/api_getcallbackip.html"/></para>
    /// <para>响应形态与约束同 <see cref="GetApiDomainIpAsync"/>（官方两页正文逐项一致）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/getcallbackip")]
    Task<MpGetCallbackIpResponse> GetCallbackIpAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 网络通信检测（公众号特有端点，企业微信无对应接口）。
    /// </summary>
    /// <param name="request">检测请求（<c>action</c> 与 <c>check_operator</c> 均为官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>DNS 解析与 PING 检测结果。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/base/api_callbackcheck.html"/></para>
    /// <para>
    /// 功能定位：排查<b>回调连接失败</b>——对开发者 URL 做域名解析，然后对所有 IP 各 ping 一次，
    /// 得到丢包率与耗时。取值常量见 <c>MpCallbackCheckActions</c> / <c>MpCallbackCheckOperators</c>。
    /// </para>
    /// <para>
    /// 官方说明：<c>package_loss</c> 因仅发送一个 ping 包，取值只有 <c>0%</c> / <c>100%</c> 两种，
    /// 不宜作为精细化网络质量指标；官方「注意事项」章节原文为「本接口无特殊注意事项」。
    /// </para>
    /// <para>官方错误码：<c>40201</c>（未设置回调 URL）/ <c>40202</c>（非法 action）/ <c>40203</c>（非法运营商参数）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/callback/check")]
    Task<MpCallbackCheckResponse> CheckCallbackAsync(
        [Body] MpCallbackCheckRequest request,
        CancellationToken cancellationToken = default);
}
