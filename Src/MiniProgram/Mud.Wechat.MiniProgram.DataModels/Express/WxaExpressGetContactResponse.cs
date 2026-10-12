// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>获取面单联系人信息应答（<c>sender</c> / <c>receiver</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetContactResponse : WxaResponse
{
    /// <summary>运单 ID（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>发件人信息（<c>sender</c>），见 <see cref="WxaExpressWaybillContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressWaybillContact? Sender { get; set; }

    /// <summary>收件人信息（<c>receiver</c>），见 <see cref="WxaExpressWaybillContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressWaybillContact? Receiver { get; set; }
}
