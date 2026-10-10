// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Batch;

/// <summary>
/// 异步任务结果明细（<see cref="GetBatchJobResultResponse.Result"/> 的元素；按任务类型承载两种官方形状）。
/// </summary>
/// <remarks>
/// 任务类型为 <c>sync_user</c> / <c>replace_user</c> 时填充 <see cref="UserId"/>；
/// 任务类型为 <c>invite_user</c> 时填充 <see cref="UserId"/> 与 <see cref="InviteType"/>；
/// 任务类型为 <c>replace_party</c> 时填充 <see cref="Action"/> 与 <see cref="PartyId"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Batch")]
public class BatchTaskResultItem
{
    /// <summary>
    /// 获取或设置成员 UserID（对应管理端的账号；成员任务有效）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置邀请类型（0 没有邀请方式或未邀请、1 微信邀请、2 邮件邀请；<c>invite_user</c> 任务有效）。
    /// </summary>
    [JsonPropertyName("invitetype")]
    public int? InviteType { get; set; }

    /// <summary>
    /// 获取或设置部门操作类型（按位或：1 新建部门、2 更改部门名称、4 移动部门、8 修改部门排序；部门任务有效）。
    /// </summary>
    [JsonPropertyName("action")]
    public int? Action { get; set; }

    /// <summary>
    /// 获取或设置部门 ID（部门任务有效）。
    /// </summary>
    [JsonPropertyName("partyid")]
    public int? PartyId { get; set; }

    /// <summary>
    /// 获取或设置该成员 / 部门对应操作的结果错误码。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }

    /// <summary>
    /// 获取或设置错误信息（例如无权限错误，键值冲突，格式错误等）。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}
