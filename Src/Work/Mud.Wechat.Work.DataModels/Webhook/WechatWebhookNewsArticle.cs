// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人图文消息条目（<c>msgtype=news</c> 的 <c>news.articles[]</c> 项）。
/// <para>图文条数 1~8 条；picurl 必须是公网可访问的完整图片链接（http/https）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookNewsArticle
{
    /// <summary>
    /// 获取或设置图文标题（官方必填）。
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置图文描述（可选）。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置点击后跳转的链接（可选）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置图文消息的图片链接（可选；必须是<b>公网可访问</b>的完整 URL，http/https，
    /// 支持 JPG/PNG 格式，否则官方展示为空白图；较好的效果为大图 1068×455、小图 150×150）。
    /// </summary>
    [JsonPropertyName("picurl")]
    public string? Picurl { get; set; }
}
