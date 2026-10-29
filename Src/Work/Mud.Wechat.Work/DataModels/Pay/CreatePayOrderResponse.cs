// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 小程序下单响应体（<c>/cgi-bin/miniapppay/create_order</c>，普通支付域）。
/// <para>官方业务限制：prepay_id 用于后续接口调用（如获取支付签名），该值有效期为 2 小时。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class CreatePayOrderResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置预支付交易会话标识（1~64 个字符）：
    /// 用于后续接口调用，该值有效期为 2 小时。
    /// </summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }
}
