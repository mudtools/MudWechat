// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantManage;

/// <summary>
/// 商户管理记录状态值（<c>manage_record_state</c>）。
/// </summary>
public static class MerchantManageRecordStates
{
    /// <summary>待提交资料（<c>PENDING_SUBMISSION</c>）。</summary>
    public const string PendingSubmission = "PENDING_SUBMISSION";

    /// <summary>审核中（<c>AUDITING</c>）。</summary>
    public const string Auditing = "AUDITING";

    /// <summary>审核通过（<c>AUDIT_PASSED</c>）。</summary>
    public const string AuditPassed = "AUDIT_PASSED";

    /// <summary>审核驳回（<c>AUDIT_REJECTED</c>）。</summary>
    public const string AuditRejected = "AUDIT_REJECTED";
}
