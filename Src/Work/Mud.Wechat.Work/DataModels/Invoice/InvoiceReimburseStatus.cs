// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Invoice;

/// <summary>
/// 电子发票报销状态常量（<c>reimburse_status</c>）。
/// </summary>
/// <remarks>
/// 官方约束：报销（核销）为不可逆操作——发票核销后将从用户卡包中移除，请慎重调用；
/// 报销方须保证在报销、锁定、解锁后及时将状态同步至微信端，保证用户发票可以正常使用。
/// </remarks>
public static class InvoiceReimburseStatus
{
    /// <summary>发票初始状态，未锁定。</summary>
    public const string Init = "INVOICE_REIMBURSE_INIT";

    /// <summary>发票已锁定，无法重复提交报销。</summary>
    public const string Lock = "INVOICE_REIMBURSE_LOCK";

    /// <summary>发票已核销，从用户卡包中移除。</summary>
    public const string Closure = "INVOICE_REIMBURSE_CLOSURE";
}
