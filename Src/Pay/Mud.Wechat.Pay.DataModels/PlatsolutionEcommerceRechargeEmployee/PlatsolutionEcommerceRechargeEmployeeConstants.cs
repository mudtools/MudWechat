// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRechargeEmployee;

/// <summary>
/// 可扫码充值员工的登记关系状态（<c>state</c>）取值 —— <b>两值，与出款族 <c>state</c> 六值集无关</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013521743"/>（添加）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013521753"/>（删除，应答与添加支同形）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013521757"/>（列表，包在 <c>employees</c> 数组内），三页同集。
/// </para>
/// <para>
/// <b>本字段名也是 <c>state</c>，但语义是「员工与二级商户的登记关系是否生效」</b>，
/// 与出款族付款单状态（<see cref="Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceMchTransfer.PlatsolutionEcommerceTransferStates"/> 六值）
/// 同名<b>不同集</b> ⇒ 判定器不得共用；<c>DELETED</c> 是删除后的可查状态而非「记录不存在」，
/// 列表支返回的是含 <c>DELETED</c> 的全量登记记录。
/// </para>
/// </remarks>
public static class PlatsolutionEcommerceRechargeEmployeeStates
{
    /// <summary>生效中（<c>EFFECTIVE</c>）：该员工可对该二级商户发起扫码充值。</summary>
    public const string Effective = "EFFECTIVE";

    /// <summary>已删除（<c>DELETED</c>）：登记关系已解除，扫码面失效。</summary>
    public const string Deleted = "DELETED";
}
