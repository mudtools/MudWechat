// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.AutoReply;

/// <summary>关键词自动回复规则（官方 <c>keyword_autoreply_info.list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpKeywordAutoReplyRule
{
    /// <summary>获取或设置规则名称（官方 <c>rule_name</c>）。</summary>
    [JsonPropertyName("rule_name")]
    public string? RuleName { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置回复模式（官方 <c>reply_mode</c>：reply_all 全部回复 / random_one 随机回复其中一条）。</summary>
    [JsonPropertyName("reply_mode")]
    public string? ReplyMode { get; set; }

    /// <summary>获取或设置匹配的关键词列表（官方 <c>keyword_list_info</c>）。</summary>
    [JsonPropertyName("keyword_list_info")]
    public List<MpAutoReplyKeyword>? KeywordListInfo { get; set; }

    /// <summary>获取或设置回复列表（官方 <c>reply_list_info</c>）。</summary>
    [JsonPropertyName("reply_list_info")]
    public List<MpAutoReplyReply>? ReplyListInfo { get; set; }
}
