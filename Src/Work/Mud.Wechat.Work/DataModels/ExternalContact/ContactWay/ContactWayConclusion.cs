// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 「联系我」临时会话结束语（<c>conclusions</c>）。
/// <para>
/// 官方约束：文本 / 图片 / 链接 / 小程序四者不能全空；文本可与其他类型的内容同时使用（下发两条消息）；
/// 图片 / 链接 / 小程序三者只能选择其一，同时填写时按图片 &gt; 链接 &gt; 小程序的优先级生效。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ContactWayConclusion
{
    /// <summary>
    /// 获取或设置文本消息类型的结束语。
    /// </summary>
    [JsonPropertyName("text")]
    public ContactWayConclusionText? Text { get; set; }

    /// <summary>
    /// 获取或设置图片消息类型的结束语（构造时只填 <see cref="ContactWayConclusionImage.MediaId"/>，读取时返回 <c>PicUrl</c>）。
    /// </summary>
    [JsonPropertyName("image")]
    public ContactWayConclusionImage? Image { get; set; }

    /// <summary>
    /// 获取或设置链接消息类型的结束语。
    /// </summary>
    [JsonPropertyName("link")]
    public ContactWayConclusionLink? Link { get; set; }

    /// <summary>
    /// 获取或设置小程序消息类型的结束语。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public ContactWayConclusionMiniProgram? MiniProgram { get; set; }
}

/// <summary>
/// 结束语文本消息（<c>conclusions.text</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ContactWayConclusionText
{
    /// <summary>
    /// 获取或设置消息文本内容（最长 4000 个字节）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 结束语图片消息（<c>conclusions.image</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ContactWayConclusionImage
{
    /// <summary>
    /// 获取或设置图片的素材 id（构造结束语时填写；经「上传临时素材」接口获得）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置图片的链接（仅读取「联系我」配置时由官方返回）。
    /// </summary>
    [JsonPropertyName("pic_url")]
    public string? PicUrl { get; set; }
}

/// <summary>
/// 结束语链接消息（<c>conclusions.link</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ContactWayConclusionLink
{
    /// <summary>
    /// 获取或设置消息标题（最多 128 个字节）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置消息封面的 url。
    /// </summary>
    [JsonPropertyName("picurl")]
    public string? PicUrl { get; set; }

    /// <summary>
    /// 获取或设置消息描述（最多 512 个字节）。
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置消息链接的 url。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 结束语小程序消息（<c>conclusions.miniprogram</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ContactWayConclusionMiniProgram
{
    /// <summary>
    /// 获取或设置小程序消息标题（最多 64 个字节）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置小程序消息封面的素材 id（图片建议尺寸为 520 × 416）。
    /// </summary>
    [JsonPropertyName("pic_media_id")]
    public string? PicMediaId { get; set; }

    /// <summary>
    /// 获取或设置小程序 appid（必须关联到企业）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置小程序页面的 page 路径。
    /// </summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }
}
