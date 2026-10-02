// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 图文消息文章项（<c>news.articles</c> 项），被发送应用消息（<c>/cgi-bin/message/send</c>）与发送「学校通知」（<c>/cgi-bin/externalcontact/message/send</c>）共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageNewsArticle
{
    /// <summary>
    /// 获取或设置标题，不超过 128 个字节，超过会自动截断（支持 id 转译）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置描述，不超过 512 个字节，超过会自动截断（支持 id 转译）。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置点击后跳转的链接，最长 2048 字节，须包含协议头（http/https）；小程序或者 url 必须填写一个（应用推送消息/学校通知场景必填）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置图文消息的图片链接，最长 2048 字节，支持 JPG、PNG 格式，较好效果为大图 1068*455、小图 150*150。
    /// </summary>
    [JsonPropertyName("picurl")]
    public string? PicUrl { get; set; }

    /// <summary>
    /// 获取或设置小程序 appid，必须是与当前应用关联的小程序；appid 和 pagepath 必须同时填写，填写后会忽略 url 字段（仅发送应用消息支持）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置点击消息卡片后的小程序页面，最长 128 字节，仅限本小程序内的页面（仅发送应用消息支持）。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}
