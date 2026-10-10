// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「订阅消息 / 服务卡片」域 SDK（4 端点：发送订阅消息 + 服务卡片三端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 消息管理 → 订阅消息，2026-10-10 依据官方清单核验）：
/// 发送订阅消息 <c>mp-message-management/subscribe-message/api_sendmessage.html</c>、
/// 激活与更新服务卡片 <c>api_setusernotify.html</c>、
/// 更新服务卡片扩展信息 <c>api_setusernotifyext.html</c>、
/// 查询服务卡片状态 <c>api_getusernotify.html</c>。
/// </para>
/// <para>
/// <b>为何只补 4 端点（MP-X1）</b>：模板库六端点（<c>/wxaapi/newtmpl/deltemplate</c>、<c>getcategory</c>、
/// <c>getpubtemplatekeywords</c>、<c>getpubtemplatetitles</c>、<c>gettemplate</c>、<c>addtemplate</c>）
/// 已被公众号线 <c>IMpSubscriptionNoticeService</c> 占据 —— 小程序与公众号同平台、同令牌域，重复声明即
/// 制造双份维护，故本线只补公众号线未覆盖的「发送（<c>subscribe/send</c>）」与「服务卡片」四个端点。
/// </para>
/// <para>
/// <b>服务卡片形态（官方原文）</b>：服务卡片是订阅消息的「卡片化」表达 ——
/// <c>set_user_notify</c> 激活并首次写入卡片内容；<c>set_user_notifyext</c> 更新卡片的扩展信息；
/// <c>get_user_notify</c> 查询卡片当前激活状态。三者以 <c>openid</c> + <c>template_id</c> 定位。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "SubscribeMessage", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaSubscribeMessageService
{
    /// <summary>
    /// 发送订阅消息。官方文档：<c>mp-message-management/subscribe-message/api_sendmessage.html</c>。
    /// </summary>
    /// <param name="request">接收者、模板与内容，见 <see cref="WxaSubscribeMessageSendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示发送成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/message/subscribe/send</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>前置约束（官方原文）</b>：一次调用<b>只能下发一条</b>且须用户订阅过该模板
    /// —— 小程序端经 <c>wx.requestSubscribeMessage</c> 取得订阅授权（一次性订阅模板长期有效、长期订阅模板
    /// 长期有效但受条数/灰度约束）；未订阅或已用尽即报 <c>43101</c>。若用户滥用，则可能收到 <c>43101</c> 或 <c>40037</c> 等
    /// 错误码，开发者可在小程序后台管理「滥用处罚」状态。
    /// </para>
    /// <para>
    /// <b><c>data</c> 的关键词校验（官方原文）</b>：<c>data</c> 中若传了模板关键词之外的关键词，会报
    /// <c>47003</c>（参数不合法，关键词与模板不匹配）；关键词取值须与模板规定的类型一致，如数字模板
    /// 不允许字符串。跳转 <c>page</c> 必须是模板允许的小程序页面。
    /// </para>
    /// <para><b>环境限定</b>：<c>miniprogram_state</c> = <c>trial</c>（体验版）时模板关键词\n\n或内容包含
    /// 特殊字符会报 <c>43102</c>（模板内容与正式版不同）之类的<a>环境差异</a>，SDK 不做本地校验。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（令牌无效，走自愈）/ <c>40037</c>（模板不存在）/ <c>43101</c>（用户未订阅）/ <c>47003</c>（关键词不匹配）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/subscribe/send")]
    Task<WxaResponse> SendSubscribeMessageAsync(
        [Body] WxaSubscribeMessageSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 激活与更新服务卡片。官方文档：<c>mp-message-management/subscribe-message/api_setusernotify.html</c>。
    /// </summary>
    /// <param name="request">卡片定位与内容（<c>openid</c> / <c>template_id</c> 必填），见 <see cref="WxaSetUserNotifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/set_user_notify</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>能力口径（官方原文）</b>：用于<b>激活</b>服务卡片（首次）或<b>更新</b>其内容；
    /// <c>template_id</c> 为服务卡片模板 ID，<c>data</c> 为卡片内容键值集（键须在模板中登记）。
    /// <see cref="WxaSetUserNotifyRequest.Page"/> 为点击卡片后的跳转页面（仅限本小程序内）。
    /// </para>
    /// </remarks>
    [Post("/wxa/set_user_notify")]
    Task<WxaResponse> SetUserNotifyAsync(
        [Body] WxaSetUserNotifyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新服务卡片扩展信息。官方文档：<c>mp-message-management/subscribe-message/api_setusernotifyext.html</c>。
    /// </summary>
    /// <param name="request">卡片定位与扩展内容，见 <see cref="WxaSetUserNotifyExtRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/set_user_notifyext</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>与 <see cref="SetUserNotifyAsync"/> 的关系（官方原文）</b>：本接口只<b>追加/更新扩展信息</b>
    /// （如按钮、表单等交互组件的数据），不改写卡片主体内容；卡片须先经 <c>set_user_notify</c> 激活。
    /// </para>
    /// </remarks>
    [Post("/wxa/set_user_notifyext")]
    Task<WxaResponse> SetUserNotifyExtAsync(
        [Body] WxaSetUserNotifyExtRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询服务卡片状态。官方文档：<c>mp-message-management/subscribe-message/api_getusernotify.html</c>。
    /// </summary>
    /// <param name="request">卡片定位（<c>openid</c> / <c>template_id</c>），见 <see cref="WxaGetUserNotifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>卡片激活状态（<c>notify_status</c>），见 <see cref="WxaGetUserNotifyResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/get_user_notify</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>状态语义</b>：<c>notify_status</c> = <c>1</c> 已激活 / <c>0</c> 未激活（数值以官方页面为准，SDK 不做本地映射）。</para>
    /// </remarks>
    [Post("/wxa/get_user_notify")]
    Task<WxaGetUserNotifyResponse> GetUserNotifyAsync(
        [Body] WxaGetUserNotifyRequest request,
        CancellationToken cancellationToken = default);
}