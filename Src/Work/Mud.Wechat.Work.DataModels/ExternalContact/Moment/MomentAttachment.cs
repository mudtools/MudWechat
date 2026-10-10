// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 发表朋友圈的图片附件（<c>attachments[].image</c>）。
/// <para>图片素材经「上传附件资源」接口获得，长边不超过 10800 像素、短边不超过 1080 像素。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentImageAttachment
{
    /// <summary>
    /// 获取或设置图片素材 id（官方必填）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 发表朋友圈的图文（链接）附件（<c>attachments[].link</c>，只支持 1 个）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentLinkAttachment
{
    /// <summary>
    /// 获取或设置图文的标题（最多 64 字 / 128 字节）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置图文链接的 url（官方必填）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置图文的封面图素材 id（官方必填；尺寸约束同图片附件）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 发表朋友圈的视频附件（<c>attachments[].video</c>，只支持 1 个）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentVideoAttachment
{
    /// <summary>
    /// 获取或设置视频素材 id（官方必填；视频时长不超过 30 秒，大小不超过 10MB，未填写报 invalid msg）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 发表朋友圈的附件（<c>attachments[]</c> 元素）。
/// <para>图片 / 视频 / 链接三种附件类型三选一，不能混用；最多 9 个图片、或 1 个视频、或 1 个链接。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentAttachment
{
    /// <summary>
    /// 获取或设置附件类型（官方必填）：image - 图片，link - 图文，video - 视频。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>
    /// 获取或设置图片附件（最多 9 个，超过报错 invalid attachments size）。
    /// </summary>
    [JsonPropertyName("image")]
    public MomentImageAttachment? Image { get; set; }

    /// <summary>
    /// 获取或设置图文（链接）附件（只支持 1 个）。
    /// </summary>
    [JsonPropertyName("link")]
    public MomentLinkAttachment? Link { get; set; }

    /// <summary>
    /// 获取或设置视频附件（只支持 1 个）。
    /// </summary>
    [JsonPropertyName("video")]
    public MomentVideoAttachment? Video { get; set; }
}
