// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantOperationManage;

/// <summary>
/// 商户管控措施类型（<c>limitation_type</c>）。
/// </summary>
public static class MerchantLimitationTypes
{
    /// <summary>限制收款（<c>LIMITATION_TYPE_RECEIPT</c>）。</summary>
    public const string Receipt = "LIMITATION_TYPE_RECEIPT";

    /// <summary>限制提现（<c>LIMITATION_TYPE_WITHDRAW</c>）。</summary>
    public const string Withdraw = "LIMITATION_TYPE_WITHDRAW";

    /// <summary>限制分账（<c>LIMITATION_TYPE_PROFIT_SHARING</c>）。</summary>
    public const string ProfitSharing = "LIMITATION_TYPE_PROFIT_SHARING";
}
