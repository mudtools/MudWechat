// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>diff 里的一个实体引用（<paramref name="Kind"/> 为 <c>dept</c> / <c>user</c>）。</summary>
/// <param name="Kind">实体种类。</param>
/// <param name="Id">实体 id（部门 id 或 userid）。</param>
/// <param name="Name">可读名称（可能为空：部门名/成员名都受权限分层影响）。</param>
public sealed record EntityRef(string Kind, string Id, string? Name);

/// <summary>
/// 一轮同步的变更报告（方案 §4.3 <c>/api/sync/runs/{id}/diff</c> 的数据源）。
/// </summary>
/// <remarks>
/// <para>
/// <b><c>Missing</c> 只是「本轮全量没见到」，不是删除</b>（方案 §6.2 失败语义 / §6.4）：
/// 可见范围收窄、权限被改、某次 API 半途失败都会让本地记录凭空「消失」，
/// 故缺项只标「疑似删除」并由人工确认。
/// </para>
/// <para>
/// <c>Guardian</c> 记录本轮是否触发了「人数波动保护」：命中时整轮降级为仅告警、不标任何 missing，
/// 前端要能看见这条原因，否则使用者会以为是同步失败。
/// </para>
/// </remarks>
/// <param name="Created">本轮新增。</param>
/// <param name="Updated">本轮字段有变化（指纹不同）。</param>
/// <param name="Missing">本轮未出现（疑似删除）。</param>
/// <param name="Counts">计数汇总（<c>deptCreated</c> / <c>userCreated</c> …）。</param>
/// <param name="Guardian">人数波动保护的结论说明；未触发为 <see langword="null"/>。</param>
public sealed record OrgDiff(
    List<EntityRef> Created,
    List<EntityRef> Updated,
    List<EntityRef> Missing,
    Dictionary<string, int> Counts,
    string? Guardian)
{
    private static readonly JsonSerializerOptions StorageOptions = new(JsonSerializerDefaults.Web);

    /// <summary>序列化后写入 <c>org_sync_run.diff_json</c>（camelCase，前端直接消费）。</summary>
    public string ToJson() => JsonSerializer.Serialize(this, StorageOptions);

    /// <summary>从台账列反序列化；空值或坏数据返回 <see langword="null"/>（读端点据此回「本轮没有报告」）。</summary>
    public static OrgDiff? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<OrgDiff>(json, StorageOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

/// <summary>人数波动保护的阈值（方案 §6.4 第 3 条）。</summary>
public static class DiffGuard
{
    /// <summary>本轮人数低于上一轮的该比例时，整轮降级为「仅告警」。</summary>
    public const double MemberShrinkRatio = 0.8;

    /// <summary>部门数同理（部门被大面积移出可见范围的场景更常见）。</summary>
    public const double DepartmentShrinkRatio = 0.8;
}
