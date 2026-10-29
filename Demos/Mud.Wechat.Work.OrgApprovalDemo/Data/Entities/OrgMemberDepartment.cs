// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 「成员 × 部门」关系行（方案 §5.2 <c>org_member_department</c>，复合主键）。
/// </summary>
/// <remarks>
/// <c>user/list_id</c> 返回的 <c>dept_user</c> 本身就是关系行（同一人多部门即多行），故本表直接承载它；
/// <c>order_no</c> 是成员在<b>该部门内</b>的排序值 —— <b>值大靠前</b>，与部门 <c>order</c> 语义相反。
/// </remarks>
[Table(Name = "org_member_department")]
public sealed class OrgMemberDepartment
{
    /// <summary>成员账号。</summary>
    [Column(Name = "user_id", IsPrimary = true)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>部门 id。</summary>
    [Column(Name = "dept_id", IsPrimary = true)]
    public int DeptId { get; set; }

    /// <summary>是否在该部门为负责人：1 是 / 0 否。</summary>
    [Column(Name = "is_leader")]
    public int IsLeader { get; set; }

    /// <summary>成员在该部门内的排序值（<b>值大靠前</b>）。</summary>
    [Column(Name = "order_no")]
    public long OrderNo { get; set; }
}
