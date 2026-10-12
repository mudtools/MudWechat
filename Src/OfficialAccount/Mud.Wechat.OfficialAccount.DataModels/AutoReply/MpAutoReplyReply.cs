// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.AutoReply;

/// <summary>自动回复条目（官方 <c>reply_list_info</c> 元素；关键词自动回复比简单回复多支持图文 news）。</summary>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpAutoReplyReply
{
    /// <summary>获取或设置回复类型（官方 <c>type</c>：text/img/voice/video/news）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置回复内容或 mediaID（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置图文消息的信息（官方 <c>news_info</c>；仅 news 类型携带）。</summary>
    [JsonPropertyName("news_info")]
    public MpAutoReplyNewsInfo? NewsInfo { get; set; }
}
