// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块消息推送（群发）域公共 SDK
/// （企业群发管理 + 群发记录与执行结果 + 新客户欢迎语 + 入群欢迎语素材管理）。
/// <para>
/// 官方对三类应用开放完全一致的 11 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactGroupMsgService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactGroupMsgService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactGroupMsgService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：群发端点要求自建应用配置到「可调用应用」列表中，第三方 / 代开发应用须具有
/// 「企业客户权限-&gt;客户联系-&gt;群发消息给客户和客户群」权限；新客户欢迎语须具有「给客户发送欢迎语」权限；
/// 入群欢迎语素材须具有「配置入群欢迎语素材」权限。自建应用只能给应用可见范围内的成员推送，
/// 调用时只返回应用可见范围内用户的发送情况。
/// </para>
/// <para>
/// 官方约束：每位客户或客户群每月最多接收的群发条数为当月天数；成员需在企业微信 2.7.5 及以上终端
/// 确认后才会真正发送；2023-12-01 起不再支持系统应用 secret 调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactGroupMsgService
{
    /// <summary>
    /// 创建企业群发
    /// <para>创建企业群发任务（发给客户或客户群），成员确认后才会真正发送，返回群发消息 id。</para>
    /// <para>文本与附件不能同时为空；客户群发时 sender、external_userid、tag_filter 不可同时为空，
    /// 指定 external_userid 后 tag_filter 不生效；每位客户或客户群每月最多接收条数为当月天数。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="AddGroupMsgTemplateRequest"/>：chat_type / external_userid / chat_id_list / tag_filter 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群发消息 id（msgid）与无效 / 无法发送的目标列表（fail_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92135"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92698"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96366"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/add_msg_template")]
    Task<AddGroupMsgTemplateResponse> AddGroupMsgTemplateAsync(
        [Body] AddGroupMsgTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提醒成员群发
    /// <para>重新触发群发通知，提醒成员完成群发任务。</para>
    /// <para>24 小时内每个群发最多触发三次提醒。</para>
    /// </summary>
    /// <param name="request">提醒请求体（<see cref="RemindGroupMsgSendRequest"/>：msgid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提醒结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97610"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97613"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97618"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/remind_groupmsg_send")]
    Task<WechatWorkResponse> RemindGroupMsgSendAsync(
        [Body] RemindGroupMsgSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止企业群发
    /// <para>停止无需成员继续发送的企业群发任务。</para>
    /// <para><b>无法撤回已经群发给客户的消息</b>。</para>
    /// </summary>
    /// <param name="request">停止请求体（<see cref="CancelGroupMsgSendRequest"/>：msgid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>停止结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97611"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97614"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97619"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/cancel_groupmsg_send")]
    Task<WechatWorkResponse> CancelGroupMsgSendAsync(
        [Body] CancelGroupMsgSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取群发记录列表
    /// <para>分页获取企业的全部群发记录（含文本与附件内容）。</para>
    /// <para>起止时间间隔不能超过 1 个月。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupMsgListRequest"/>：chat_type / start_time / end_time / creator 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群发记录列表（group_msg_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93338"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93439"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96355"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_groupmsg_list_v2")]
    Task<GetGroupMsgListResponse> GetGroupMsgListAsync(
        [Body] GetGroupMsgListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取群发成员发送任务列表
    /// <para>获取指定群发消息下各成员的发送任务与状态。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupMsgTaskListRequest"/>：msgid / limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群发成员发送任务列表（task_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93338"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93439"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96355"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_groupmsg_task")]
    Task<GetGroupMsgTaskListResponse> GetGroupMsgTaskListAsync(
        [Body] GetGroupMsgTaskListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业群发成员执行结果
    /// <para>获取指定成员发送指定群发消息的逐客户 / 逐群执行结果。</para>
    /// <para>2020-11-17 之前创建的消息无发送任务列表，需通过获取企业群发成员执行结果旧接口获取。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupMsgSendResultRequest"/>：msgid / userid / limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群成员发送结果列表（send_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93338"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93439"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96355"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_groupmsg_send_result")]
    Task<GetGroupMsgSendResultResponse> GetGroupMsgSendResultAsync(
        [Body] GetGroupMsgSendResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送新客户欢迎语
    /// <para>向新添加的客户发送欢迎语（文本与附件可同时发送，以多条消息触达）。</para>
    /// <para><b>仅可在收到「添加外部联系人事件」后 20 秒内调用，且只可调用一次</b>；
    /// 管理端已配置可用欢迎语时事件不返回 welcome_code；欢迎语已下发后再调用返回 41051（无需重试）；
    /// 多应用同时调用时仅最先调用者成功，其余返回 41096（可重试，不代表下发成功）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendWelcomeMsgRequest"/>：welcome_code / text / attachments）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92137"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92599"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96356"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/send_welcome_msg")]
    Task<WechatWorkResponse> SendWelcomeMsgAsync(
        [Body] SendWelcomeMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加入群欢迎语素材
    /// <para>为「加入群聊」配置入群欢迎语素材，返回素材 id。</para>
    /// <para>文本 / 图片 / 图文 / 小程序 / 文件 / 视频不能全为空；文本以外的类型只能配置一个，
    /// 同时填写多个时按 image &gt; link &gt; miniprogram &gt; file &gt; video 优先级取参。</para>
    /// </summary>
    /// <param name="request">添加请求体（<see cref="AddGroupWelcomeTemplateRequest"/>：text / image / link / miniprogram 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>欢迎语素材 id（template_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92366"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93438"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96357"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/group_welcome_template/add")]
    Task<AddGroupWelcomeTemplateResponse> AddGroupWelcomeTemplateAsync(
        [Body] AddGroupWelcomeTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑入群欢迎语素材
    /// <para>编辑入群欢迎语素材内容。仅可编辑本应用创建的入群欢迎语素材。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="UpdateGroupWelcomeTemplateRequest"/>：template_id + 素材字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92366"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93438"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96357"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/group_welcome_template/edit")]
    Task<WechatWorkResponse> UpdateGroupWelcomeTemplateAsync(
        [Body] UpdateGroupWelcomeTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取入群欢迎语素材
    /// <para>通过素材 id 获取入群欢迎语素材的完整内容。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupWelcomeTemplateRequest"/>：template_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>入群欢迎语素材内容（text / image / link / miniprogram / file / video 根级字段）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92366"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93438"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96357"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/group_welcome_template/get")]
    Task<GetGroupWelcomeTemplateResponse> GetGroupWelcomeTemplateAsync(
        [Body] GetGroupWelcomeTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除入群欢迎语素材
    /// <para>删除入群欢迎语素材。仅能删除调用方自己创建的素材。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteGroupWelcomeTemplateRequest"/>：template_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92366"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93438"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96357"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/group_welcome_template/del")]
    Task<WechatWorkResponse> DeleteGroupWelcomeTemplateAsync(
        [Body] DeleteGroupWelcomeTemplateRequest request,
        CancellationToken cancellationToken = default);
}
