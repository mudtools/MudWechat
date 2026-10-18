// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 撤销订单请求体（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012382161"/>；
/// <b>POST /v3/pay/transactions/out-trade-no/{out_trade_no}/reverse</b>。</para>
/// <para><b>业务场景</b>：支付交易返回失败或支付系统超时时，调用本接口撤销交易。
/// 若交易已成功，则尝试退款；若交易未成功，则直接关闭订单。
/// <b>注意</b>：撤销订单与关闭订单不同，撤销会触发退款流程。</para>
/// <para><b>必填</b>：<c>out_trade_no</c>（路径参数）+ 请求体中至少一个字段（目前官方契约仅校验路径参数）。</para>
/// <para><b>频率限制</b>：同一订单号频繁调用会被限流，官方建议做好重试控制。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class ReverseOrderRequest
{
    /// <summary>
    /// 商户订单号（<c>out_trade_no</c>，必填 string(32)）：6-32 字符，
    /// 只能是数字、大小写字母 <c>_-|*</c>，同一商户号下唯一。
    /// 此字段同时作为<b>路径参数</b>出现在路由中。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}
