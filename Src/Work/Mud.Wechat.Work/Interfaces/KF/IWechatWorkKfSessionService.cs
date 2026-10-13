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
/// （会话状态获取与变更 + 同步消息和事件 + 发送消息 10 种 msgtype + 发送欢迎语等事件响应消息 2 种 msgtype）。
/// <para>
/// 官方对三类应用开放完全一致的 5 条路由（15 个端点方法，发送类路由按 msgtype 一方法一请求体收敛），
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
    /// 同步消息和事件
    /// <para>拉取客户发送的消息、企业发送的消息以及会话相关事件（按发生顺序返回）。
    /// 入参 <c>token</c> 由「微信客服事件通知」回调下发，与调用本接口所用的 <c>access_token</c>
    /// 是两个不同概念的令牌，不可互相代替；首次请求不传 <c>cursor</c>，
    /// 其后逐页以上一次响应的 <c>next_cursor</c> 继续请求，直到 <c>has_more</c> 为 0
    /// （官方以数字 0/1 承载该开关，DTO 原样以 <c>int</c> 承载）。</para>
    /// <para>响应消息体按 <c>msgtype</c> 判别载体（text / image / voice / video / file / link /
    /// miniprogram / msgmenu / location / business_card / event / channels /
    /// channels_shop_product / channels_shop_order / merged_msg），
    /// <c>origin</c> 判别消息来源（客户发送、系统自动发送、机器人发送、人工发送等）。</para>
    /// </summary>
    /// <param name="request">同步请求体（<see cref="SyncKfMessageRequest"/>：open_kfid 官方<b>必填</b>，token / cursor / limit / voice_format 官方均为可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息列表（msg_list）与翻页标记（has_more / next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94670"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94699"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96426"/></para>
    /// <para>官方权限（三页同口径的权限表原文）：自建应用「配置到「微信客服 - 可调用接口的应用」中」；
    /// 第三方应用与代开发自建应用「具有“微信客服-&gt;管理账号、分配会话和收发消息”权限」。
    /// 注：从 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口，存量企业暂不受影响。
    /// 另两条官方硬约束：只能通过 API 管理企业指定的客服账号（企业需在管理后台「微信客服 - 通过API管理微信客服账号」处开启），
    /// 且操作的客服账号对应的接待人员应在应用的可见范围内。</para>
    /// <para>官方限制（本页正文口径）：本接口为 POST，可获取<b>最近 3 天内</b>的具体消息内容和事件，
    /// <b>不支持</b>读取通过发送消息接口发送的消息；图片/语音/视频/文件的媒体大小限制为
    /// <b>2MB / 2MB / 10MB / 20MB</b>，超出时官方改为返回文本提示消息；
    /// <c>cursor</c> 官方不多于 64 字节，不填则从 3 天内最早的消息开始返回；
    /// <c>token</c> 官方不多于 128 字节、「回调事件返回的 token 字段，10 分钟内有效；<b>可不填，如果不填接口有严格的频率限制</b>」；
    /// <c>limit</c> 的<b>默认值和最大值都为 1000</b>，且官方明确「可能会出现返回条数少于 limit 的情况」；
    /// <c>voice_format</c> 取 0-Amr、1-Silk，默认 0。</para>
    /// <para>官方翻页判据：<c>has_more</c> 为 uint32 的 0-否 / 1-是（本仓原样以 <c>int</c> 承载），
    /// 官方明令「<b>不能通过判断 msg_list 是否空来停止拉取</b>，可能会出现 has_more 为 1 而 msg_list 为空的情况」；
    /// <c>next_cursor</c> 官方强烈建议入库保存（每次请求带上、请求结束后更新，意外丢失会导致从头拉取而延迟消息）。</para>
    /// <para>本页未给出独立的数值型频率限制（唯一与频率相关的官方口径是「不传 token 时有严格的频率限制」），
    /// 默认阈值走官方「附录 - 访问频率限制」<see href="https://developer.work.weixin.qq.com/document/path/90312"/>；
    /// 本端点<b>不</b>写「接口调用频率限制为 N 次/分钟」——不得凭第三方 SDK 注释或记忆补数。</para>
    /// </remarks>
    [Post("/cgi-bin/kf/sync_msg")]
    Task<SyncKfMessageResponse> SyncMessageAsync(
        [Body] SyncKfMessageRequest request,
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
