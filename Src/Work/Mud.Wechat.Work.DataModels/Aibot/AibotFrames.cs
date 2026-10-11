// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.Work.DataModels.Aibot;

/// <summary>
/// 智能机器人长连接帧外壳（官方 101463：<c>{cmd, headers: {req_id}, body}</c>，收发两向共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两阶段反序列化</b>（<c>cmd</c> 与 <c>body</c> 多态的官方形态）：第一阶段只解本外壳
/// （<see cref="Cmd"/> / <see cref="Headers"/> / <see cref="Body"/> 保留为 <see cref="JsonElement"/>），
/// 第二阶段按 <see cref="Cmd"/> 取对应 <c>JsonTypeInfo</c> 解 <see cref="Body"/> ——
/// 回调帧 body 即回调模式报文（<c>AibotMessageCallback</c> / <c>AibotEventCallback</c>，明文），
/// 上传帧 body 为 <c>aibot_upload_media_*</c> 的专用结构。
/// </para>
/// <para>
/// <b>发送侧</b>：强类型 body 先 <c>JsonSerializer.SerializeToElement(body, typeInfo)</c> 转成
/// <see cref="JsonElement"/> 再挂到 <see cref="Body"/>（源生成上下文对 <see cref="JsonElement"/> 有内建元数据，
/// 零反射）。心跳 <c>ping</c> 帧无 body（官方示例）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotFrame
{
    /// <summary>帧命令（官方 <c>cmd</c>，取值见 <c>WechatBotFrameCommands</c>）。</summary>
    [JsonPropertyName("cmd")]
    public string? Cmd { get; set; }

    /// <summary>帧头（官方 <c>headers</c>，承载 <c>req_id</c>）。</summary>
    [JsonPropertyName("headers")]
    public AibotFrameHeaders? Headers { get; set; }

    /// <summary>帧体（官方 <c>body</c>；两阶段反序列化的第二阶段按 <see cref="Cmd"/> 解析）。</summary>
    [JsonPropertyName("body")]
    public JsonElement? Body { get; set; }
}

/// <summary>长连接帧头（官方 <c>headers</c>，只承载 <c>req_id</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotFrameHeaders
{
    /// <summary>请求唯一标识（官方 <c>req_id</c>；应答帧原样回传，供请求侧关联）。</summary>
    [JsonPropertyName("req_id")]
    public string? ReqId { get; set; }
}

/// <summary>
/// 长连接<b>应答帧</b>（官方统一形态 <c>{headers: {req_id}, errcode, errmsg}</c>；
/// 素材上传 <c>init</c> / <c>finish</c> 的应答另带 <c>body</c> 载荷）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotFrameAck
{
    /// <summary>帧头（官方 <c>headers</c>；<c>req_id</c> 与请求帧一致）。</summary>
    [JsonPropertyName("headers")]
    public AibotFrameHeaders? Headers { get; set; }

    /// <summary>错误码（官方 <c>errcode</c>，<c>0</c> = 成功）。</summary>
    [JsonPropertyName("errcode")]
    public int? ErrCode { get; set; }

    /// <summary>错误描述（官方 <c>errmsg</c>，成功时为 <c>ok</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }

    /// <summary>应答载荷（官方 <c>body</c>；仅上传 <c>init</c> / <c>finish</c> 的应答携带，按 <c>cmd</c> 第二阶段解析）。</summary>
    [JsonPropertyName("body")]
    public JsonElement? Body { get; set; }

    /// <summary>是否成功（<c>errcode == 0</c>；官方对每支应答帧统一以 errcode 表达失败）。</summary>
    [JsonIgnore]
    public bool IsSuccess => ErrCode == 0;
}

/// <summary>素材上传初始化帧体（官方 <c>aibot_upload_media_init</c> 的 body）。</summary>
/// <remarks>
/// 官方约束（101463）：会话 30 分钟有效；<paramref name="totalSize"/> 最少 5 字节、
/// 尺寸上限图片 ≤10MB / 语音 ≤2MB / 视频 ≤10MB / 普通文件 ≤20MB；
/// 分片数 ≤100、单分片 ≤512KB（base64 前）。上限值官方未给逐项枚举页 ⇒ SDK 不做本地硬拦，越界由应答 <c>errcode</c> 表达。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotUploadMediaInitBody
{
    /// <summary>素材类型（官方 <c>type</c>，<c>file</c> / <c>image</c> / <c>voice</c> / <c>video</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>文件名（官方 <c>filename</c>）。</summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>总字节数（官方 <c>total_size</c>，<c>integer</c>，最少 5）。</summary>
    [JsonPropertyName("total_size")]
    public long? TotalSize { get; set; }

    /// <summary>分片总数（官方 <c>total_chunks</c>，<c>integer</c>，≤100）。</summary>
    [JsonPropertyName("total_chunks")]
    public int? TotalChunks { get; set; }

    /// <summary>文件 MD5（官方 <c>md5</c>，选填）。</summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }
}

