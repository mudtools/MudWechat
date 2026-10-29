// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 成员镜像（方案 §5.2 <c>org_member</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>敏感字段可能为空是常态</b>：自 2022-06-20 起新建自建/代开发应用不再返回头像/性别/手机/邮箱/地址
/// （需 oauth2 手工授权），列表页必须按「字段可能为空」设计，详情页缺失时显示「该应用无此字段权限」。
/// </para>
/// <para>
/// <b>库文件含个人信息</b>：<c>mobile</c> / <c>email</c> 明文落库，库文件随 <c>**/bin/</c> 被 gitignore，
/// <b>不得提交</b>（README 已标注）。
/// </para>
/// </remarks>
[Table(Name = "org_member")]
public sealed class OrgMember
{
    /// <summary>成员账号（企业内唯一，不区分大小写，1~64 字节）。</summary>
    [Column(Name = "user_id", IsPrimary = true)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>成员名称（可能为空：2022-08-15 后新回调 URL 只回 userid 子集）。</summary>
    [Column(Name = "name")]
    public string? Name { get; set; }

    /// <summary>职务信息。</summary>
    [Column(Name = "position")]
    public string? Position { get; set; }

    /// <summary>
    /// 性别（<b>字符串</b>）：官方参数表标整数但示例按 <c>"1"</c>/<c>"2"</c> 传输，
    /// <c>UserInfo.Gender</c> / <c>CreateUserRequest.Gender</c> 均为 <c>string?</c>，勿改建库类型。
    /// </summary>
    [Column(Name = "gender")]
    public string? Gender { get; set; }

    /// <summary>手机号（<b>敏感</b>；列表页掩码、仅详情页明文）。</summary>
    [Column(Name = "mobile")]
    public string? Mobile { get; set; }

    /// <summary>邮箱（<b>敏感</b>；同上）。</summary>
    [Column(Name = "email")]
    public string? Email { get; set; }

    /// <summary>主部门 id（<c>main_department</c>）。</summary>
    [Column(Name = "main_dept_id")]
    public int? MainDeptId { get; set; }

    /// <summary>企微成员状态：1 已激活 / 2 已禁用 / 4 未激活 / 5 退出企业。</summary>
    [Column(Name = "status")]
    public int? Status { get; set; }

    /// <summary>
    /// 本地删除态：1 正常 / 2 疑似删除（本轮全量未出现）/ 3 已删除（人工确认）。
    /// </summary>
    /// <remarks>
    /// 方案 §5.2 把 <c>org_member.status</c> 定为「企微 status（1/2/4/5）」，而 §6.4「missing 只写 status=2」
    /// 复用了同一个列名 —— 两个 <c>2</c> 含义完全不同（企微 2 = 已禁用）。故这里<b>另开一列</b>
    /// <c>local_status</c> 承载本地删除态，语义与 <see cref="OrgDepartment.Status"/> 对齐；这是对有冲突的
    /// 方案条文的显式取舍，已在 README「与方案的偏差」中登记。
    /// </remarks>
    [Column(Name = "local_status")]
    public int LocalStatus { get; set; } = OrgStatus.Normal;

    /// <summary>远端事实指纹（判定 <c>updated</c> 用；不含敏感字段以外的本地派生值）。</summary>
    [Column(Name = "source_hash")]
    public string? SourceHash { get; set; }

    /// <summary>本地快照时间（Unix 秒）。</summary>
    [Column(Name = "synced_at")]
    public long SyncedAt { get; set; }
}
