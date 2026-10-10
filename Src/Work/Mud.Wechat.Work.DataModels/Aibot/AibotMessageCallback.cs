// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Aibot;

/// <summary>
/// 智能机器人「接收消息」回调报文（明文 JSON）——<b>可空超集</b>，覆盖全部 7 种 <c>msgtype</c>。
/// </summary>
/// <remarks>
/// <para>
/// 官方 100719（回调地址模式）与 101463（长连接模式）的消息报文结构一致，差异仅在「值是否出现」
/// （长连接媒体结构体额外携带 <c>aeskey</c>）⇒ 一份可空超集覆盖两模式，<b>禁逐 msgtype 拆分类</b>。
/// </para>
/// <para>
/// 顶层 <c>msgtype</c> 取值：<c>text</c> / <c>image</c>（仅单聊）/ <c>mixed</c> / <c>voice</c>（仅单聊）/
/// <c>file</c>（仅单聊）/ <c>video</c>（仅单聊）/ <c>stream</c>（流式消息刷新，<b>无 <c>response_url</c></b>）。
/// </para>
/// <para>
/// 官方业务约束：<c>msgid</c> 为本次回调唯一标志，<b>需据此排重</b>（网络原因可能重复回调）；
/// 用户与同一机器人最多同时三条消息交互中；流式消息刷新自用户发消息起最多等待 6 分钟，超时结束推送；
/// <c>file</c> / <c>video</c> 官方仅支持 100M 以内。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotMessageCallback
{
    /// <summary>本次回调的唯一性标志（宿主据此排重；网络原因可能重复回调）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>智能机器人 id（= 后台配置的 BotID）。</summary>
    [JsonPropertyName("aibotid")]
    public string? AibotId { get; set; }

    /// <summary>会话 id（仅群聊类型时返回）。</summary>
    [JsonPropertyName("chatid")]
    public string? ChatId { get; set; }

    /// <summary>会话类型：<c>single</c> 单聊 / <c>group</c> 群聊。</summary>
    [JsonPropertyName("chattype")]
    public string? ChatType { get; set; }

    /// <summary>事件触发者信息。</summary>
    [JsonPropertyName("from")]
    public AibotFrom? From { get; set; }

    /// <summary>消息类型（<c>text</c> / <c>image</c> / <c>mixed</c> / <c>voice</c> / <c>file</c> / <c>video</c> / <c>stream</c>）。</summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>
    /// 支持主动回复消息的临时 url（<c>{msgtype}</c> 消息族携带）。
    /// </summary>
    /// <remarks>
    /// <b>流式消息刷新（<c>msgtype=stream</c>）不返回本字段</b>（官方 100719 参数表无该字段）；
    /// 每个 url 仅可调用一次、有效期 1 小时（官方 101138）。
    /// </remarks>
    [JsonPropertyName("response_url")]
    public string? ResponseUrl { get; set; }

    /// <summary>文本消息内容（<c>msgtype=text</c>）。</summary>
    [JsonPropertyName("text")]
    public AibotTextBody? Text { get; set; }

    /// <summary>图片内容（<c>msgtype=image</c>，仅单聊）。</summary>
    [JsonPropertyName("image")]
    public AibotMediaContent? Image { get; set; }

    /// <summary>图文混排内容（<c>msgtype=mixed</c>）。</summary>
    [JsonPropertyName("mixed")]
    public AibotMixedContent? Mixed { get; set; }

    /// <summary>语音内容（<c>msgtype=voice</c>，仅单聊；官方只提供<b>转换后的文本</b>，不提供音频下载 url）。</summary>
    [JsonPropertyName("voice")]
    public AibotVoiceContent? Voice { get; set; }

    /// <summary>文件内容（<c>msgtype=file</c>，仅单聊；官方仅支持 100M 以内）。</summary>
    [JsonPropertyName("file")]
    public AibotMediaContent? File { get; set; }

    /// <summary>视频内容（<c>msgtype=video</c>，仅单聊；官方仅支持 100M 以内）。</summary>
    [JsonPropertyName("video")]
    public AibotMediaContent? Video { get; set; }

    /// <summary>流式消息刷新标识（<c>msgtype=stream</c>；机器人按该 id 返回对应的流式消息）。</summary>
    [JsonPropertyName("stream")]
    public AibotStreamRefresh? Stream { get; set; }

    /// <summary>引用内容（用户引用了其他消息时才有该字段；<c>text</c> / <c>mixed</c> 消息族携带）。</summary>
    [JsonPropertyName("quote")]
    public AibotQuote? Quote { get; set; }
}

