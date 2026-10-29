// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 部门镜像（方案 §5.2 <c>org_department</c>）：企业微信是<b>唯一权威源</b>，本表只是本地快照。
/// </summary>
/// <remarks>
/// <para>
/// <c>Id</c> 用企微部门 id 直接作主键（32 位整型）；根部门恒为 <c>1</c>。
/// </para>
/// <para>
/// <b>时间列一律 <see cref="long"/>（Unix 秒）</b>：SQLite provider 对 <c>DateTimeOffset</c> 列
/// 的 <c>ORDER BY</c> 支持有坑，DM-14 已实测过，本 Demo 不重复踩。
/// </para>
/// </remarks>
[Table(Name = "org_department")]
public sealed class OrgDepartment
{
    /// <summary>企微部门 id（根部门 = 1）。</summary>
    [Column(Name = "id", IsPrimary = true)]
    public int Id { get; set; }

    /// <summary>部门名称（<c>department/get</c> 的 <c>name</c>；第三方通讯录应用取不到）。</summary>
    [Column(Name = "name")]
    public string? Name { get; set; }

    /// <summary>英文名称。</summary>
    [Column(Name = "name_en")]
    public string? NameEn { get; set; }

    /// <summary>父部门 id（根部门为 0）。</summary>
    [Column(Name = "parent_id")]
    public int ParentId { get; set; }

    /// <summary>在父部门中的次序值 —— <b>值小靠前</b>（与成员 <c>order</c> 语义相反）。</summary>
    [Column(Name = "order_no")]
    public long OrderNo { get; set; }

    /// <summary>部门负责人 userid 列表的 JSON 数组（<c>department_leader</c>）。</summary>
    [Column(Name = "leaders_json")]
    public string? LeadersJson { get; set; }

    /// <summary>状态：1 正常 / 2 疑似删除（本轮全量未出现）/ 3 已删除（人工确认）。</summary>
    [Column(Name = "status")]
    public int Status { get; set; } = OrgStatus.Normal;

    /// <summary>远端事实的指纹（规范化字段拼接后的 SHA256），用于判定 <c>updated</c>。</summary>
    [Column(Name = "source_hash")]
    public string? SourceHash { get; set; }

    /// <summary>本地快照时间（Unix 秒）。</summary>
    [Column(Name = "synced_at")]
    public long SyncedAt { get; set; }
}

/// <summary>镜像行状态（方案 §5.2 / §6.4）。</summary>
public static class OrgStatus
{
    /// <summary>正常（远端存在且本轮出现过）。</summary>
    public const int Normal = 1;

    /// <summary>疑似删除：本轮全量未出现 —— <b>不物理删除</b>，也不影响读端点默认查询。</summary>
    public const int Suspected = 2;

    /// <summary>已删除：人工确认后置位（唯一物理删除的替代形态，读端点 <c>?includeDeleted=1</c> 才可见）。</summary>
    public const int Deleted = 3;
}
