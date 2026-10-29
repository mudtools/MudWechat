// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>
/// 商户余额信息（官方 <c>Object BalanceInfo</c>，由 <c>POST /shop/funds/getbalance</c> 返回）。
/// </summary>
/// <remarks>
/// <para>
/// <b>金额单位官方本页未标注</b>（参数表只写类型 <c>number</c> 与「可提现余额 / 待结算余额」语义，示例 45331 / 7549）
/// ⇒ 本仓照录 <c>long</c>、不做单位换算；同域内官方唯一明写「单位：分」的是 <c>submitwithdraw</c> 的 <c>amount</c>，
/// 两者不得互相推断（守卫 SHOPF5 锁定类型口径）。
/// </para>
/// <para><b>可提现余额 ≠ 账户总额</b>：官方另给 <c>pending_amount</c>（待结算），资金流水详情侧的 <c>balance</c> 才是「可提现 + 待结算」合计。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsBalanceInfo
{
    /// <summary>可提现余额（<c>available_amount</c>，官方类型 number，本页未标金额单位）。</summary>
    [JsonPropertyName("available_amount")]
    public long? AvailableAmount { get; set; }

    /// <summary>待结算余额（<c>pending_amount</c>，官方类型 number，本页未标金额单位）。</summary>
    [JsonPropertyName("pending_amount")]
    public long? PendingAmount { get; set; }
}
