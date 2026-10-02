// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「消息推送」模块家校消息推送域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （发送「学校通知」，支持文本 / 图片 / 语音 / 视频 / 文件 / 图文 / mpnews / 小程序八种消息类型），全部声明于本接口。
/// <para>第三方应用与服务商代开发官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 学校管理员需要将应用配置在「家长可使用的应用」才可调用；
/// 通过 recv_scope / to_parent_userid / to_student_userid / to_party / toall 指定接收对象
/// （家长 / 学生 / 家长和学生三态，单次最多 1000 个家长或学生、100 个班级）；
/// 部分接收人无权限或不存在时发送仍执行，但会返回无效部分（invalid_parent_userid /
/// invalid_student_userid / invalid_party）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Message",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSchoolMessageService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalSchoolMessageService : IWechatWorkSchoolMessageService
{
    /// <summary>
    /// 发送文本学校通知
    /// <para>msgtype = <c>text</c>：消息内容最长不超过 2048 个字节，超过将截断（支持 id 转译）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendTextRequest"/>：学校通知信封 + text 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolTextMessageAsync(
        [Body] SchoolSendTextRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送图片学校通知
    /// <para>msgtype = <c>image</c>：图片媒体文件经上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendImageRequest"/>：学校通知信封 + image 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolImageMessageAsync(
        [Body] SchoolSendImageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送语音学校通知
    /// <para>msgtype = <c>voice</c>：语音媒体文件经上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendVoiceRequest"/>：学校通知信封 + voice 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolVoiceMessageAsync(
        [Body] SchoolSendVoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送视频学校通知
    /// <para>msgtype = <c>video</c>：标题不超过 128 字节、描述不超过 512 字节，超过自动截断。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendVideoRequest"/>：学校通知信封 + video 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolVideoMessageAsync(
        [Body] SchoolSendVideoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送文件学校通知
    /// <para>msgtype = <c>file</c>：文件经上传临时素材接口获取。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendFileRequest"/>：学校通知信封 + file 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolFileMessageAsync(
        [Body] SchoolSendFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送图文学校通知
    /// <para>msgtype = <c>news</c>：一个图文消息支持 1 到 8 条图文，articles[].url 必填（支持 id 转译）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendNewsRequest"/>：学校通知信封 + news 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolNewsMessageAsync(
        [Body] SchoolSendNewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送 mpnews 图文学校通知
    /// <para>msgtype = <c>mpnews</c>：与普通图文一致，唯一差异是图文内容存储在企业微信；
    /// 多次发送会被认为是不同的图文，阅读、点赞统计分开计算（支持 id 转译）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendMpNewsRequest"/>：学校通知信封 + mpnews 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolMpNewsMessageAsync(
        [Body] SchoolSendMpNewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送小程序学校通知
    /// <para>msgtype = <c>miniprogram</c>：appid 必须是关联到企业的小程序应用；
    /// 封面 thumb_media_id 建议尺寸 520*416（支持 id 转译）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="SchoolSendMiniProgramRequest"/>：学校通知信封 + miniprogram 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invalid_parent_userid / invalid_student_userid / invalid_party）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91609"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/message/send")]
    Task<SchoolMessageSendResponse> SendSchoolMiniProgramMessageAsync(
        [Body] SchoolSendMiniProgramRequest request,
        CancellationToken cancellationToken = default);
}
