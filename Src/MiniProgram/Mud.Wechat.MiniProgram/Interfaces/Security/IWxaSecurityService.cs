// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「内容安全」域 SDK（3 端点：文本同步审核 + 音视频异步审核 + 安全风控）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 内容安全，2026-10-09 核验）：
/// 文本 <c>sec-center/sec-check/api_msgseccheck.html</c>、
/// 音视频 <c>sec-center/sec-check/api_mediacheckasync.html</c>、
/// 安全风控 <c>sec-center/safety-control-capability/api_getuserriskrank.html</c>。
/// </para>
/// <para>
/// <b>图片内容安全 <c>/wxa/img_sec_check</c> 刻意不实现</b>：官方文档页
/// <c>sec-center/sec-check/api_imgseccheck.html</c> 经实测为<b>硬 404</b>（已下架）；
/// 图片 / 音频 / 视频统一走 <c>media_check_async</c>（同步图片审核已无公开入口，不凭旧记忆建模）。
/// </para>
/// <para>
/// <b>同步 vs 异步（勿混为一谈）</b>：<c>msg_sec_check</c> 为同步（应答即结论）；
/// <c>media_check_async</c> 仅<b>受理</b>并返回 <c>trace_id</c>，结论须经
/// <c>media_check_async</c> 的结果<b>回调</b>（<c>wxa_media_check</c> 事件）或另途查询获取 ——
/// SDK 只提供端点，<b>不做「提交 → 轮询 / 等回调」编排</b>（与公众号线智能接口同一取舍）。
/// </para>
/// <para>
/// <b>建议策略（官方原文）</b>：内容安全接口为<b>异步/准异步</b>能力，官方建议
/// 「先审后发」时对<b>疑似</b>内容做人工复核；<c>40001</c> 走令牌自愈。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Security", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaSecurityService
{
    /// <summary>
    /// 文本内容安全识别（同步）。官方文档：<c>sec-center/sec-check/api_msgseccheck.html</c>。
    /// </summary>
    /// <param name="request">待检文本与场景，见 <see cref="WxaMsgSecCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核结论（<c>result.suggest</c> / <c>detail[]</c> / <c>trace_id</c>），见 <see cref="WxaMsgSecCheckResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/msg_sec_check</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>结论判定（官方原文）</b>：以 <c>result.suggest</c> 为准（<c>risky</c> 违规 / <c>pass</c> 通过 /
    /// <c>review</c> 需人工复核）；<c>detail[].strategy</c> 标明命中的策略（内容 / 色情 / 广告等）。
    /// <b>注意 <c>errcode</c> 为 0 仅表示「调用成功」，不代表内容合规</b> ——
    /// 违规与否在 <c>result</c> 与 <c>detail</c> 里，勿据 <c>errcode</c> 放行。
    /// </para>
    /// <para><b>版本与场景</b>：<c>version</c> = <c>2</c> 为当前版本；<c>scene</c> 标明调用场景
    /// （资料 / 评论 / 论坛 / 社交日志），<c>openid</c> 为可选的用户标识（同用户累积违规会触发封禁策略）。</para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>43104</c>（未开通内容安全能力）/ <c>87014</c>（内容含违规信息）。</para>
    /// </remarks>
    [Post("/wxa/msg_sec_check")]
    Task<WxaMsgSecCheckResponse> MsgSecCheckAsync(
        [Body] WxaMsgSecCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 音视频内容安全识别（<b>异步受理</b>）。官方文档：<c>sec-center/sec-check/api_mediacheckasync.html</c>。
    /// </summary>
    /// <param name="request">媒体地址与类型，见 <see cref="WxaMediaCheckAsyncRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c> 与<b>查询用</b> <c>trace_id</c>，见 <see cref="WxaMediaCheckAsyncResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/media_check_async</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>本端点只受理，不给结论</b>（官方语义）：应答的 <c>trace_id</c> 是唯一凭据，
    /// 结论经官方<b>回调</b>（<c>wxa_media_check</c>）送达，<b>或</b>由宿主另途查询。
    /// SDK <b>不建轮询</b>，也<b>不把 <c>errcode = 0</c> 当审核通过</b>。
    /// </para>
    /// <para>
    /// <b>媒体地址要求（官方原文）</b>：<c>media_url</c> 须为<b>公网可访问</b>的链接（微信服务器需回源拉取），
    /// 内网 / 需鉴权的地址会被官方拒绝。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>43104</c>（未开通）/ <c>44002</c>（空请求体）/ <c>47001</c>（数据格式错误）。</para>
    /// </remarks>
    [Post("/wxa/media_check_async")]
    Task<WxaMediaCheckAsyncResponse> MediaCheckAsyncAsync(
        [Body] WxaMediaCheckAsyncRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户安全等级（<c>risk_rank</c>）。官方文档：<c>sec-center/safety-control-capability/api_getuserriskrank.html</c>。
    /// </summary>
    /// <param name="request">用户信息与场景，见 <see cref="WxaGetUserRiskRankRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>风险等级（<c>risk_rank</c>，0~4），见 <see cref="WxaGetUserRiskRankResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/getuserriskrank</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>能力口径（官方原文）</b>：根据提交的用户信息获取安全等级，<b>无需用户授权</b>；
    /// <c>appid</c> / <c>openid</c> 为用户侧标识，<c>scene</c> 默认 <c>2</c>（营销活动）。
    /// </para>
    /// <para><b>等级语义（勿把数值当布尔）</b>：<c>0</c> 通过 / <c>1</c> 未知 / <c>2</c> 风险待确认 /
    /// <c>3</c> 风险命中 / <c>4</c> 高风险命中。</para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>43104</c>（未开通安全风控能力）。</para>
    /// </remarks>
    [Post("/wxa/getuserriskrank")]
    Task<WxaGetUserRiskRankResponse> GetUserRiskRankAsync(
        [Body] WxaGetUserRiskRankRequest request,
        CancellationToken cancellationToken = default);
}
