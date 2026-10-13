// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「支付者」（上传侧 <c>payer</c>，发货信息录入与合单录入共用）。
/// </summary>
/// <remarks>
/// <para><b>必填</b>（官方请求体表 <c>payer</c> 标「是」，其内 <c>openid</c> 亦标「是」）：
/// 漏传即无法定位支付单，官方错误码族 10060001「支付单不存在」。</para>
/// <para><c>openid</c> 属用户级标识（禁止落日志、遥测或异常消息，与 <c>session_key</c> 同级处置）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderPayer
{
    /// <summary>openid（官方类型 <c>string</c>，必填）：用户标识，用户在小程序appid下的唯一标识。 下单前需获取到用户的Openid 字符字节限制: [1, 128]</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
