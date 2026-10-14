// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantFund;

/// <summary>
/// 账户余额应答（实时余额与日终余额<b>两支共用</b>本型，金额单位是<b>分</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：查询账户实时余额 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012721477"/>、
/// 查询账户日终余额 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012721446"/>，2026-10-14 逐字段核验。</para>
/// <para><b>两页应答表逐字段相同</b>（<c>available_amount</c> 必填 integer、<c>pending_amount</c> 选填 integer），
/// 差异只在<b>定位方式</b>：实时余额只有 path 的 <c>account_type</c>，日终余额额外要求 query 的
/// <c>date</c>（必填 string(10)、<c>YYYY-MM-DD</c>）⇒ 按「表相同则共用」归并为一型（守卫 MF-B6）。</para>
/// <para><b>金额是官方 int 型的「分」</b>，本 SDK 以 <c>long</c> 承载：余额量级可越出 int32
/// （约 21.47 亿分 = 2147 万元），用 <c>int</c> 会在头部商户上静默溢出（守卫 MF-B3 锁定该 CLR 宽度选择）。</para>
/// <para><b><c>pending_amount</c> 缺失不等于 0</b>：官方标<b>选填</b> ⇒ 宿主须按 <c>null</c> 与 0 分列处理，
/// 把 <c>null</c> 当 0 会低估非可用头寸。两键的业务口径以官方页面描述为准，本 SDK 不做解释性加工。</para>
/// <para><b>本型不含可提现头寸</b>：余额查询只给账户余额，「能提多少」还要扣在途提现与冻结，
/// 而提现族在【平台商户】面（本包零端点）⇒ 不得由余额推算可提金额。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantFund")]
public class MerchantFundBalanceResponse : WechatPayResponse
{
    /// <summary>可用余额（<c>available_amount</c>，必填 integer）：单位<b>分</b>。</summary>
    [JsonPropertyName("available_amount")]
    public long? AvailableAmount { get; set; }

    /// <summary>待结算/不可用金额（<c>pending_amount</c>，选填 integer）：单位<b>分</b>，官方键语义随账户形态。</summary>
    [JsonPropertyName("pending_amount")]
    public long? PendingAmount { get; set; }
}
