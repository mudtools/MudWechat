// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「客服消息 → 会话控制」子分组 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/subscription/api/"/>
/// → 客服消息 → 会话控制（文档路径段 <c>customer/messctrl/</c>）。深链均为逐页核验过的真实页面。
/// </para>
/// <para>
/// <b>本子分组写入 SDK 的业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>创建会话的前置条件</b>：指定的客服账号<b>必须已经绑定微信号且在线</b>
/// （否则 <c>65415 the worker is not online</c>、<c>65402</c> 未绑定）。</item>
/// <item><b>接待互斥</b>：客户正在被其他客服接待时不可重复接入 / 关闭（<c>65414</c>）；
/// 无有效会话时关闭 / 查询报 <c>65413</c>。</item>
/// <item><b>未接入列表上限</b>：<c>getwaitcase</c> <b>最多返回 100 条</b>，按来访顺序，且官方<b>未提供分页游标</b>。</item>
/// <item><b>会话与聊天记录是两套东西</b>：本子分组管「谁在接待」；消息内容需经
/// <see cref="IMpCustomerMessageService.GetMsgListAsync"/>（聊天记录）获取。</item>
/// </list>
/// <para><b>账号适用性</b>：官方本子分组各页适用范围为「公众号 / 服务号 均<b>仅认证</b>」（非服务号专属）。</para>
/// <para>
/// <b>令牌路由</b>：5 端点均消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入 <c>access_token</c>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "KfSession", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpKfSessionService
{
    /// <summary>
    /// 创建会话。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/messctrl/api_createkfsession.html"/>
    /// （官方接口英文名 <c>createkfsession</c>）。
    /// </summary>
    /// <param name="request">会话双方（<c>kf_account</c> + <c>openid</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方「注意事项」原文</b>：「指定的客服账号<b>必须已经绑定微信号且在线</b>」
    /// ⇒ 调用前建议先用 <see cref="IMpKfAccountService.GetOnlineKfListAsync"/> 确认在线状态，
    /// 避免以异常做流程控制。
    /// </para>
    /// <para>
    /// 官方错误码：<c>0</c> / <c>65400</c>（未开通新版客服功能）/ <c>65401</c>（无效客服账号）/
    /// <c>65402</c>（客服账号尚未绑定微信号）/ <c>65415</c>（指定的客服不在线）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/kfsession/create")]
    Task<MpResponse> CreateSessionAsync(
        [Body] MpKfSessionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭会话。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/messctrl/api_closesession.html"/>
    /// （官方接口英文名 <c>closeSession</c>）。
    /// </summary>
    /// <param name="request">会话双方（<c>kf_account</c> + <c>openid</c>；与创建会话<b>共用同一 DTO</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方错误码：<c>0</c> / <c>40003</c>（invalid openid）/ <c>65413</c>（不存在对应用户的会话信息）/
    /// <c>65414</c>（客户正在被其他客服接待）。</para>
    /// <para>官方「注意事项」原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/kfsession/close")]
    Task<MpResponse> CloseSessionAsync(
        [Body] MpKfSessionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户会话状态。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/messctrl/api_getkfsession.html"/>
    /// （官方接口英文名 <c>getkfsession</c>）。
    /// </summary>
    /// <param name="openid">用户 openid（<b>官方为必填 Query 参数</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会话接入时间与接待客服账号（<c>createtime</c> / <c>kf_account</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET 且无请求体</b>，业务 Query 仅 <c>openid</c>。</para>
    /// <para>官方错误码：<c>40003</c>（invalid openid）/ <c>65400</c>（未开通新版客服功能）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/customservice/kfsession/getsession")]
    Task<MpGetKfSessionResponse> GetSessionAsync(
        [Query("openid")] string openid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客服会话列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/messctrl/api_getkfsessionlist.html"/>
    /// （官方接口英文名 <c>getkfsessionlist</c>）。
    /// </summary>
    /// <param name="kfAccount">完整客服账号（<b>官方为必填 Query 参数</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该客服的会话列表（<c>sessionlist</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET 且无请求体</b>，业务 Query 仅 <c>kf_account</c>。</para>
    /// <para>官方错误码：<c>0</c> / <c>65401</c>（无效客服账号）/ <c>65402</c>（客服账号尚未绑定微信号）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/customservice/kfsession/getsessionlist")]
    Task<MpGetKfSessionListResponse> GetSessionListAsync(
        [Query("kf_account")] string kfAccount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取未接入会话列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/messctrl/api_getwaitcase.html"/>
    /// （官方接口英文名 <c>getwaitcase</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>未接入会话（<c>count</c> + <c>waitcaselist[{latest_time, openid}]</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方契约（GET 且无请求体、无业务 Query 参数）</b>；元素字段官方<b>仅</b> <c>latest_time</c> 与
    /// <c>openid</c>（页面未出现 <c>kf_account</c>/<c>createtime</c>，SDK 不据其他页补全）。
    /// </para>
    /// <para>
    /// <b>返回上限（官方原文）</b>：<c>waitcaselist</c> <b>最多 100 条，按来访顺序</b>，
    /// 且官方<b>未提供分页游标</b> ⇒ 调用方须按 <c>latest_time</c> 自行做时序补偿，
    /// 不得假定「列表即全量积压」。
    /// </para>
    /// <para>官方错误码：<c>0</c> / <c>65400</c>（未开通或未升级新版客服功能）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/customservice/kfsession/getwaitcase")]
    Task<MpGetWaitCaseResponse> GetWaitCaseAsync(CancellationToken cancellationToken = default);
}
