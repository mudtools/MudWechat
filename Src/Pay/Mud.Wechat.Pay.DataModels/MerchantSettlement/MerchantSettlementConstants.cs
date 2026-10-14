// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantSettlement;

/// <summary>
/// 结算规则申请单状态（官方 <c>application_state</c>，3 值 string）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697924"/>（创建申请单）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012698574"/>（查询申请单状态），2026-10-14 逐字核验。</para>
/// <para><b>终态只有两个</b>：<see cref="Passed"/> / <see cref="Failed"/>；<see cref="Processing"/> 是中间态
/// ⇒ 创建应答里就可能出现 <c>PROCESSING</c>，「拿到 <c>application_id</c>」不等于规则已生效，
/// 唯一闭环路径是轮询查询支（守卫 MSET-B4 锁三值清单）。</para>
/// </remarks>
public static class MerchantSettlementApplicationStates
{
    /// <summary>处理中（官方 <c>PROCESSING</c>）。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>审核通过（官方 <c>PASSED</c>）。</summary>
    public const string Passed = "PASSED";

    /// <summary>审核失败（官方 <c>FAILED</c>）。</summary>
    public const string Failed = "FAILED";
}
