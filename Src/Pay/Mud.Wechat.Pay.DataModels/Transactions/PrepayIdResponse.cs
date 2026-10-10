// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 「仅返回 <c>prepay_id</c>」的下单应答（JSAPI / 小程序 / APP 三族共用；HTTP 200）。
/// </summary>
/// <remarks>
/// <para>
/// <b>应答仅一个业务字段</b>（官方原文即如此，不是漏抓）：<c>prepay_id</c>。
/// 它是调起支付所需的预支付交易会话标识，<b>2 小时有效</b>，失效须重新下单获取。
/// 后续「小程序调起支付」端点（docId <c>4012791898</c>）用它换取二次签名参数。
/// </para>
/// <para>
/// <b>命名与共用范围</b>：JSAPI（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>）与
/// APP（docId <c>4013070347</c>，SKIT 登记，官方逐页核验待补）两页的应答表<b>逐项一致</b>
/// （都只有 <c>prepay_id</c>）⇒ 按本线纪律「<b>表相同则共用，表不同则分建</b>」共用本类型，
/// 故类型名<b>不带产品族前缀</b>（同合单域 <c>CombinePrepayResponse</c> 的处理）。
/// Native（<c>code_url</c>）与 H5（<c>h5_url</c>）应答形态不同 ⇒ 各自独立类型，见
/// <see cref="NativePrepayResponse"/> / <see cref="H5PrepayResponse"/>。
/// </para>
/// <para>
/// <b>继承 <see cref="WechatPayResponse"/> 的理由</b>：下单失败时官方返回 4xx + <c>{"code","message"}</c>，
/// 继承后错误体可原样落到 <c>Code</c>/<c>Message</c>，调用方经 <c>WechatPayException.ThrowIfFailed</c> 判错；
/// 成功时 <c>code</c> 缺省、<c>prepay_id</c> 有值（成功响应体不会带上 <c>code</c> 字段）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class PrepayIdResponse : WechatPayResponse
{
    /// <summary>预支付交易会话标识（<c>prepay_id</c>，必填 string(64)，有效期 2 小时）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }
}
