// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「消息推送」模块发送应用消息族公共 SDK
/// （发送应用消息 / 更新模版卡片消息 / 撤回应用消息）。
/// <para>
/// 官方对企业自建应用（90236 / 94888 / 94867）、第三方应用（90372 / 94945 / 94947）、
/// 服务商代开发（96458 / 96459 / 96460）开放完全一致的 3 个端点，全部收敛声明于本接口；
/// 应用类型子接口：自建应用见 <see cref="IWechatWorkInternalMessageService"/>（空标记）、
/// 第三方应用见 <see cref="IWechatWorkThirdPartyMessageService"/>（另持 template_msg 模板消息差异端点，94515）、
/// 服务商代开发见 <see cref="IWechatWorkProviderMessageService"/>（空标记）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 限频口径：每应用不可超过「账号上限数 × 200」人次/天（一次 API 发给 1000 人算 1000 人次）；
/// 对同一成员不可超过 30 次/分钟、1000 次/小时，超过部分被丢弃不下发；
/// 官方建议避开每小时 0 分 / 30 分的集中推送高峰。
/// </para>
/// <para>
/// 接收人口径：<c>touser</c> / <c>toparty</c> / <c>totag</c> 不能同时为空；
/// 部分接收人无权限或不存在时发送仍执行并返回无效部分（invaliduser / invalidparty / invalidtag / unlicenseduser），
/// 常见原因是接收人不在应用可见范围内；全部接收人无权限或不存在则本次调用失败（errcode 81013）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMessageService
{
    /// <summary>
    /// 发送文本消息
    /// <para>msgtype = <c>text</c>：消息内容最长不超过 2048 个字节，超过将截断；支持 id 转译；
    /// 换行须使用转义的 <c>\n</c>，支持 <c>&lt;a href="..."&gt;</c> 标签打开自定义网页。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendTextRequest"/>：公共信封 + text 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid / response_code）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendTextMessageAsync(
        [Body] MessageSendTextRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送图片消息
    /// <para>msgtype = <c>image</c>：图片媒体文件经上传临时素材接口获取（支持 JPG、PNG）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendImageRequest"/>：公共信封 + image 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendImageMessageAsync(
        [Body] MessageSendImageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送语音消息
    /// <para>msgtype = <c>voice</c>：语音媒体文件经上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendVoiceRequest"/>：公共信封 + voice 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendVoiceMessageAsync(
        [Body] MessageSendVoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送视频消息
    /// <para>msgtype = <c>video</c>：标题不超过 128 字节、描述不超过 512 字节，超过自动截断。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendVideoRequest"/>：公共信封 + video 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendVideoMessageAsync(
        [Body] MessageSendVideoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送文件消息
    /// <para>msgtype = <c>file</c>：文件经上传临时素材接口获取；
    /// 保密消息仅支持 txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif 格式。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendFileRequest"/>：公共信封 + file 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendFileMessageAsync(
        [Body] MessageSendFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送文本卡片消息
    /// <para>msgtype = <c>textcard</c>：标题不超过 128 个字符、描述不超过 512 个字符，超过自动截断（支持 id 转译）；
    /// 描述支持 br 标签或空格换行、div 标签 class 内置 gray/highlight/normal 三种文字颜色；
    /// 跳转链接最长 2048 字节且须包含协议头（http/https）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendTextCardRequest"/>：公共信封 + textcard 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendTextCardMessageAsync(
        [Body] MessageSendTextCardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送图文消息
    /// <para>msgtype = <c>news</c>：一个图文消息支持 1 到 8 条图文；
    /// 小程序（appid + pagepath）或者 url 必须填写一个。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendNewsRequest"/>：公共信封 + news 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendNewsMessageAsync(
        [Body] MessageSendNewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送图文消息（mpnews）
    /// <para>msgtype = <c>mpnews</c>：与普通图文一致，唯一差异是图文内容存储在企业微信；
    /// 多次发送会被认为是不同的图文，阅读、点赞统计分开计算；
    /// safe 仅本类型额外支持取值 2（仅限在企业内分享）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendMpNewsRequest"/>：公共信封 + mpnews 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendMpNewsMessageAsync(
        [Body] MessageSendMpNewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送 markdown 消息
    /// <para>msgtype = <c>markdown</c>：仅支持 markdown 语法的子集（标题 / 加粗 / 链接 / 行内代码段 / 引用）；
    /// 字体颜色仅 info（绿）/ comment（灰）/ warning（橙红）三种内置；
    /// 微工作台（原企业号）不支持展示 markdown 消息。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendMarkdownRequest"/>：公共信封 + markdown 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendMarkdownMessageAsync(
        [Body] MessageSendMarkdownRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送小程序通知消息
    /// <para>msgtype = <c>miniprogram_notice</c>：只允许绑定了小程序的应用发送；
    /// 不支持 @all 全员发送；微工作台（原企业号）不支持展示；
    /// 2019-06-28 起用户收到的小程序通知会出现在各个独立的应用中。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendMiniProgramNoticeRequest"/>：公共信封 + miniprogram_notice 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendMiniProgramNoticeMessageAsync(
        [Body] MessageSendMiniProgramNoticeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送模板卡片消息
    /// <para>msgtype = <c>template_card</c>：卡片类型 card_type 取值
    /// text_notice（文本通知型）/ news_notice（图文展示型）/ button_interaction（按钮交互型）/
    /// vote_interaction（投票选择型）/ multiple_interaction（多项选择型）。</para>
    /// <para>版本要求：文本通知型 / 图文展示型 / 按钮交互型需企业微信 3.1.6 及以上（附件下载需 3.1.12），
    /// 投票选择型 / 多项选择型需 3.1.12 及以上，desc_color / horizontal_content_list type 3 /
    /// action_menu / quote_area / image_text_area / button_selection 需 3.1.18 及以上；
    /// 微工作台不支持展示模板卡片消息；没有配置回调接口的应用不可发送支持回调的卡片。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendTemplateCardRequest"/>：公共信封 + template_card 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（支持回调的卡片返回 response_code，72 小时内有效且只能使用一次）。</returns>
    /// <remarks>
    /// <para>发送后可经 <see cref="UpdateTemplateCardAsync"/> 以 response_code 更新卡片。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90236"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90372"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96458"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendTemplateCardMessageAsync(
        [Body] MessageSendTemplateCardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新模版卡片消息
    /// <para>仅原卡片为按钮交互型、投票选择型、多项选择型的卡片，
    /// 以及填写了 action_menu 字段的文本通知型、图文展示型可以调用本接口更新。</para>
    /// <para>支持两种模式：更新按钮为不可点击状态（button.replace_name），或更新为新的卡片（template_card）；
    /// response_code 可通过发送模版卡片消息接口与用户点击卡片的回调事件获取，
    /// 一个 code 只能调用一次且有效期 72 小时。</para>
    /// </summary>
    /// <param name="request">更新请求体（<see cref="UpdateTemplateCardRequest"/>：userids / partyids / tagids / atall + agentid + response_code + button 或 template_card）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（invaliduser 无效用户列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94888"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94945"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96459"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/update_template_card")]
    Task<UpdateTemplateCardResponse> UpdateTemplateCardAsync(
        [Body] UpdateTemplateCardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤回应用消息
    /// <para>撤回 24 小时内通过发送应用消息接口推送的消息；
    /// 仅可撤回企业微信端的数据，微信插件端的数据不支持撤回。</para>
    /// </summary>
    /// <param name="request">撤回请求体（<see cref="RecallMessageRequest"/>：msgid，从发送应用消息接口处获得）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>撤回结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94867"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94947"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96460"/></para>
    /// </remarks>
    [Post("/cgi-bin/message/recall")]
    Task<RecallMessageResponse> RecallMessageAsync(
        [Body] RecallMessageRequest request,
        CancellationToken cancellationToken = default);
}
