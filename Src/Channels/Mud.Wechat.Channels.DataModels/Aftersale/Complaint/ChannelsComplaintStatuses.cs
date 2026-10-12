// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「纠纷单」DTO（aftersale/getcomplaintorder、aftersale/addcomplaintmaterial、
// aftersale/addcomplaintproof、aftersale/syncworkorder）+ 纠纷单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>纠纷单状态（官方 <c>getcomplaintorder</c> 的 <c>status</c> 枚举）。</summary>
public static class ChannelsComplaintStatuses
{
    /// <summary>待商家处理纠纷。</summary>
    public const int WaitMerchantHandle = 100;

    /// <summary>待客服处理。</summary>
    public const int WaitCustomerServiceHandle = 101;

    /// <summary>取消客服介入。</summary>
    public const int CancelCustomerServiceIntervention = 102;

    /// <summary>客服处理中。</summary>
    public const int CustomerServiceHandling = 103;

    /// <summary>待用户补充凭证。</summary>
    public const int WaitUserProof = 104;

    /// <summary>用户已补充凭证。</summary>
    public const int UserProofAdded = 105;

    /// <summary>待商家补充凭证。</summary>
    public const int WaitMerchantProof = 106;

    /// <summary>商家已补充凭证。</summary>
    public const int MerchantProofAdded = 107;

    /// <summary>待双方补充凭证。</summary>
    public const int WaitBothProof = 108;

    /// <summary>双方补充凭证超时。</summary>
    public const int BothProofTimeout = 109;

    /// <summary>商家申诉中。</summary>
    public const int MerchantAppealing = 111;

    /// <summary>调解完成。</summary>
    public const int MediationDone = 112;

    /// <summary>待客服核实。</summary>
    public const int WaitCustomerServiceVerify = 113;

    /// <summary>重新退款中。</summary>
    public const int Refunding = 114;

    /// <summary>退款核实完成。</summary>
    public const int RefundVerifyDone = 115;

    /// <summary>调解关闭。</summary>
    public const int MediationClosed = 116;

    /// <summary>用户补充凭证超时。</summary>
    public const int UserProofTimeout = 305;

    /// <summary>商家补充凭证超时。</summary>
    public const int MerchantProofTimeout = 307;

    /// <summary>待商家上传工单。</summary>
    public const int WaitMerchantWorkOrder = 400;
}
