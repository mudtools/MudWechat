// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MerchantStore;

/// <summary>
/// 门店状态（官方 <c>store_state.store_state_value</c>，6 值）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085769"/>，
/// 值集内联印在描述单元格的「可选取值」列表内（服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948577"/> 同），
/// 页内<b>没有</b>独立的「状态说明」附录表 ⇒ 本类是唯一出处。</para>
/// <para><b>六个状态里有三个是"等待他方动作"</b>：<c>RECIPIENT_VERIFYING</c> 等收款商户号超管确认、
/// <c>ADDRESS_VERIFYING</c> 等地址核实、<c>ADDRESS_FAILED</c> 须改地址 ⇒
/// 更新支是 204 无包体，这三者的推进与结论<b>只能靠轮询查询支读本字段</b>，
/// 把 204 当成「门店已可用」会把未生效门店放进收款链路。</para>
/// <para><c>STORE_STATE_CLOSED</c> 是<b>可由商户平台删除且删除后不可恢复</b>的终态（官方原文带「请谨慎操作」），
/// 本 SDK 不提供删除门店端点（官方无该 HTTP 面）。</para>
/// </remarks>
public static class MerchantStoreStateValues
{
    /// <summary>门店可用（官方 <c>STORE_STATE_VALID</c>）：收款信息已确认、地址核实成功，可用于微信支付生态其他业务。</summary>
    public const string Valid = "STORE_STATE_VALID";

    /// <summary>地址核实中（官方 <c>STORE_STATE_ADDRESS_VERIFYING</c>）：暂不支持基于地理位置的流量应用，须等核实结果。</summary>
    public const string AddressVerifying = "STORE_STATE_ADDRESS_VERIFYING";

    /// <summary>地址核实失败（官方 <c>STORE_STATE_ADDRESS_FAILED</c>）：须按 <c>address_failed_reason</c> 修改地址。</summary>
    public const string AddressFailed = "STORE_STATE_ADDRESS_FAILED";

    /// <summary>收款关系被拒或超时（官方 <c>STORE_STATE_RECIPIENT_REFUSED</c>）：超管拒绝、或超 10 天未确认 ⇒ 门店创建失败。</summary>
    public const string RecipientRefused = "STORE_STATE_RECIPIENT_REFUSED";

    /// <summary>停业（官方 <c>STORE_STATE_CLOSED</c>）：不可用于其他业务，可在商户平台删除且<b>删除后不可恢复</b>。</summary>
    public const string Closed = "STORE_STATE_CLOSED";

    /// <summary>待收款商户确认（官方 <c>STORE_STATE_RECIPIENT_VERIFYING</c>）：须由收款商户号超级管理员在微信下发的确认消息内完成。</summary>
    public const string RecipientVerifying = "STORE_STATE_RECIPIENT_VERIFYING";
}
