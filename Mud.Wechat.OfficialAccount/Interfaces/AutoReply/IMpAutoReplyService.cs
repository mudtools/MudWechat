// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「自动回复」域 SDK（1 端点，只读查询）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 → 自动回复（notify/autoreplies/ 前缀；2026-10-07 逐页核验，
/// subscription 订阅号域命中）。
/// </para>
/// <para>
/// <b>域级约束（逐页核验）</b>：适用范围「公众号 ✔ / 服务号 ✔」（非仅认证——官方注意事项原文
/// 「认证/未认证的服务号/订阅号，以及接口测试号，均拥有该接口权限」）；<b>只读</b>——仅能获取
/// 公众平台官网自动回复功能中设置的规则，公众号自行开发或第三方实现的自动回复无法获取；
/// 返回的图片/语音/视频为临时素材（每次获取不同，3 天内有效），图文为永久素材（详见响应 DTO remarks）。
/// </para>
/// <para><b>令牌路由</b>：消费 <see cref="MpTokenTypes.AccessToken"/>，Query 注入。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AutoReply", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpAutoReplyService
{
    /// <summary>
    /// 获取当前自动回复规则。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/notify/autoreplies/api_getcurrentautoreplyinfo.html"/>
    /// （官方接口英文名 <c>getCurrentAutoreplyInfo</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>自动回复规则（关注后 / 消息默认 / 关键词三类，结构见 <see cref="MpAutoReplyInfoResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>、无请求体。</para>
    /// <para>官方错误码表仅列 <c>0</c>（ok 或 in a normal state，官方原文两义并置，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/get_current_autoreply_info")]
    Task<MpAutoReplyInfoResponse> GetCurrentAutoReplyInfoAsync(
        CancellationToken cancellationToken = default);
}
