// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询扣费主体响应（<c>POST /cgi-bin/express/intracity/getpaymode</c>）。</summary>
/// <remarks>
/// <c>pay_mode</c> 取值见 <see cref="WxaExpressPayModes"/>。
/// <c>pay_appid</c> 与 <c>pay_component_appid</c> <b>互斥</b>：扣费主体为小程序时返回前者、为服务商时返回后者。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPayModeResponse : WxaResponse
{
    /// <summary>扣费主体（<c>pay_mode</c>；<c>PAY_MODE_STORE</c> 门店 / <c>PAY_MODE_APP</c> 小程序 / <c>PAY_MODE_COMPONENT</c> 服务商）。</summary>
    [JsonPropertyName("pay_mode")]
    public string? PayMode { get; set; }

    /// <summary>扣费小程序 AppId（<c>pay_appid</c>；扣费主体为小程序时返回）。</summary>
    [JsonPropertyName("pay_appid")]
    public string? PayAppId { get; set; }

    /// <summary>扣费服务商 AppId（<c>pay_component_appid</c>；扣费主体为服务商时返回）。</summary>
    [JsonPropertyName("pay_component_appid")]
    public string? PayComponentAppId { get; set; }
}
