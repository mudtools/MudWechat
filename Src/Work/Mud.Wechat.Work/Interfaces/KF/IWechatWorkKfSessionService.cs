// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「微信客服」模块会话分配与消息收发域公共 SDK
/// （会话状态获取与变更 + 发送消息 10 种 msgtype + 发送欢迎语等事件响应消息 2 种 msgtype）。
/// <para>
/// 官方对三类应用开放完全一致的 4 条路由（14 个端点方法，发送类路由按 msgtype 一方法一请求体收敛），
/// 全部声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalKfSessionService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyKfSessionService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderKfSessionService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「微信客服-可调用接口的应用」中；
/// 第三方 / 代开发应用须具有「微信客服-&gt;管理账号、分配会话和收发消息」权限；
/// 只能通过 API 管理企业指定的客服账号，接待人员应在应用的可见范围内；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// 官方约束：会话状态 4（已结束/未开始）不允许通过 API 变更；目标接待人员未在企业微信激活返回错误码 95014；
/// 发送消息仅当客户处于「新接入待处理」或「由智能助手接待」状态时可调用，且客户发消息后 48 小时内
/// 企业最多可下发 5 条；接口返回成功不代表消息最终发送成功，须关注「消息发送失败事件」回调；
/// msgid 须在客服账号内唯一；事件响应消息 code 仅可使用一次，欢迎语 / 结束会话场景有效期仅 20 秒；
/// 第三方 / 代开发应用的 userid 一律为密文 userid（即 open_userid）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkKfSessionService
{
    /// <summary>
    /// 获取会话状态
    /// <para>获取客户与指定客服账号的会话状态（0 - 未处理，1 - 由智能助手接待，
    /// 2 - 待接入池排队中，3 - 由人工接待，4 - 已结束/未开始）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetKfServiceStateRequest"/>：open_kfid / external_userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会话状态（service_state）与接待人员 userid（仅状态 3 时有效）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94669"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94698"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96425"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/service_state/get")]
    Task<GetKfServiceStateResponse> GetServiceStateAsync(
        [Body] GetKfServiceStateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 变更会话状态
    /// <para>变更客户与指定客服账号的会话状态（分配接待人员 / 转待接入池 / 结束会话）；
    /// 状态 4 不允许通过 API 变更；service_state = 3 时 servicer_userid 必填且须「正在接待」中。</para>
    /// <para>会话初次变更为状态 2 / 3 时返回回复语 code，变更为状态 4 时返回结束语 code，
    /// 可用于「发送欢迎语等事件响应消息」接口。</para>
    /// </summary>
    /// <param name="request">变更请求体（<see cref="TransitKfServiceStateRequest"/>：open_kfid / external_userid / service_state / servicer_userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>事件响应消息 code（msg_code）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94669"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94698"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96425"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/service_state/trans")]
    Task<TransitKfServiceStateResponse> TransitServiceStateAsync(
        [Body] TransitKfServiceStateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送文本消息
    /// <para>msgtype = <c>text</c>：向客户发送文本消息；content 最长 2048 个字节，超出截断。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfTextMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendTextMessageAsync(
        [Body] SendKfTextMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送图片消息
    /// <para>msgtype = <c>image</c>：向客户发送图片消息；media_id 通过上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfImageMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendImageMessageAsync(
        [Body] SendKfImageMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送语音消息
    /// <para>msgtype = <c>voice</c>：向客户发送语音消息；media_id 通过上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfVoiceMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendVoiceMessageAsync(
        [Body] SendKfVoiceMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送视频消息
    /// <para>msgtype = <c>video</c>：向客户发送视频消息；media_id 通过上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfVideoMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendVideoMessageAsync(
        [Body] SendKfVideoMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送文件消息
    /// <para>msgtype = <c>file</c>：向客户发送文件消息；media_id 通过上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfFileMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendFileMessageAsync(
        [Body] SendKfFileMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送链接消息
    /// <para>msgtype = <c>link</c>：向客户发送链接消息；url 须含 http / https 协议头。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfLinkMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendLinkMessageAsync(
        [Body] SendKfLinkMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送小程序消息
    /// <para>msgtype = <c>miniprogram</c>：向客户发送小程序消息；pagepath 须以 .html 为后缀。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfMiniProgramMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendMiniProgramMessageAsync(
        [Body] SendKfMiniProgramMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送菜单消息
    /// <para>msgtype = <c>msgmenu</c>：向客户发送菜单消息；菜单项不超过 50 个（其中 click / view / miniprogram
    /// 合计不超过 10 个），用户点击 click 菜单后会自动回复一条文本消息并附带菜单 ID。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfMenuMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendMenuMessageAsync(
        [Body] SendKfMenuMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送位置消息
    /// <para>msgtype = <c>location</c>：向客户发送位置消息；latitude / longitude 为官方必填。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfLocationMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendLocationMessageAsync(
        [Body] SendKfLocationMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送获客链接消息
    /// <para>msgtype = <c>ca_link</c>：向客户发送获客链接名片；
    /// 经此添加不计入获客助手「打开链接的客户数」统计。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfCaLinkMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94677"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94700"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96427"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg")]
    Task<SendKfMsgResponse> SendCaLinkMessageAsync(
        [Body] SendKfCaLinkMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送事件响应文本消息
    /// <para>msgtype = <c>text</c>：响应欢迎语、排队提示、接入提示、结束语等事件；
    /// code 由事件回调下发、仅可使用一次，欢迎语 / 结束会话场景有效期仅 20 秒，须及时调用。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfEventTextMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95122"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94910"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96428"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg_on_event")]
    Task<SendKfMsgResponse> SendTextMsgOnEventAsync(
        [Body] SendKfEventTextMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送事件响应菜单消息
    /// <para>msgtype = <c>msgmenu</c>：响应欢迎语、结束会话（满意度）等事件；
    /// 菜单项不超过 10 个；code 由事件回调下发、仅可使用一次，有效期仅 20 秒，须及时调用。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SendKfEventMenuMsgRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息 ID（msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95122"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94910"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96428"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/send_msg_on_event")]
    Task<SendKfMsgResponse> SendMenuMsgOnEventAsync(
        [Body] SendKfEventMenuMsgRequest request,
        CancellationToken cancellationToken = default);
}
