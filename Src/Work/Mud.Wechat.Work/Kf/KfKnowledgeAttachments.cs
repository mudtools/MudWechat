// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 知识库答案附件（按 <see cref="MsgType"/> 判别，填充对应的附件子对象；官方按 msgtype 区分 image / video / link / miniprogram 四类）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeAttachment
{
    /// <summary>
    /// 获取或设置附件类型（官方必填）：image - 图片，video - 视频，link - 链接，miniprogram - 小程序。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>
    /// 获取或设置图片附件体（<see cref="MsgType"/> 为 image 时填充）。
    /// </summary>
    [JsonPropertyName("image")]
    public KfKnowledgeImageAttachment? Image { get; set; }

    /// <summary>
    /// 获取或设置视频附件体（<see cref="MsgType"/> 为 video 时填充）。
    /// </summary>
    [JsonPropertyName("video")]
    public KfKnowledgeVideoAttachment? Video { get; set; }

    /// <summary>
    /// 获取或设置链接附件体（<see cref="MsgType"/> 为 link 时填充）。
    /// </summary>
    [JsonPropertyName("link")]
    public KfKnowledgeLinkAttachment? Link { get; set; }

    /// <summary>
    /// 获取或设置小程序附件体（<see cref="MsgType"/> 为 miniprogram 时填充）。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public KfKnowledgeMiniProgramAttachment? MiniProgram { get; set; }
}

/// <summary>
/// 知识库答案的图片附件（image）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeImageAttachment
{
    /// <summary>
    /// 获取或设置图片的临时素材 media_id（官方必填，调用上传临时素材接口获取；
    /// 获取问答列表时不返回该字段，仅返回 <see cref="Name"/>）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置图片名称（仅获取问答列表时返回）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// 知识库答案的视频附件（video）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeVideoAttachment
{
    /// <summary>
    /// 获取或设置视频的临时素材 media_id（官方必填，调用上传临时素材接口获取；
    /// 获取问答列表时不返回该字段，仅返回 <see cref="Name"/>）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置视频名称（仅获取问答列表时返回）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// 知识库答案的链接附件（link）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeLinkAttachment
{
    /// <summary>
    /// 获取或设置链接标题（官方必填）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置链接地址（官方必填）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置链接描述。
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置链接封面图片 URL。
    /// </summary>
    [JsonPropertyName("pic_url")]
    public string? PicUrl { get; set; }
}

/// <summary>
/// 知识库答案的小程序附件（miniprogram）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeMiniProgramAttachment
{
    /// <summary>
    /// 获取或设置小程序消息标题（最多 64 个字节，超过将截断）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置小程序消息封面的临时素材 media_id（官方必填，封面图片建议尺寸 520*416；
    /// 获取问答列表时不返回该字段）。
    /// </summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>
    /// 获取或设置小程序 appid（官方必填，必须是关联到企业的小程序 appid）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置小程序消息的页面路径（官方必填）。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}
