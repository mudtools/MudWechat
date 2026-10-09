// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transfer;

/// <summary>
/// 商家转账单状态（官方 <c>state</c>，8 值）—— <b>含「是否可原单重试」这一资金安全语义</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716437"/>
/// （2026-10-09 逐值核验，中文说明照录）。
/// </para>
/// <para>
/// <b>🔴 重试语义是本表的重点</b>：<see cref="Accepted"/> 与 <see cref="Processing"/> 两个非终态，
/// 官方明确说明<b>可「原单重试」</b> —— 即<b>不要</b>更换 <c>out_bill_no</c>。
/// 与「发起转账遇错不得换单重试」的红线配套：<b>先查单 → 看本状态 → 原单重试或按终态处理</b>。
/// </para>
/// <para>
/// <b>终态与非终态</b>：终态为 <see cref="Success"/> / <see cref="Fail"/> / <see cref="Cancelled"/>；
/// 其余五个为非终态（须继续查单）。
/// </para>
/// </remarks>
public static class TransferBillStates
{
    /// <summary>转账已受理（官方 <c>ACCEPTED</c>，非终态）：官方说明「<b>可原单重试</b>」。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>
    /// 转账锁定资金中（官方 <c>PROCESSING</c>，非终态）：官方说明「如果一直停留在该状态，
    /// 建议检查账户余额是否足够，如余额不足，可充值后再<b>原单重试</b>」。
    /// </summary>
    public const string Processing = "PROCESSING";

    /// <summary>待收款用户确认（官方 <c>WAIT_USER_CONFIRM</c>，非终态）：资金已锁定，可拉起微信收款确认页。</summary>
    public const string WaitUserConfirm = "WAIT_USER_CONFIRM";

    /// <summary>转账中（官方 <c>TRANSFERING</c>，非终态）：可拉起微信收款确认页<b>再次重试确认收款</b>。</summary>
    public const string Transfering = "TRANSFERING";

    /// <summary>转账成功（官方 <c>SUCCESS</c>，<b>终态</b>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>
    /// 转账失败（官方 <c>FAIL</c>，<b>终态</b>）：官方说明「若需重新向用户转账，
    /// 请<b>重新生成单据</b>并再次发起」—— 这才是允许换单的时点。
    /// </summary>
    public const string Fail = "FAIL";

    /// <summary>转账撤销中（官方 <c>CANCELING</c>，非终态）：撤销请求已受理，须查单确认最终状态。</summary>
    public const string Canceling = "CANCELING";

    /// <summary>转账撤销完成（官方 <c>CANCELLED</c>，<b>终态</b>）。</summary>
    public const string Cancelled = "CANCELLED";
}
