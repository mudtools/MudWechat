// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Comment;

/// <summary>评论条目（官方 <c>comment</c> 数组元素）。</summary>
/// <remarks>comment_type 的官方说明原文「0为即非精选，1为true，即精选」疑漏字（照录）：0 非精选 / 1 精选。</remarks>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpComment
{
    /// <summary>获取或设置用户评论 id（官方 <c>user_comment_id</c>；精选/删除/回复操作均以此定位）。</summary>
    [JsonPropertyName("user_comment_id")]
    public long UserCommentId { get; set; }

    /// <summary>获取或设置评论时间（官方 <c>create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置评论内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置是否精选评论（官方 <c>comment_type</c>：0 非精选 / 1 精选；官方说明原文疑漏字，照录）。</summary>
    [JsonPropertyName("comment_type")]
    public int? CommentType { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>；「用户如果用非微信身份评论，不返回 openid」——官方原文）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置回复信息（官方 <c>reply</c>；无回复时缺省）。</summary>
    [JsonPropertyName("reply")]
    public MpCommentReplyInfo? Reply { get; set; }
}