/// <summary>智能机器人事件触发者信息（<c>from</c>）。</summary>
/// <remarks>
/// 官方 101027：企业内部智能机器人<b>不返回</b> <c>corpid</c>；<c>userid</c> 为明文 userid
/// （机器人创建者为超管时）或<b>企业主体下的加密 userid</b>（否则，需经「自建应用与智能机器人的对接」转明文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotFrom
{
    /// <summary>操作者的 corpid（企业内部智能机器人不返回）。</summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>操作者的 userid（可能为企业主体下的加密 userid）。</summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }
}

/// <summary>
/// 智能机器人媒体内容（<c>image</c> / <c>file</c> / <c>video</c> 结构体，回调方向）。
/// </summary>
/// <remarks>
/// <para>
/// 回调地址模式（100719）：仅 <c>url</c>；加密 AESKey <b>与回调加解密 AESKey 相同</b>（即 <c>EncodingAESKey</c>）。
/// 长连接模式（101463）：额外返回 <c>aeskey</c>（<b>每个下载链接唯一</b>，不同于统一的 EncodingAESKey）。
/// </para>
/// <para>
/// 两种模式的加密算法一致：AES-256-CBC + PKCS#7 填充至 32 字节的倍数，IV 取 key 前 16 字节；
/// 下载 url <b>5 分钟内有效</b>，且下载得到的内容<b>已加密、不能直接打开</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotMediaContent
{
    /// <summary>资源下载 url（5 分钟内有效；下载内容已加密）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>解密密钥（长连接模式返回，每个下载链接唯一；回调地址模式不返回，此时用 EncodingAESKey）。</summary>
    [JsonPropertyName("aeskey")]
    public string? AesKey { get; set; }
}

/// <summary>智能机器人图文混排内容（<c>mixed</c> 结构体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotMixedContent
{
    /// <summary>图文混排消息列表（元素 <c>msgtype</c> 为 <c>text</c> / <c>image</c>）。</summary>
    [JsonPropertyName("msg_item")]
    public List<AibotMixedItem>? MsgItem { get; set; }
}

/// <summary>智能机器人图文混排项（<c>mixed.msg_item</c> 项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotMixedItem
{
    /// <summary>图文混排中的类型：<c>text</c> / <c>image</c>。</summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>图文混排中的文本内容（<c>msgtype=text</c>）。</summary>
    [JsonPropertyName("text")]
    public AibotTextBody? Text { get; set; }

    /// <summary>图文混排中的图片内容（<c>msgtype=image</c>）。</summary>
    [JsonPropertyName("image")]
    public AibotMediaContent? Image { get; set; }
}

/// <summary>智能机器人语音内容（<c>voice</c> 结构体，回调方向）。</summary>
/// <remarks>官方只提供语音转换后的文本，不提供音频下载 url。</remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotVoiceContent
{
    /// <summary>语音转换成文本的内容。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>智能机器人流式消息刷新标识（<c>stream</c> 结构体，回调方向）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotStreamRefresh
{
    /// <summary>流式消息 id（机器人据该 id 返回对应的流式消息）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

/// <summary>
/// 智能机器人引用内容（<c>quote</c> 结构体）：按被引用消息的 <c>msgtype</c> 填充对应分支。
/// </summary>
/// <remarks>官方 100719：被引用类型为 <c>text</c> / <c>image</c> / <c>mixed</c> / <c>voice</c> / <c>file</c> / <c>video</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotQuote
{
    /// <summary>引用的消息类型（<c>text</c> / <c>image</c> / <c>mixed</c> / <c>voice</c> / <c>file</c> / <c>video</c>）。</summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>引用的文本内容。</summary>
    [JsonPropertyName("text")]
    public AibotTextBody? Text { get; set; }

    /// <summary>引用的图片内容。</summary>
    [JsonPropertyName("image")]
    public AibotMediaContent? Image { get; set; }

    /// <summary>引用的图文混排内容。</summary>
    [JsonPropertyName("mixed")]
    public AibotMixedContent? Mixed { get; set; }

    /// <summary>引用的语音内容。</summary>
    [JsonPropertyName("voice")]
    public AibotVoiceContent? Voice { get; set; }

    /// <summary>引用的文件内容。</summary>
    [JsonPropertyName("file")]
    public AibotMediaContent? File { get; set; }

    /// <summary>引用的视频内容。</summary>
    [JsonPropertyName("video")]
    public AibotMediaContent? Video { get; set; }
}
