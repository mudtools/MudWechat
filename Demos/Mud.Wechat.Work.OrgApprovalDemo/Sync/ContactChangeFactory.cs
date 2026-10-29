// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text.Json;
using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 把回调载荷投影成 <see cref="ContactChange"/>（处理器与刷新作业之间的唯一契约）。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影刻意很窄</b>：只带镜像需要的标识与名称，<c>Mobile</c>/<c>Email</c>/<c>Avatar</c> 等字段
/// 一概不进 <c>org_change_log.payload_json</c>。理由有二 —— 一是台账会长期留存，
/// 二是这些字段本就受「权限分层」约束（2022-08-15 后新回调 URL 只回 id 子集），
/// 把「没给」写成「被清空」比不记录危险得多。
/// </para>
/// <para>
/// <b><c>OccurredAt</c> 取信封 <c>CreateTime</c></b>（官方 Unix 秒）：它是幂等键的组成部分，
/// 用接收时间会让「同一事件被重推」在跨秒时也被当成新事件。
/// </para>
/// </remarks>
internal static class ContactChangeFactory
{
    /// <summary>信封时间（Unix 秒）；解析失败退回当前时间。</summary>
    public static long OccurredAt(WechatCallbackEvent envelope)
        => long.TryParse(envelope.CreateTime, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : SyncRunRepository.NowUnix();

    /// <summary>部门事件（<c>create_party</c> / <c>update_party</c> / <c>delete_party</c>）。</summary>
    public static ContactChange? Department(
        WechatCallbackEvent envelope,
        ContactPartyChangedPayload payload,
        string kind)
    {
        if (string.IsNullOrWhiteSpace(payload.PartyId))
        {
            return null;
        }

        var json = JsonSerializer.Serialize(new
        {
            id = payload.PartyId,
            name = payload.Name,
            parentId = payload.ParentId,
            order = payload.Order,
        });

        return new ContactChange(
            envelope.EventTypeKey,
            ChangeEntityType.Department,
            payload.PartyId!,
            NewEntityId: null,
            kind,
            OccurredAt(envelope),
            json);
    }

    /// <summary>成员事件（<c>create_user</c> / <c>update_user</c> / <c>delete_user</c>）。</summary>
    public static ContactChange? User(
        WechatCallbackEvent envelope,
        ContactUserChangedPayload payload,
        string kind)
    {
        if (string.IsNullOrWhiteSpace(payload.UserId))
        {
            return null;
        }

        var json = JsonSerializer.Serialize(new
        {
            userId = payload.UserId,
            newUserId = payload.NewUserId,
            name = payload.Name,
            department = payload.DepartmentIds,
            mainDepartment = payload.MainDepartmentId,
        });

        return new ContactChange(
            envelope.EventTypeKey,
            ChangeEntityType.User,
            payload.UserId!,
            payload.NewUserId,
            kind,
            OccurredAt(envelope),
            json);
    }
}
