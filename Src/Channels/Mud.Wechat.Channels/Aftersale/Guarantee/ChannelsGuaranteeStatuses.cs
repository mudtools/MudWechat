// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「保障单」DTO（aftersale/getguaranteeorder、aftersale/searchguaranteeorder、
// aftersale/merchantacceptguarantee、aftersale/merchantmodifyguarantee、aftersale/merchantproofguarantee、
// aftersale/merchantrefuseguarantee）+ 保障单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>保障单当前状态（官方 <c>guarantee_order.status</c> 枚举）。</summary>
public static class ChannelsGuaranteeStatuses
{
    /// <summary>等待商家处理。</summary>
    public const string WaitMerchantHandle = "STATUS_WAIT_MERCHANT_HANDLE";

    /// <summary>等待平台处理。</summary>
    public const string WaitPlatformHandle = "STATUS_WAIT_PLATFORM_HANDLE";

    /// <summary>等待用户确认。</summary>
    public const string WaitUserConfirm = "STATUS_WAIT_USER_CONFIRM";

    /// <summary>等待商家举证。</summary>
    public const string WaitMerchantProof = "STATUS_WAIT_MERCHANT_PROOF";

    /// <summary>等待用户举证。</summary>
    public const string WaitUserProof = "STATUS_WAIT_USER_PROOF";

    /// <summary>等待双方举证。</summary>
    public const string WaitBothProof = "STATUS_WAIT_BOTH_PROOF";

    /// <summary>等待 OP 确认。</summary>
    public const string WaitOpConfirm = "STATUS_WAIT_OP_COMFIRM";

    /// <summary>等待支付分付款。</summary>
    public const string WaitPayscoreDone = "STATUS_WAIT_PAYSCORE_DONE";

    /// <summary>无需赔付。</summary>
    public const string NoNeedPay = "STATUS_NO_NEED_PAY";

    /// <summary>赔付中。</summary>
    public const string Paying = "STATUS_PAYING";

    /// <summary>假一赔三金额异常，待确认。</summary>
    public const string PayBlock = "STATUS_PAY_BLOCK";

    /// <summary>赔付成功。</summary>
    public const string PaySucc = "STATUS_PAY_SUCC";

    /// <summary>赔付失败。</summary>
    public const string PayFail = "STATUS_PAY_FAIL";

    /// <summary>用户取消申请。</summary>
    public const string UserCancel = "STATUS_USER_CANCEL";
}
