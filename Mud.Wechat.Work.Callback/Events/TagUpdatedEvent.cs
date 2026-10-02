// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Events;

/// <summary>
/// 标签成员变更事件（<c>change_contact</c> + <c>update_tag</c>；官方 90972）。
/// </summary>
/// <remarks>
/// 列表字段按官方报文<b>字符串承载</b>（逗号分隔 UserId/部门 id）。官方明示：标签的成员变更与成员/部门
/// 自身变更事件<b>时序不保证</b>，须以「获取标签成员」等拉取接口对齐（v1 方案 §10.4）。
/// </remarks>
public class TagUpdatedEvent
{
    /// <summary>标签 id。</summary>
    public string? TagId { get; set; }

    /// <summary>标签中新增的成员 UserId（逗号分隔）。</summary>
    public string? AddUserItems { get; set; }

    /// <summary>标签中移除的成员 UserId（逗号分隔）。</summary>
    public string? DelUserItems { get; set; }

    /// <summary>标签中新增的部门 id（逗号分隔）。</summary>
    public string? AddPartyItems { get; set; }

    /// <summary>标签中移除的部门 id（逗号分隔）。</summary>
    public string? DelPartyItems { get; set; }
}