/// <summary>素材上传初始化应答载荷（官方应答 body：<c>{upload_id}</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotUploadMediaInitAck
{
    /// <summary>上传会话 id（官方 <c>upload_id</c>；后续分片与结束帧凭它关联，会话 30 分钟有效）。</summary>
    [JsonPropertyName("upload_id")]
    public string? UploadId { get; set; }
}

/// <summary>素材上传分片帧体（官方 <c>aibot_upload_media_chunk</c> 的 body）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotUploadMediaChunkBody
{
    /// <summary>上传会话 id（官方 <c>upload_id</c>，来自初始化应答）。</summary>
    [JsonPropertyName("upload_id")]
    public string? UploadId { get; set; }

    /// <summary>
    /// 分片序号（官方 <c>chunk_index</c>，<b>从 0 开始</b>；官方示例为数字而字段表标注 string
    /// —— 官方两处自相矛盾，SDK 按 <see cref="long"/> 承载并照录该矛盾）。
    /// </summary>
    [JsonPropertyName("chunk_index")]
    public long? ChunkIndex { get; set; }

    /// <summary>分片内容（官方 <c>base64_data</c>，单分片 ≤512KB（base64 前）；分片可乱序、重复上传幂等忽略）。</summary>
    [JsonPropertyName("base64_data")]
    public string? Base64Data { get; set; }
}

/// <summary>素材上传结束帧体（官方 <c>aibot_upload_media_finish</c> 的 body）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotUploadMediaFinishBody
{
    /// <summary>上传会话 id（官方 <c>upload_id</c>）。</summary>
    [JsonPropertyName("upload_id")]
    public string? UploadId { get; set; }
}

/// <summary>素材上传结束应答载荷（官方应答 body：<c>{type, media_id, created_at}</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotUploadMediaFinishAck
{
    /// <summary>素材类型（官方 <c>type</c>，回显请求的 type）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>素材 id（官方 <c>media_id</c>；<b>3 天内有效</b>，供 <c>aibot_send_msg</c> / 应答帧的媒体分支引用）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>创建时间（官方 <c>created_at</c>）。</summary>
    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
}

/// <summary>订阅帧体（官方 <c>aibot_subscribe</c> 的 body：<c>{bot_id, secret}</c>）。</summary>
/// <remarks>
/// <see cref="Secret"/> 是长连接<b>专用</b>密钥（官方原文与回调地址模式的 Token/EncodingAESKey 不同）
/// —— <b>属凭据</b>：本类型实例只用于订阅帧序列化，<b>不得</b>进日志 / 遥测 / 异常消息。
/// 官方有频率保护：订阅成功后禁止反复订阅（每条连接恰一次）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotSubscribeBody
{
    /// <summary>机器人标识（官方 <c>bot_id</c>）。</summary>
    [JsonPropertyName("bot_id")]
    public string? BotId { get; set; }

    /// <summary>长连接专用密钥（官方 <c>secret</c>；属凭据，不得外泄）。</summary>
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }
}

/// <summary>
/// 主动推送帧体（官方 <c>aibot_send_msg</c> 的 body：<c>{chatid, chat_type, msgtype, …}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么继承 <see cref="AibotMessage"/> 而不是复用</b>：主动推送在应答超集之上多 <c>chatid</c> /
/// <c>chat_type</c> 两个寻址字段（官方 101463 帧体原文）；ADR-11 的「一份应答超集」纪律不允许为它平行复制
/// 一套 msgtype 分支 ⇒ 继承扩展是唯一不漂移的形态。欢迎语（<c>aibot_respond_welcome_msg</c>）与
/// 模板卡片更新（<c>aibot_respond_update_msg</c>）的帧体<b>无新增字段</b>，直接用 <see cref="AibotMessage"/>。
/// </para>
/// <para>
/// <b>寻址语义</b>：<see cref="ChatType"/> 取 <c>1</c> 单聊（<see cref="ChatId"/> 填 userid）/
/// <c>2</c> 群聊 / <c>0</c> 或不填为兼容模式（优先按群聊解析）；官方要求用户在会话中先发过消息才可推送。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotSendMessageBody : AibotMessage
{
    /// <summary>会话 id（官方 <c>chatid</c>；单聊时填 userid）。</summary>
    [JsonPropertyName("chatid")]
    public string? ChatId { get; set; }

    /// <summary>会话类型（官方 <c>chat_type</c>，<c>integer</c>：1 单聊 / 2 群聊 / 0 或不填 = 兼容模式）。</summary>
    /// <remarks>
    /// 与回调帧的 <c>chattype</c>（字符串 <c>single</c>/<c>group</c>）<b>不同键不同型</b> ——
    /// 官方两处自相矛盾的形态照录，不得互相「统一」。
    /// </remarks>
    [JsonPropertyName("chat_type")]
    public int? ChatType { get; set; }
}
