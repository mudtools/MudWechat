// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 物流联系方式（官方 <c>contact</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShippingInfo.html"/></para>
/// <para><b>隐私字段</b>：联系方式不得进日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderContact
{
    /// <summary>寄件人联系方式（官方 <c>consignor_contact</c>，选填；敏感字段勿落日志）。</summary>
    [JsonPropertyName("consignor_contact")]
    public string? ConsignorContact { get; set; }

    /// <summary>收件人联系方式（官方 <c>receiver_contact</c>，选填；敏感字段勿落日志）。</summary>
    [JsonPropertyName("receiver_contact")]
    public string? ReceiverContact { get; set; }
}
