// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 公众号临时票据签发接口（<c>GET /cgi-bin/ticket/getticket</c>；票据管理器直调）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何与 <see cref="IMpAuthentication"/> 同形：不带 <c>[Token]</c>、令牌显式传参</b>。
/// 本接口由票据管理器在「令牌管理器内部」调用，若带上 <c>[Token]</c> 经框架注入
/// <c>access_token</c>，则票据刷新会进入 errcode 恢复链路 ⇒ 与令牌刷新互相递归
/// （令牌刷新 → 取令牌 → 票据请求失败 → 触发令牌恢复 → 再取令牌…）。
/// 故按仓库既有先例（<see cref="IMpAuthentication"/> 亦为「显式传参、不带 <c>[Token]</c>」）建模，
/// 由管理器把<b>本应用</b>的 <c>access_token</c> 显式传入。
/// </para>
/// <para>
/// <b>为什么必须显式传 <c>access_token</c> 而不是用「当前应用上下文」</b>：票据缓存是 per-app 的，
/// 而请求注入型令牌取自<b>当前异步上下文</b>；两者若混用，A 应用的票据缓存键可能配上 B 应用的令牌
/// （异步上下文切换期间尤其危险）⇒ 显式传参把「用哪个令牌取票据」变成编译期可见的事实。
/// </para>
/// <para>
/// <b>官方约束</b>：<c>type</c> 仅 <c>jsapi</c> / <c>wx_card</c>；有效期 7200 秒；
/// 「api 调用次数非常有限，频繁刷新会导致 api 调用受限」⇒ <b>只允许</b>由票据管理器调用；
/// 本接口<b>不支持第三方平台调用</b>（官方明确），故无 <c>authorizer_access_token</c> 形态。
/// </para>
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "Ticket")]
public interface IMpTicketService
{
    /// <summary>
    /// 获取临时票据。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/webdev/js/api_getticket.html"/>
    /// （官方接口英文名 <c>getTicket</c>）。
    /// </summary>
    /// <param name="accessToken">本应用的接口调用凭证（由票据管理器传入）。</param>
    /// <param name="type">票据类型（<see cref="MpTicketTypes.JsApi"/> / <see cref="MpTicketTypes.WxCard"/>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>票据与有效期。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>，两个业务 Query 参数（<c>access_token</c> 与 <c>type</c>），无请求体。</para>
    /// <para>错误码：<c>-1</c>（系统繁忙）/ <c>40001</c>（AppSecret 错误或 access_token 无效）。</para>
    /// <para>MUD005：令牌强制走 Query 参数（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/ticket/getticket")]
    Task<MpGetTicketResponse?> GetTicketAsync(
        [Query("access_token")] string accessToken,
        [Query("type")] string type,
        CancellationToken cancellationToken = default);
}
