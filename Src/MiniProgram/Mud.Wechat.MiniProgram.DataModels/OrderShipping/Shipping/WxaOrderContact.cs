// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「联系方式」（<c>contact</c>，上传侧与查询侧字段面相同 ⇒ 两个方向共用本类型）。
/// </summary>
/// <remarks>
/// <para><b>掩码传输</b>（官方原文）：寄件人与收件人联系方式均须采用掩码传输，<b>最后 4 位数字不能打掩码</b>，
/// 值限制 <c>0 ≤ value ≤ 1024</c> ⇒ SDK 侧不做二次脱敏、也不得补全明文。</para>
/// <para><b>顺丰必填、二选一</b>（官方原文「当发货的物流公司为顺丰时，联系方式为必填，收件人或寄件人联系方式二选一」）。</para>
/// <para>联系方式属个人敏感信息（禁止落日志、遥测或异常消息）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderContact
{
    /// <summary>consignor_contact（官方类型 <c>string</c>）：寄件人联系方式，采用掩码传输，最后4位数字不能打掩码 值限制: 0 ≤ value ≤ 1024</summary>
    [JsonPropertyName("consignor_contact")]
    public string? ConsignorContact { get; set; }

    /// <summary>receiver_contact（官方类型 <c>string</c>）：收件人联系方式，采用掩码传输，最后4位数字不能打掩码 值限制: 0 ≤ value ≤ 1024</summary>
    [JsonPropertyName("receiver_contact")]
    public string? ReceiverContact { get; set; }
}
