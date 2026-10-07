// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.AutoReply;

/// <summary>
/// 获取当前自动回复规则（<c>get_current_autoreply_info</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；仅能获取公众平台官网自动回复功能中设置的规则
/// （公众号自行开发或第三方实现的自动回复无法获取）。
/// </para>
/// <para>
/// <b>素材时效（官方注意事项原文）</b>：返回的图片/语音/视频为<b>临时素材</b>（每次获取不同，
/// 3 天内有效，需通过「获取临时素材」接口获取）；图文消息为<b>永久素材</b>（需通过「获取永久素材」接口获取）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpAutoReplyInfoResponse : MpResponse
{
    /// <summary>获取或设置关注后自动回复是否开启（官方 <c>is_add_friend_reply_open</c>：0 未开启 / 1 开启）。</summary>
    [JsonPropertyName("is_add_friend_reply_open")]
    public int IsAddFriendReplyOpen { get; set; }

    /// <summary>获取或设置消息自动回复是否开启（官方 <c>is_autoreply_open</c>：0 未开启 / 1 开启）。</summary>
    [JsonPropertyName("is_autoreply_open")]
    public int IsAutoReplyOpen { get; set; }

    /// <summary>获取或设置关注后自动回复的信息（官方 <c>add_friend_autoreply_info</c>）。</summary>
    [JsonPropertyName("add_friend_autoreply_info")]
    public MpSimpleAutoReplyInfo? AddFriendAutoReplyInfo { get; set; }

    /// <summary>获取或设置消息自动回复的信息（官方 <c>message_default_autoreply_info</c>）。</summary>
    [JsonPropertyName("message_default_autoreply_info")]
    public MpSimpleAutoReplyInfo? MessageDefaultAutoReplyInfo { get; set; }

    /// <summary>获取或设置关键词自动回复的信息（官方 <c>keyword_autoreply_info</c>）。</summary>
    [JsonPropertyName("keyword_autoreply_info")]
    public MpKeywordAutoReplyInfo? KeywordAutoReplyInfo { get; set; }
}

/// <summary>
/// 简单自动回复信息（关注后 / 消息默认两处共用；官方两处字段表同形 <c>type</c>+<c>content</c> ⇒ 共用 DTO）。
/// </summary>
/// <remarks>官方枚举：text / img / voice / video / news（关注后与消息自动回复仅支持文本、图片、语音、视频）。</remarks>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpSimpleAutoReplyInfo
{
    /// <summary>获取或设置自动回复类型（官方 <c>type</c>：text/img/voice/video/news）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置回复内容（官方 <c>content</c>：文本类型为文本内容；图文/图片/语音/视频类型为 mediaID）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>关键词自动回复信息（官方 <c>keyword_autoreply_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpKeywordAutoReplyInfo
{
    /// <summary>获取或设置自动回复规则列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpKeywordAutoReplyRule>? List { get; set; }
}

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

/// <summary>图文消息信息容器（官方 <c>news_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpAutoReplyNewsInfo
{
    /// <summary>获取或设置图文列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpAutoReplyNewsItem>? List { get; set; }
}

/// <summary>图文条目（官方 <c>news_info.list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpAutoReplyNewsItem
{
    /// <summary>获取或设置图文消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置摘要（官方 <c>digest</c>）。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>获取或设置作者（官方 <c>author</c>）。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>获取或设置是否显示封面（官方 <c>show_cover</c>：0 不显示 / 1 显示）。</summary>
    [JsonPropertyName("show_cover")]
    public int? ShowCover { get; set; }

    /// <summary>获取或设置封面图片的 URL（官方 <c>cover_url</c>）。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>获取或设置正文的 URL（官方 <c>content_url</c>）。</summary>
    [JsonPropertyName("content_url")]
    public string? ContentUrl { get; set; }

    /// <summary>获取或设置原文的 URL（官方 <c>source_url</c>；官方原文「若置空则无查看原文入口」）。</summary>
    [JsonPropertyName("source_url")]
    public string? SourceUrl { get; set; }
}
