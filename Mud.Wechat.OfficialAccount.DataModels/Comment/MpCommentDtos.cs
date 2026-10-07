// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Comment;

/// <summary>
/// 打开 / 关闭文章评论的请求体（<c>comment/open</c> 与 <c>comment/close</c> 请求体字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决（N4：字段集一致才共用，由守卫双向锁定）</b>：open 与 close 两页请求体均为
/// <c>{msg_data_id, index}</c>（index 否——多图文时指定第几篇，从 0 开始，不带默认第一篇）。
/// </para>
/// <para><b>前置条件</b>：公众号需具备<b>留言功能权限</b>（官方原文；无权限返回 88000）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentOpenRequest
{
    /// <summary>获取或设置群发返回的 msg_data_id（官方 <c>msg_data_id</c>，必填）。</summary>
    [JsonPropertyName("msg_data_id")]
    public long MsgDataId { get; set; }

    /// <summary>获取或设置多图文序号（官方 <c>index</c>，从 0 开始；不带默认操作第一篇）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }
}

/// <summary>查看文章评论数据（<c>comment/list</c>）请求体。</summary>
/// <remarks>
/// <para>
/// <b>逐页核验修正</b>：参数形制为 <c>msg_data_id</c>/<c>index</c>/<c>begin</c>/<c>count</c>/<c>type</c>
/// （<b>非</b>方案文档预判的 article_id/begin/limit）；<c>count</c>「获取数目（&gt;=50 会被拒绝）」⇒ 上限 50
/// （官方错误码 88010 描述原文拼写「cout」——照录）。
/// </para>
/// <para><c>type</c>：0 普通评论 &amp; 精选评论 / 1 普通评论 / 2 精选评论（官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentListRequest
{
    /// <summary>获取或设置群发返回的 msg_data_id（官方 <c>msg_data_id</c>，必填）。</summary>
    [JsonPropertyName("msg_data_id")]
    public long MsgDataId { get; set; }

    /// <summary>获取或设置多图文序号（官方 <c>index</c>，从 0 开始；不带默认返回第一篇）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>获取或设置起始位置（官方 <c>begin</c>，必填）。</summary>
    [JsonPropertyName("begin")]
    public int Begin { get; set; }

    /// <summary>获取或设置获取数目（官方 <c>count</c>，必填；&gt;=50 会被拒绝）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置评论类型（官方 <c>type</c>，必填：0 全部 / 1 普通 / 2 精选）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}

/// <summary>查看文章评论数据（<c>comment/list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentListResponse : MpResponse
{
    /// <summary>获取或设置评论总数（官方 <c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>获取或设置评论列表（官方 <c>comment</c>）。</summary>
    [JsonPropertyName("comment")]
    public List<MpComment>? Comments { get; set; }
}

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

/// <summary>评论的回复信息（官方 <c>reply</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentReplyInfo
{
    /// <summary>获取或设置回复内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置回复时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}

/// <summary>
/// 按 user_comment_id 寻址评论的请求体（markelect / unmarkelect / delete / reply/delete 四端点共用）。
/// </summary>
/// <remarks>
/// <b>共用裁决（N4：字段集一致才共用，由守卫双向锁定）</b>：四页请求体均为
/// <c>{msg_data_id, index, user_comment_id}</c>（逐页核验——方案文档预判的「嵌套 user_comment 对象」不存在，
/// 官方为扁平三字段形态）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentRefRequest
{
    /// <summary>获取或设置群发返回的 msg_data_id（官方 <c>msg_data_id</c>，必填）。</summary>
    [JsonPropertyName("msg_data_id")]
    public long MsgDataId { get; set; }

    /// <summary>获取或设置多图文序号（官方 <c>index</c>，从 0 开始；不带默认操作第一篇）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>获取或设置用户评论 id（官方 <c>user_comment_id</c>，必填）。</summary>
    [JsonPropertyName("user_comment_id")]
    public long UserCommentId { get; set; }
}

/// <summary>回复评论（<c>comment/reply/add</c>）请求体（官方扁平四字段——无嵌套 user_comment 形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentReplyAddRequest
{
    /// <summary>获取或设置群发返回的 msg_data_id（官方 <c>msg_data_id</c>，必填）。</summary>
    [JsonPropertyName("msg_data_id")]
    public long MsgDataId { get; set; }

    /// <summary>获取或设置多图文序号（官方 <c>index</c>，从 0 开始；不带默认操作第一篇）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>获取或设置用户评论 id（官方 <c>user_comment_id</c>，必填）。</summary>
    [JsonPropertyName("user_comment_id")]
    public long UserCommentId { get; set; }

    /// <summary>获取或设置回复内容（官方 <c>content</c>，必填；超过长度限制或为空返回 88007）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
