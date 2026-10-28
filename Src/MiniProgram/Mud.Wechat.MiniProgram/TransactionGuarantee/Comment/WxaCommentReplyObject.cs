// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评论 / 回复的主体信息（官方 <c>replyObject</c> 与 <c>commentReplyObject</c> 参数表一致
/// ⇒ 共用本类型）。
/// </summary>
/// <remarks>
/// <b>字段名口径</b>：本对象官方用<b>小写</b> <c>nickname</c> 与 <c>imgUrl</c>，
/// 而评价用户信息 <see cref="WxaCommentUserInfo"/> 用 <c>nickName</c> / <c>headImg</c>
/// ——同域两套命名并存，照官方原文逐字段建模，不得互相「对齐」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentReplyObject
{
    /// <summary>用户的昵称（<c>nickname</c>，官方全小写拼写）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>用户的头像（<c>imgUrl</c>）。</summary>
    [JsonPropertyName("imgUrl")]
    public string? ImgUrl { get; set; }
}
