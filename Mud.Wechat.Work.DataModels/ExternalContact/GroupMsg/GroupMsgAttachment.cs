// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 群发 / 欢迎语消息的图片附件（<c>attachments[].image</c> 或素材根字段 <c>image</c>）。
/// <para><see cref="MediaId"/> 与 <see cref="PicUrl"/> 只填一个即可，
/// 同时填写时优先使用 media_id；pic_url 仅可使用「上传图片」接口得到的链接。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgImageAttachment
{
    /// <summary>
    /// 获取或设置图片的素材 id（经素材管理接口获得）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置图片的链接（仅可使用「上传图片」接口得到的链接）。
    /// </summary>
    [JsonPropertyName("pic_url")]
    public string? PicUrl { get; set; }
}

/// <summary>
/// 群发 / 欢迎语消息的图文（链接）附件（<c>attachments[].link</c> 或素材根字段 <c>link</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgLinkAttachment
{
    /// <summary>
    /// 获取或设置图文消息标题（最长 128 字节，官方必填）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置图文消息封面的 url（最长 2048 字节）。
    /// </summary>
    [JsonPropertyName("picurl")]
    public string? PicUrl { get; set; }

    /// <summary>
    /// 获取或设置图文消息的描述（最长 512 字节）。
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置图文消息的链接（最长 2048 字节，官方必填）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 群发 / 欢迎语消息的小程序附件（<c>attachments[].miniprogram</c> 或素材根字段 <c>miniprogram</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgMiniProgramAttachment
{
    /// <summary>
    /// 获取或设置小程序消息标题（最长 64 字节，官方必填）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置小程序消息封面的素材 id（官方必填；封面建议尺寸 520 × 416）。
    /// </summary>
    [JsonPropertyName("pic_media_id")]
    public string? PicMediaId { get; set; }

    /// <summary>
    /// 获取或设置小程序 appid（官方必填；必须是关联到企业的小程序应用）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置小程序消息页面的 page 路径（官方必填）。
    /// </summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }
}

/// <summary>
/// 群发 / 欢迎语消息的视频附件（<c>attachments[].video</c> 或素材根字段 <c>video</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgVideoAttachment
{
    /// <summary>
    /// 获取或设置视频的素材 id（官方必填；经素材管理 / 异步上传临时素材接口获得）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 群发 / 欢迎语消息的文件附件（<c>attachments[].file</c> 或素材根字段 <c>file</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgFileAttachment
{
    /// <summary>
    /// 获取或设置文件的素材 id（官方必填；经素材管理 / 异步上传临时素材接口获得）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 企业群发 / 新客户欢迎语的附件（<c>attachments[]</c> 元素）。
/// <para>最多 9 个附件；各字段须与 <see cref="MsgType"/> 一致，否则报错；
/// 图片的 media_id 与 pic_url 同时填写时优先 media_id。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgAttachment
{
    /// <summary>
    /// 获取或设置附件类型（官方必填）：image - 图片，link - 图文，miniprogram - 小程序，video - 视频，file - 文件。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>
    /// 获取或设置图片附件。
    /// </summary>
    [JsonPropertyName("image")]
    public GroupMsgImageAttachment? Image { get; set; }

    /// <summary>
    /// 获取或设置图文（链接）附件。
    /// </summary>
    [JsonPropertyName("link")]
    public GroupMsgLinkAttachment? Link { get; set; }

    /// <summary>
    /// 获取或设置小程序附件。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public GroupMsgMiniProgramAttachment? MiniProgram { get; set; }

    /// <summary>
    /// 获取或设置视频附件。
    /// </summary>
    [JsonPropertyName("video")]
    public GroupMsgVideoAttachment? Video { get; set; }

    /// <summary>
    /// 获取或设置文件附件。
    /// </summary>
    [JsonPropertyName("file")]
    public GroupMsgFileAttachment? File { get; set; }
}
