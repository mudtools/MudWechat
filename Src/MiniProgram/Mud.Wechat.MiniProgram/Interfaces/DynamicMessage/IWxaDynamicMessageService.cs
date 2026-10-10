// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「动态消息」域 SDK（3 端点：创建 activity_id + 修改动态消息 + 聊天工具动态卡片）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 消息管理 → 动态消息，2026-10-10 依据官方清单核验）：
/// 创建 <c>activity_id</c> <c>mp-message-management/updatable-message/api_createactivityid.html</c>、
/// 修改动态消息 <c>mp-message-management/updatable-message/api_setupdatablemsg.html</c>、
/// 修改小程序聊天工具的动态卡片消息 <c>mp-message-management/updatable-message/api_setchattoolmsg.html</c>。
/// </para>
/// <para>
/// <b>能力口径（官方原文）</b>：动态消息是<b>被分享后可实时更新内容</b>的消息形态（如活动进度 / 排行）；
/// 先经 <c>activityid/create</c> 取得 <c>activity_id</c>（同时用于动态消息与私密消息），
/// 再由 <c>updatablemsg/send</c> 按 <c>target_state</c> 更新已分享卡片的内容。
/// <c>chattoolmsg/send</c> 是<b>小程序聊天工具</b>场景的动态卡片消息，走同一套 <c>parameter_list</c> 表达。
/// </para>
/// <para>
/// <b>令牌与时限</b>：三者均走应用级 <c>access_token</c>（Query）；<c>activity_id</c> 携带有效期
/// （官方以 <c>expiration_time</c> 表达），更新须在分享卡片的有效期内进行。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "DynamicMessage", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaDynamicMessageService
{
    /// <summary>
    /// 创建被分享动态消息 / 私密消息的 <c>activity_id</c>。官方文档：<c>api_createactivityid.html</c>。
    /// </summary>
    /// <param name="request">创建请求（官方无必填参数；<c>unionid</c> 为选填的开放平台用户标识），见 <see cref="WxaCreateActivityIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>活动唯一标识与有效期（<c>activity_id</c> / <c>expiration_time</c> / <c>is_expired</c>），见 <see cref="WxaCreateActivityIdResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/message/wxopen/activityid/create</c> ＋ 请求体 JSON（可为空对象）；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>有效期（官方原文）</b>：<c>activity_id</c> 有时效（<c>expiration_time</c>，Unix 秒级时间戳）；
    /// <b>同一 <c>activity_id</c> 只能被使用一次</b>（分享后即失效，不可复用于多条动态消息）。
    /// 需再次分享时须重新创建。
    /// </para>
    /// <para><b>私密消息说明</b>：本接口同时是私密消息（<c>openapi/privatemessage</c> 系列）的入口凭证来源。</para>
    /// </remarks>
    [Post("/cgi-bin/message/wxopen/activityid/create")]
    Task<WxaCreateActivityIdResponse> CreateActivityIdAsync(
        [Body] WxaCreateActivityIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改被分享的动态消息。官方文档：<c>api_setupdatablemsg.html</c>。
    /// </summary>
    /// <param name="request">动态消息活动与目标状态，见 <see cref="WxaUpdateDynamicMessageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/message/wxopen/updatablemsg/send</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b><c>target_state</c> 语义（官方原文）</b>：<c>0</c>=未进入游戏 / <c>1</c>=已进入游戏（用于区分
    /// 玩前 / 玩中 / 玩后展示的卡片内容）。<c>template_info.parameter_list[].value</c> 为具体展示文本，
    /// 一键更新同一卡片在各处的展示。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/message/wxopen/updatablemsg/send")]
    Task<WxaResponse> UpdateDynamicMessageAsync(
        [Body] WxaUpdateDynamicMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改小程序聊天工具的动态卡片消息。官方文档：<c>api_setchattoolmsg.html</c>。
    /// </summary>
    /// <param name="request">聊天工具动态卡片的活动与内容，见 <see cref="WxaChatToolMsgRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/message/wxopen/chattoolmsg/send</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>与 <see cref="UpdateDynamicMessageAsync"/> 的差异</b>：本端点是<b>小程序聊天工具</b>
    /// （微信内聊天场景的卡片消息）专属形态，模板参数表达同构（<c>parameter_list</c>），但<b>不含</b>
    /// <c>target_state</c> 游戏态语义。</para>
    /// </remarks>
    [Post("/cgi-bin/message/wxopen/chattoolmsg/send")]
    Task<WxaResponse> SendChatToolMsgAsync(
        [Body] WxaChatToolMsgRequest request,
        CancellationToken cancellationToken = default);
}