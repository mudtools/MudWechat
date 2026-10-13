// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评论 / 回复的内容外壳（官方 <c>replyContent</c> 与 <c>commentReplyContent</c> 参数表一致，均只有一个
/// <c>content</c> 字段 ⇒ 共用本类型）。
/// </summary>
/// <remarks><b>注意与请求侧的区别</b>：创建评论 / 回复时内容直接放在请求体顶层 <c>content</c>，
/// 只有<b>响应侧</b>才包一层本类型。</remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReplyContent
{
    /// <summary>回复的内容（<c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
