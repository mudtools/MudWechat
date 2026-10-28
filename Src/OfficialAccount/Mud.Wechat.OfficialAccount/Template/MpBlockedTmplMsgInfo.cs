// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>被拦截的模板消息信息（官方 <c>msginfo</c> 内字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpBlockedTmplMsgInfo
{
    /// <summary>获取或设置记录唯一 ID（官方 <c>id</c>，用于翻页 largest_id；官方类型标注 string、示例为数字，矛盾照录）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置被拦截的模板消息 id（官方 <c>tmpl_msg_id</c>）。</summary>
    [JsonPropertyName("tmpl_msg_id")]
    public string? TmplMsgId { get; set; }

    /// <summary>获取或设置模板消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置模板消息的内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置下发的时间戳（官方 <c>send_timestamp</c>）。</summary>
    [JsonPropertyName("send_timestamp")]
    public long? SendTimestamp { get; set; }

    /// <summary>获取或设置下发目标用户的 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
