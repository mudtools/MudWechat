// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「消息推送」模块群聊会话域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （创建群聊会话 / 修改群聊会话 / 获取群聊会话 / 应用推送消息到群聊会话），全部声明于本接口。
/// <para>第三方应用官方明示不可调用，服务商代开发无对应文档，均不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 应用可见范围必须为根部门；修改 / 获取 / 推送消息要求群为该应用所创建。
/// 限频：创建 1000 群/天；修改 1000 次/小时；推送消息每企业 2 万人次/分
/// （群 100 人，每发一次算 100 人次）＋企业规模分档小时额度（未认证或小型 ≤15 万、
/// 中型 ≤35 万、大型 ≤70 万人次/小时）；每个成员在群中收到的同一应用消息不可超过
/// 200 条/分、1 万条/天，超过部分被丢弃且接口不报错（推送成功不等于全员送达）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Message",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAppChatService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalAppChatService : IWechatWorkAppChatService
{
    /// <summary>
    /// 创建群聊会话
    /// <para>chatid 不填则系统随机生成；群成员至少 2 人、至多 2000 人（含应用），
    /// 且不可超过管理端配置的「群成员人数上限」；每企业创建群数不可超过 1000 个/天。</para>
    /// <para>刚创建的群如果没有下发消息，在旧版本企业微信终端上可能不会出现该群。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="CreateAppChatRequest"/>：name / owner / userlist / chatid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群聊的唯一标志（chatid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90245"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/create")]
    Task<CreateAppChatResponse> CreateAppChatAsync(
        [Body] CreateAppChatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改群聊会话
    /// <para>chatid 所代表的群必须是该应用所创建；每企业变更群的次数不可超过 1000 次/小时。</para>
    /// <para>课程群聊群主必须拥有课程群创建权限，del_user_list 包含群主时 owner 必填。</para>
    /// </summary>
    /// <param name="request">修改请求体（<see cref="UpdateAppChatRequest"/>：chatid + name / owner / add_user_list / del_user_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98913"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/update")]
    Task<UpdateAppChatResponse> UpdateAppChatAsync(
        [Body] UpdateAppChatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取群聊会话
    /// <para>chatid 所代表的群必须是该应用所创建；返回群聊的群名、群主、成员列表与群聊类型。</para>
    /// </summary>
    /// <param name="chatId">群聊 id（创建群聊会话时指定或系统随机生成的 chatid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群聊会话信息（chat_info：chatid / name / owner / userlist / chat_type）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98914"/></para>
    /// </remarks>
    [Get("/cgi-bin/appchat/get")]
    Task<GetAppChatResponse> GetAppChatAsync(
        [Query("chatid")] string chatId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送文本消息到群聊会话
    /// <para>msgtype = <c>text</c>：content 最长不超过 2048 个字节；
    /// 支持 mentioned_list 提醒群中指定成员（@all 提醒所有人），
    /// 支持 <c>&lt;@userid&gt;</c> 扩展语法 @群成员（企业微信 5.0.6 及以上版本支持）。</para>
    /// <para>chatid 所代表的群必须是该应用所创建；每企业消息发送量不可超过 2 万人次/分；
    /// 每个成员在群中收到的同一应用消息不可超过 200 条/分、1 万条/天，超过部分被丢弃且接口不报错。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendTextRequest"/>：chatid + text 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatTextMessageAsync(
        [Body] AppChatSendTextRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送图片消息到群聊会话
    /// <para>msgtype = <c>image</c>：图片媒体文件经上传临时素材接口获取（支持 JPG、PNG）。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendImageRequest"/>：chatid + image 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatImageMessageAsync(
        [Body] AppChatSendImageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送语音消息到群聊会话
    /// <para>msgtype = <c>voice</c>：语音媒体文件经上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendVoiceRequest"/>：chatid + voice 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatVoiceMessageAsync(
        [Body] AppChatSendVoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送视频消息到群聊会话
    /// <para>msgtype = <c>video</c>：标题不超过 128 字节、描述不超过 512 字节，超过自动截断。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendVideoRequest"/>：chatid + video 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatVideoMessageAsync(
        [Body] AppChatSendVideoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送文件消息到群聊会话
    /// <para>msgtype = <c>file</c>：文件经上传临时素材接口获取；
    /// 保密消息仅支持 txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif 格式。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendFileRequest"/>：chatid + file 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatFileMessageAsync(
        [Body] AppChatSendFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送文本卡片消息到群聊会话
    /// <para>msgtype = <c>textcard</c>：标题、描述、跳转链接必填；
    /// 描述支持 br 标签或空格换行、div 标签 class 内置 gray/highlight/normal 三种文字颜色。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendTextCardRequest"/>：chatid + textcard 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatTextCardMessageAsync(
        [Body] AppChatSendTextCardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送图文消息到群聊会话
    /// <para>msgtype = <c>news</c>：一个图文消息支持 1 到 8 条图文。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendNewsRequest"/>：chatid + news 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatNewsMessageAsync(
        [Body] AppChatSendNewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送 mpnews 图文消息到群聊会话
    /// <para>msgtype = <c>mpnews</c>：与普通图文一致，唯一差异是图文内容存储在企业微信；
    /// 多次发送会被认为是不同的图文，阅读、点赞统计分开计算。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendMpNewsRequest"/>：chatid + mpnews 消息体 + safe）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatMpNewsMessageAsync(
        [Body] AppChatSendMpNewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 应用推送 markdown 消息到群聊会话
    /// <para>msgtype = <c>markdown</c>：仅支持 markdown 语法的子集；
    /// 支持 <c>&lt;@userid&gt;</c> 扩展语法 @群成员（企业微信 5.0.6 及以上版本支持）；
    /// 微信插件（原企业号）不支持展示 markdown 消息。</para>
    /// </summary>
    /// <param name="request">推送请求体（<see cref="AppChatSendMarkdownRequest"/>：chatid + markdown 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推送结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90248"/></para>
    /// </remarks>
    [Post("/cgi-bin/appchat/send")]
    Task<AppChatSendResponse> SendAppChatMarkdownMessageAsync(
        [Body] AppChatSendMarkdownRequest request,
        CancellationToken cancellationToken = default);
}
