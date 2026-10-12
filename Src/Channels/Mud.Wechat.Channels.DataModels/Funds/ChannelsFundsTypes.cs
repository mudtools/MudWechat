// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>资金类型常量（官方 <c>funds_type</c> 枚举值，<c>getfundsflowdetail</c>）。</summary>
public static class ChannelsFundsTypes
{
    /// <summary>未知。</summary>
    public const int Unknown = 0;

    /// <summary>订单支付。</summary>
    public const int OrderPay = 1;

    /// <summary>订单退款。</summary>
    public const int OrderRefund = 3;

    /// <summary>提现。</summary>
    public const int Withdraw = 4;

    /// <summary>提现退票。</summary>
    public const int WithdrawRefund = 5;

    /// <summary>达人佣金。</summary>
    public const int PromoterCommission = 10;

    /// <summary>技术服务费。</summary>
    public const int PlatformCommission = 11;

    /// <summary>带货机构服务费。</summary>
    public const int LoaderOrganizationCommission = 12;

    /// <summary>极速退款垫资。</summary>
    public const int ExpressRefundAdvance = 14;

    /// <summary>回补极速退款垫资。</summary>
    public const int ExpressRefundAdvanceSupplement = 15;

    /// <summary>运费险。</summary>
    public const int FreightInsurance = 16;

    /// <summary>预付运费退回。</summary>
    public const int PreFreightRefund = 17;

    /// <summary>平台优惠补贴。</summary>
    public const int PlatformSubsidy = 19;

    /// <summary>补交运费。</summary>
    public const int FreightMakeUp = 20;

    /// <summary>国补发放。</summary>
    public const int NationalSubsidy = 21;

    /// <summary>注销提现。</summary>
    public const int CancelWithdraw = 22;

    /// <summary>注销提现退票。</summary>
    public const int CancelWithdrawRefund = 23;

    /// <summary>平台赔付。</summary>
    public const int PlatformCompensation = 25;
}
