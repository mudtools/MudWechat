// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 协商历史操作类型取值（官方 <c>Res.history.operate_type</c> 枚举表，20 值）。
/// </summary>
/// <remarks>
/// <para><b>照录官方拼写，含两处官方 typo</b>：<c>USER_COMFIRM_COMPLAINT</c>（应为 CONFIRM）、<c>MERCHANT_REFUSE_RERUND</c>（应为 REFUND）⇒ 线格式值是官方原文，属性名只做大小写转换，<b>不得</b>「顺手修正」为正确拼写（守卫 XP5 逐值锁定）。</para>
/// <para><b>六值是系统通知而非人发言</b>（*_SYSTEM_MESSAGE 三支 + 撤诉/确认两支仅存在于历史单）⇒ 做「待回复」判据时须排除系统通知支。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_negotiation_history.html"/></para>
/// </remarks>
public static class WxaXPayOperateTypes
{
    /// <summary>用户提交投诉（<c>USER_CREATE_COMPLAINT</c>）</summary>
    public const string UserCreateComplaint = "USER_CREATE_COMPLAINT";

    /// <summary>用户继续投诉（<c>USER_CONTINUE_COMPLAINT</c>）</summary>
    public const string UserContinueComplaint = "USER_CONTINUE_COMPLAINT";

    /// <summary>用户留言（<c>USER_RESPONSE</c>）</summary>
    public const string UserResponse = "USER_RESPONSE";

    /// <summary>平台留言（<c>PLATFORM_RESPONSE</c>）</summary>
    public const string PlatformResponse = "PLATFORM_RESPONSE";

    /// <summary>商户留言（<c>MERCHANT_RESPONSE</c>）</summary>
    public const string MerchantResponse = "MERCHANT_RESPONSE";

    /// <summary>商户申请结单（<c>MERCHANT_CONFIRM_COMPLETE</c>）</summary>
    public const string MerchantConfirmComplete = "MERCHANT_CONFIRM_COMPLETE";

    /// <summary>用户提交投诉系统通知（<c>USER_CREATE_COMPLAINT_SYSTEM_MESSAGE</c>）</summary>
    public const string UserCreateComplaintSystemMessage = "USER_CREATE_COMPLAINT_SYSTEM_MESSAGE";

    /// <summary>投诉单发起全额退款系统通知（<c>COMPLAINT_FULL_REFUNDED_SYSTEM_MESSAGE</c>）</summary>
    public const string ComplaintFullRefundedSystemMessage = "COMPLAINT_FULL_REFUNDED_SYSTEM_MESSAGE";

    /// <summary>用户继续投诉系统通知（<c>USER_CONTINUE_COMPLAINT_SYSTEM_MESSAGE</c>）</summary>
    public const string UserContinueComplaintSystemMessage = "USER_CONTINUE_COMPLAINT_SYSTEM_MESSAGE";

    /// <summary>用户主动撤诉（只存在于历史投诉单的协商历史中）（<c>USER_REVOKE_COMPLAINT</c>）</summary>
    public const string UserRevokeComplaint = "USER_REVOKE_COMPLAINT";

    /// <summary>用户确认投诉解决（只存在于历史投诉单的协商历史中）（<c>USER_COMFIRM_COMPLAINT</c>）</summary>
    public const string UserComfirmComplaint = "USER_COMFIRM_COMPLAINT";

    /// <summary>平台催办（<c>PLATFORM_HELP_APPLICATION</c>）</summary>
    public const string PlatformHelpApplication = "PLATFORM_HELP_APPLICATION";

    /// <summary>用户申请平台协助（<c>USER_APPLY_PLATFORM_HELP</c>）</summary>
    public const string UserApplyPlatformHelp = "USER_APPLY_PLATFORM_HELP";

    /// <summary>商户同意退款申请（<c>MERCHANT_APPROVE_REFUND</c>）</summary>
    public const string MerchantApproveRefund = "MERCHANT_APPROVE_REFUND";

    /// <summary>商户拒绝退款申请, 此时操作内容里展示拒绝原因（<c>MERCHANT_REFUSE_RERUND</c>）</summary>
    public const string MerchantRefuseRerund = "MERCHANT_REFUSE_RERUND";

    /// <summary>用户提交满意度调查结果,此时操作内容里会展示满意度分数（<c>USER_SUBMIT_SATISFACTION</c>）</summary>
    public const string UserSubmitSatisfaction = "USER_SUBMIT_SATISFACTION";

    /// <summary>服务订单已取消（<c>SERVICE_ORDER_CANCEL</c>）</summary>
    public const string ServiceOrderCancel = "SERVICE_ORDER_CANCEL";

    /// <summary>服务订单已完成（<c>SERVICE_ORDER_COMPLETE</c>）</summary>
    public const string ServiceOrderComplete = "SERVICE_ORDER_COMPLETE";

    /// <summary>投诉单发起部分退款系统通知（<c>COMPLAINT_PARTIAL_REFUNDED_SYSTEM_MESSAGE</c>）</summary>
    public const string ComplaintPartialRefundedSystemMessage = "COMPLAINT_PARTIAL_REFUNDED_SYSTEM_MESSAGE";

    /// <summary>投诉单退款到账系统通知（<c>COMPLAINT_REFUND_RECEIVED_SYSTEM_MESSAGE</c>）</summary>
    public const string ComplaintRefundReceivedSystemMessage = "COMPLAINT_REFUND_RECEIVED_SYSTEM_MESSAGE";
}
