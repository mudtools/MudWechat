// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Events;

/// <summary>
/// 新增部门事件（<c>change_contact</c> + <c>create_party</c>；官方 90971）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90971">path 90971 部门变更通知</see>。
/// </remarks>
public class PartyCreatedEvent
{
    /// <summary>部门 id。</summary>
    public string? Id { get; set; }

    /// <summary>部门名称（需通讯录部门权限；2022-08-15 后新 URL 部门事件仅回调 id 子集时不返回）。</summary>
    public string? Name { get; set; }

    /// <summary>父部门 id（根部门为 1）。</summary>
    public string? ParentId { get; set; }

    /// <summary>在父部门中的次序值（Order 越大越靠前；需通讯录部门权限）。</summary>
    public string? Order { get; set; }
}

/// <summary>
/// 更新部门事件（<c>change_contact</c> + <c>update_party</c>；官方 90971）。
/// </summary>
/// <remarks>
/// 仅部门 ParentId 变更触发（Name/Order 变更不推送）。
/// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90971">path 90971 部门变更通知</see>。
/// </remarks>
public class PartyUpdatedEvent
{
    /// <summary>部门 id。</summary>
    public string? Id { get; set; }

    /// <summary>部门名称（需通讯录部门权限）。</summary>
    public string? Name { get; set; }

    /// <summary>父部门 id（根部门为 1）。</summary>
    public string? ParentId { get; set; }
}

/// <summary>
/// 删除部门事件（<c>change_contact</c> + <c>delete_party</c>；官方 90971）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90971">path 90971 部门变更通知</see>。
/// </remarks>
public class PartyDeletedEvent
{
    /// <summary>被删除部门的 id。</summary>
    public string? Id { get; set; }
}
