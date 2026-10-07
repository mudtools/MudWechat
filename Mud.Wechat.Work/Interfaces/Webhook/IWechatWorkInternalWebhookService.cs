// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Webhook;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「群机器人」模块 Webhook 推送 SDK：官方全部 9 个端点声明于本接口
/// （<c>msgtype</c> 逐类型各一方法 + 媒体上传，同路由多方法范式对齐 <c>/cgi-bin/message/send</c>，
/// 不做运行时多态、不引入泛型 msgtype）。
/// </summary>
/// <remarks>
/// <para>
/// <b>无令牌</b>：本接口<b>不声明 <c>[Token]</c></b> —— 端点以 URL Query 上的 <c>key</c> 为机器人凭据
/// （官方 91770），不属于 <c>access_token</c> / <c>suite_access_token</c> / <c>provider_access_token</c>
/// 任一令牌链路；故也不进入通用守卫 G5（Query 令牌注入白名单）。组件分析器据此场景发出的
/// HTTPCLIENT018（建议补 <c>[Token]</c>）为不适用告警，已在声明处局部 <c>#pragma</c> 豁免；
/// <c>TokenManage</c> 与父接口保持一致（全仓父接口恒为 <c>nameof(IWechatAppManager)</c>），
/// 防止生成器以 <c>new</c> 隐藏基类切换成员（HTTPCLIENT028）。
/// </para>
/// <para>
/// <b>开放面</b>：群机器人由用户在群聊设置中添加、凭据（Webhook URL 及其 <c>key</c>）由官方直接颁发，
/// 与应用的令牌体系无关，官方文档不区分自建 / 第三方 / 代开发 ⇒ 继承链上仅本接口一层承载端点
/// （无更深层应用类型子接口）。
/// </para>
/// <para>
/// <b>官方凭证脱敏缺口</b>：Query 参数 <c>key</c> 为长期有效凭据、不在组件 <c>SensitiveUrlRedactor</c>
/// 词表内，已按「豁免（附追踪号）」路径登记（追踪号 WEBHOOK-KEY-REDACT-01，审计断言见守卫 WEB3）。
/// </para>
/// </remarks>
// HTTPCLIENT018 豁免理由：同父接口 IWechatWorkWebhookService——本域为「无令牌端点」既存例外
//（官方 91770 以 URL Query 上的 key 为机器人凭据，守卫 WEB3 锁定不得声明 [Token]），
// 分析器唯一消警路径与本域契约冲突；此处仅本接口局部豁免，不影响全仓其余接口的 018 检查。
#pragma warning disable HTTPCLIENT018
[HttpClientApi(RegistryGroupName = "Webhook",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkWebhookService))]
public interface IWechatWorkInternalWebhookService : IWechatWorkWebhookService
{
#pragma warning restore HTTPCLIENT018
    /// <summary>
    /// 群机器人发送文本消息（<c>msgtype=text</c>）
    /// <para>支持的 markdown/模板卡片等其余消息类型见本接口各 <c>Send*Async</c> 方法（同路由逐 msgtype 一方法）。</para>
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">
    /// 请求体（<see cref="WechatWebhookTextRequest"/>：msgtype / text.content ≤ 2048 字节 /
    /// text.mentioned_list 仅群聊可用、最多 20 人 / text.mentioned_mobile_list 仅群聊可用、最多 20 人）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：<c>text.content</c> 最长 2048 个字节（超出报错）；<c>@text.mentioned_list</c> 为 userid 列表、
    /// <c>@text.mentioned_mobile_list</c> 为手机号列表（支持 <c>@all</c>），二者<b>仅在群聊中生效</b>、且最多 20 人。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendTextAsync(
        [Query("key")] string key,
        [Body] WechatWebhookTextRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人发送 markdown 消息（<c>msgtype=markdown</c>）
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">请求体（<see cref="WechatWebhookMarkdownRequest"/>：msgtype / markdown.content ≤ 4096 字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：<c>markdown.content</c> 最长 4096 个字节（超出报错）；支持的 markdown 语法为官方子集
    /// （标题、加粗、斜体、链接、行内代码、引用、字体颜色 green/info/comment/warning、换行等）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendMarkdownAsync(
        [Query("key")] string key,
        [Body] WechatWebhookMarkdownRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人发送 markdown_v2 消息（<c>msgtype=markdown_v2</c>）
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">请求体（<see cref="WechatWebhookMarkdownV2Request"/>：msgtype / markdown_v2.content ≤ 4096 字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：<c>markdown_v2.content</c> 最长 4096 个字节（超出报错）；markdown_v2 为增强版 markdown，
    /// 支持无序列表、有序列表、引用、代码块、任务列表、表格等扩展语法（与 markdown 消息的语法集不同，
    /// 二者不可互相套用）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendMarkdownV2Async(
        [Query("key")] string key,
        [Body] WechatWebhookMarkdownV2Request request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人发送图片消息（<c>msgtype=image</c>）
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">请求体（<see cref="WechatWebhookImageRequest"/>：msgtype / image.base64 与 image.md5 官方均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：图片（<c>base64</c> 编码内容 + <c>md5</c> 值，二者均必填）大小不超过 2MB，
    /// 仅支持 JPG/PNG 格式（md5 为图片内容的十六进制摘要，用于官方校验）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendImageAsync(
        [Query("key")] string key,
        [Body] WechatWebhookImageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人发送图文消息（<c>msgtype=news</c>）
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">请求体（<see cref="WechatWebhookNewsRequest"/>：msgtype / news.articles 1~8 条）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：图文条数（<c>news.articles</c>）1~8 条；<c>picurl</c> 必须是<b>公网可访问</b>的完整图片链接
    /// （http/https，否则官方展示为空白图）；点击整条图文跳转 <c>url</c>。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendNewsAsync(
        [Query("key")] string key,
        [Body] WechatWebhookNewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人发送文件消息（<c>msgtype=file</c>）
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">请求体（<see cref="WechatWebhookFileRequest"/>：msgtype / file.media_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：<c>file.media_id</c> 须先经 <see cref="UploadMediaAsync"/>（type=file）上传获得，
    /// media_id 仅 <b>3 天内</b>有效。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendFileAsync(
        [Query("key")] string key,
        [Body] WechatWebhookFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人发送语音消息（<c>msgtype=voice</c>）
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">请求体（<see cref="WechatWebhookVoiceRequest"/>：msgtype / voice.media_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：<c>voice.media_id</c> 须先经 <see cref="UploadMediaAsync"/>（type=voice）上传获得，
    /// 语音文件大小不超过 2MB、播放长度不超过 60s、仅支持 AMR 格式；media_id 仅 <b>3 天内</b>有效。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendVoiceAsync(
        [Query("key")] string key,
        [Body] WechatWebhookVoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人发送模板卡片消息（<c>msgtype=template_card</c>）
    /// <para><b>频率限制</b>：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="request">请求体（<see cref="WechatWebhookTemplateCardRequest"/>：msgtype / template_card 扁平卡片结构，与发送应用消息的 <c>TemplateCardBody</c> 共用）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：模板卡片字段与 <c>/cgi-bin/message/send</c> 的 template_card 一致（共用
    /// <see cref="Mud.Wechat.Work.DataModels.Message.TemplateCardBody"/> 扁平结构）；
    /// 群机器人模板卡片不支持 <c>task_id</c> 驱动的按钮回调更新链路之外的应用级配置。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/send")]
    Task<WechatWebhookSendResponse> SendTemplateCardAsync(
        [Query("key")] string key,
        [Body] WechatWebhookTemplateCardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群机器人上传媒体文件（<c>/cgi-bin/webhook/upload_media</c>）
    /// <para>用于发送文件 / 语音消息前先上传素材，获取 media_id（multipart/form-data，文件标识名必须为 <c>media</c>，
    /// 须包含 filename、filelength、content-type 等信息）。</para>
    /// </summary>
    /// <param name="key">机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。</param>
    /// <param name="mediaType">媒体文件类型（官方必填）：file - 普通文件，voice - 语音。</param>
    /// <param name="formData">multipart 表单内容（文件字段名须为 <c>media</c>，由调用方构建
    /// <see cref="IFormContent"/> 实现，含 filename / filelength / content-type 信息）。
    /// 须标 <c>[MultipartForm]</c> —— 生成器 3.0.1 对 <c>[FormContent]</c> 发射的
    /// <c>GetFormDataContentAsync</c> 调用在运行时接口上不存在（CS1061），
    /// <c>[MultipartForm]</c> 才走 <see cref="IFormContent.ToHttpContentAsync"/> 通路。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒体文件类型（type）、素材 id（media_id，三天有效）与上传时间戳（created_at）。</returns>
    /// <remarks>
    /// <para><b>群机器人·消息推送配置说明</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91770"/></para>
    /// <para>
    /// 官方约束：文件（file）大小要求 5B ~ 20M；语音（voice）大小不超过 2MB、播放长度不超过 60s、仅支持 AMR 格式；
    /// 返回的 media_id 仅 <b>3 天内</b>有效。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/webhook/upload_media")]
    Task<WechatWebhookUploadMediaResponse> UploadMediaAsync(
        [Query("key")] string key,
        [Query("type")] string mediaType,
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);
}
