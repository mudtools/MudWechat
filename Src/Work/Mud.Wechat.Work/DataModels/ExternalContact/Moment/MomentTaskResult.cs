// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 创建发表朋友圈任务的详细处理结果（<c>get_moment_task_result</c> 响应中的 <c>result</c>，
/// 仅在任务创建完成后有效）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentTaskResult
{
    /// <summary>
    /// 获取或设置任务执行结果的错误码。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrorCode { get; set; }

    /// <summary>
    /// 获取或设置任务执行结果的错误信息。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 获取或设置朋友圈 id（可用于调用「获取客户朋友圈企业发表的列表」等接口）。
    /// </summary>
    [JsonPropertyName("moment_id")]
    public string? MomentId { get; set; }

    /// <summary>
    /// 获取或设置不合法的执行者列表（含不存在的 id 以及不在应用可见范围内的部门 / 成员）。
    /// </summary>
    [JsonPropertyName("invalid_sender_list")]
    public MomentSenderList? InvalidSenderList { get; set; }

    /// <summary>
    /// 获取或设置不合法的外部联系人列表（含不合法的客户标签列表）。
    /// </summary>
    [JsonPropertyName("invalid_external_contact_list")]
    public MomentExternalContactList? InvalidExternalContactList { get; set; }
}
