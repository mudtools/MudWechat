// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// mpnews 图文消息文章项（<c>mpnews.articles</c> 项），被发送应用消息（<c>/cgi-bin/message/send</c>）与发送「学校通知」（<c>/cgi-bin/externalcontact/message/send</c>）共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageMpNewsArticle
{
    /// <summary>
    /// 获取或设置标题，不超过 128 个字节，超过会自动截断（支持 id 转译）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置图文消息缩略图的 media_id，可通过素材管理接口获得。
    /// </summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>
    /// 获取或设置图文消息的作者，不超过 64 个字节。
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// 获取或设置图文消息点击"阅读原文"之后的页面链接。
    /// </summary>
    [JsonPropertyName("content_source_url")]
    public string? ContentSourceUrl { get; set; }

    /// <summary>
    /// 获取或设置图文消息的内容，支持 html 标签，不超过 666K 个字节（支持 id 转译）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 获取或设置图文消息的描述，不超过 512 个字节，超过会自动截断（支持 id 转译）。
    /// </summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }
}
