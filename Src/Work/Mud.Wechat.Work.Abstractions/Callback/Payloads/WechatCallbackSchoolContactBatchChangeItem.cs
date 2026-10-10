// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 家校通讯录批量变更事件（官方 path 97281）的单条变更项（官方 <c>ChangeList</c>，
/// 根下重复同名兄弟元素、无包装容器）。
/// </summary>
/// <remarks>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；元素定位与「单/多形态分派」由
/// <c>WechatPayloadConverter.RepeatSchoolContactChangeItems</c> 承担（依赖根层同名兄弟合并投影，
/// 同 <c>WechatCallbackMeetingMediumUploadItem</c> 的 <c>UploadInfo</c> 形态）。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackSchoolContactBatchChangeItem
{
    /// <summary>
    /// 变更类型（官方 <c>ChangeType</c>，闭合值域 11 类）：
    /// 成员 <c>create_student</c>/<c>update_student</c>/<c>delete_student</c>/
    /// <c>create_parent</c>/<c>update_parent</c>/<c>delete_parent</c>；
    /// 关注 <c>subscribe</c>/<c>unsubscribe</c>；
    /// 部门 <c>create_department</c>/<c>update_department</c>/<c>delete_department</c>。
    /// </summary>
    [PayloadField("ChangeType")]
    public string? ChangeType { get; set; }

    /// <summary>变更对象 id（官方 <c>Id</c>）：学生的家校通讯录 userid、家长的 id 或家校通讯录部门 id。</summary>
    [PayloadField("Id")]
    public string? Id { get; set; }

    /// <summary>
    /// 变更后的新 id（官方 <c>NewId</c>；<b>仅 <c>update_student</c>/<c>update_parent</c> 且 userid 被修改时出现</b>）。
    /// </summary>
    [PayloadField("NewId")]
    public string? NewId { get; set; }

    /// <summary>该条变更产生的时间戳（官方 <c>TimeStamp</c>，Unix 秒）。</summary>
    [PayloadField("TimeStamp")]
    public long? TimeStamp { get; set; }
}
