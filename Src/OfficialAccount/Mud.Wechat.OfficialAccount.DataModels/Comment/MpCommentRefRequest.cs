// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Comment;

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
