// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.AutoReply;

/// <summary>匹配关键词条目（官方 <c>keyword_list_info</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpAutoReplyKeyword
{
    /// <summary>获取或设置类型（官方 <c>type</c>：text/img/voice/video）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置关键词内容或 mediaID（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置匹配模式（官方 <c>match_mode</c>：contain 含有关键词即命中 / equal 严格相同）。</summary>
    [JsonPropertyName("match_mode")]
    public string? MatchMode { get; set; }
}
