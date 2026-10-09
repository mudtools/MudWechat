// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// JSAPI / 小程序下单应答（HTTP 200；官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>）。
/// </summary>
/// <remarks>
/// <b>应答仅一个字段</b>（官方原文即如此，不是漏抓）：<c>prepay_id</c>。
/// 它是调起支付所需的预支付交易会话标识，<b>2 小时有效</b>，失效须重新下单获取。
/// 后续「小程序调起支付」端点（docId <c>4012791898</c>）用它换取二次签名参数。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class JsapiPrepayResponse
{
    /// <summary>预支付交易会话标识（<c>prepay_id</c>，必填 string(64)，有效期 2 小时）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }
}
