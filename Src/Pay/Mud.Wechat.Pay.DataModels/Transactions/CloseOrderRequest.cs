// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 关闭订单请求体（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791901"/>（2026-10-09 逐字段核验）。
/// <b>POST</b> <c>/v3/pay/transactions/out-trade-no/{out_trade_no}/close</c>，
/// 必带 <c>Accept: application/json</c> 与 <c>Content-Type: application/json</c>。
/// </para>
/// <para>
/// <b>成功应答为 <c>204 No Content</c>（无包体）</b>，故对应端点返回 <see cref="Task"/>（非泛型）。
/// </para>
/// <para>
/// <b>业务限制（官方原文要点）</b>：① 订单生成后<b>不能关闭</b>；② 订单已支付成功后<b>不能关闭</b>
/// （须走退款）；③ 关单后用户<b>无法再支付</b>；④ 关单结果<b>异步通知</b>（<c>ORDER_CLOSED</c> 非实时），
/// 故关单后仍应经查单确认最终状态；⑤ 已于 <c>time_expire</c> 超时未支付的订单，官方自动关闭。
/// </para>
/// <para>
/// <b>服务商形态</b>：官方页面以 <c>mchid</c> 承载商户号。服务商 / 子商户场景的字段形态须在接入前
/// 以服务商文档中心复核后另行表达，<b>此处不按普通商户形态推断</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CloseOrderRequest
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }
}
