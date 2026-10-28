// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 会议「被操作者」对象（会议族 <c>OperatedUser</c> 节点；官方 98355/98393/98394/98397/98771）。
/// </summary>
/// <remarks>
/// <para>
/// 经上游 G-ADR-17 的 <c>Object</c> 嵌套通道声明化：单节点对象（非列表），元素缺失 ⇒ <c>null</c>。
/// 「操作者」经信封 <c>FromUserName</c>/<c>FromUserTmpOpenId</c> 承载，与本对象（被操作者）是两个主体。
/// </para>
/// <para>
/// <b>可空超集（ADR-14）</b>：<see cref="UserRole"/> 仅角色变更事件（<c>role_change</c>/<c>webinar_role_change</c>）
/// 携带，等候室类事件缺失 ⇒ <c>null</c>，处理器不得假设必有值。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackMeetingOperatedUser
{
    /// <summary>被操作者 UserID（官方 <c>OperatedUser/UserId</c>；仅当是企业成员时有值）。</summary>
    [PayloadField("UserId")]
    public string? UserId { get; set; }

    /// <summary>被操作者会中临时 ID（官方 <c>OperatedUser/TmpOpenId</c>）。</summary>
    [PayloadField("TmpOpenId")]
    public string? TmpOpenId { get; set; }

    /// <summary>
    /// 被操作者变更后的角色（官方 <c>OperatedUser/UserRole</c>，仅角色变更事件携带）：
    /// 0 普通成员 / 1 创建者 / 2 主持人 / 3 创建者+主持人 / 4 游客 / 5 游客+主持人 /
    /// 6 联席主持人 / 7 创建者+联席主持人 / 8 restApi 接口指派的主持人；
    /// 网络研讨会（<c>webinar_role_change</c>）另含 30 研讨会内部嘉宾 / 31 研讨会外部嘉宾 /
    /// 32 研讨会邀请链接入会嘉宾 / 33 研讨会观众 / 34 有音视频权限的研讨会观众。
    /// </summary>
    [PayloadField("UserRole")]
    public long? UserRole { get; set; }
}
