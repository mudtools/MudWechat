// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 文本消息载荷（<c>MsgType = text</c>，官方接收普通消息页）。
/// </summary>
/// <remarks>
/// 公共字段（<c>ToUserName</c>/<c>FromUserName</c>/<c>CreateTime</c>/<c>MsgType</c>/<c>MsgId</c>）
/// 由信封承载（<see cref="MpCallbackEnvelope"/>），载荷只建模消息体字段。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCallbackMessageTypes.Text })]
public sealed partial class MpTextMessagePayload : MpCallbackPayload
{
    /// <summary>文本消息内容。</summary>
    [PayloadField("Content")]
    public string? Content { get; set; }
}

/// <summary>图片消息载荷（<c>MsgType = image</c>）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCallbackMessageTypes.Image })]
public sealed partial class MpImageMessagePayload : MpCallbackPayload
{
    /// <summary>图片链接（由微信生成）。</summary>
    [PayloadField("PicUrl")]
    public string? PicUrl { get; set; }

    /// <summary>图片消息媒体 id（可调用多媒体文件下载接口拉取数据）。</summary>
    [PayloadField("MediaId")]
    public string? MediaId { get; set; }
}

/// <summary>语音消息载荷（<c>MsgType = voice</c>；含 16K 语音媒体 id 字段）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCallbackMessageTypes.Voice })]
public sealed partial class MpVoiceMessagePayload : MpCallbackPayload
{
    /// <summary>语音消息媒体 id。</summary>
    [PayloadField("MediaId")]
    public string? MediaId { get; set; }

    /// <summary>语音格式（如 <c>amr</c>/<c>speex</c>）。</summary>
    [PayloadField("Format")]
    public string? Format { get; set; }

    /// <summary>16K 采样率语音媒体 id（官方为兼容旧版本单独给出）。</summary>
    [PayloadField("MediaId16K")]
    public string? MediaId16K { get; set; }
}

/// <summary>视频/小视频消息载荷（<c>MsgType = video</c> / <c>shortvideo</c>；两键字段集一致，合并登记）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpCallbackMessageTypes.Video,
    MpCallbackMessageTypes.ShortVideo,
})]
public sealed partial class MpVideoMessagePayload : MpCallbackPayload
{
    /// <summary>视频消息媒体 id。</summary>
    [PayloadField("MediaId")]
    public string? MediaId { get; set; }

    /// <summary>视频消息缩略图的媒体 id。</summary>
    [PayloadField("ThumbMediaId")]
    public string? ThumbMediaId { get; set; }
}

/// <summary>地理位置消息载荷（<c>MsgType = location</c>）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCallbackMessageTypes.Location })]
public sealed partial class MpLocationMessagePayload : MpCallbackPayload
{
    /// <summary>地理位置维度（节点名 <c>Location_X</c>）。</summary>
    [PayloadField("Location_X")]
    public string? Latitude { get; set; }

    /// <summary>地理位置经度（节点名 <c>Location_Y</c>）。</summary>
    [PayloadField("Location_Y")]
    public string? Longitude { get; set; }

    /// <summary>地图缩放大小。</summary>
    [PayloadField("Scale")]
    public string? Scale { get; set; }

    /// <summary>地理位置信息。</summary>
    [PayloadField("Label")]
    public string? Label { get; set; }
}

/// <summary>链接消息载荷（<c>MsgType = link</c>）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCallbackMessageTypes.Link })]
public sealed partial class MpLinkMessagePayload : MpCallbackPayload
{
    /// <summary>消息标题。</summary>
    [PayloadField("Title")]
    public string? Title { get; set; }

    /// <summary>消息描述。</summary>
    [PayloadField("Description")]
    public string? Description { get; set; }

    /// <summary>消息链接。</summary>
    [PayloadField("Url")]
    public string? Url { get; set; }
}
