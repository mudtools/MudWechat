// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

/// <summary>发送设备消息请求体（<c>POST /cgi-bin/message/device/subscribe/send</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>hardware-device/api_sendhardwaredevicemessage.html</c>。</para>
/// <para>
/// 与订阅消息发送（<c>subscribe/send</c>）同构，但接收者为<b>设备订阅用户列表</b>（<c>tousers</c> 数组，官方原文：
/// 接收者（用户）的 openid 列表）；<c>data</c> 键须与设备消息模板关键词一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaDeviceMessageSendRequest
{
    /// <summary>接收者（用户）的 openid 列表（<c>tousers</c>，必填）。</summary>
    [JsonPropertyName("tousers")]
    public List<string>? Tousers { get; set; }

    /// <summary>所需下发的设备消息模板 ID（<c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>点击模板卡片后的跳转页面（<c>page</c>，选填；仅限本小程序内页面）。</summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }

    /// <summary>跳转小程序类型（<c>miniprogram_state</c>，选填）：<c>developer</c> 开发版 / <c>trial</c> 体验版 / <c>formal</c> 正式版（默认）。</summary>
    [JsonPropertyName("miniprogram_state")]
    public string? MiniprogramState { get; set; }

    /// <summary>进入小程序查看的语言（<c>lang</c>，选填）：<c>zh_CN</c> / <c>zh_TW</c> / <c>en-US</c>，默认 <c>zh_CN</c>。</summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }

    /// <summary>模板内容（<c>data</c>，必填；键为模板关键词 ID），见 <see cref="WxaDeviceMessageDataValue"/>。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, WxaDeviceMessageDataValue>? Data { get; set; }
}
