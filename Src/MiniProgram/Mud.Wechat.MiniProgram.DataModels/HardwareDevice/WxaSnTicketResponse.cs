// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

/// <summary>获取设备票据应答（<c>POST /wxa/getsnticket</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>hardware-device/api_getsnticket.html</c>。</para>
/// <para>票据（<see cref="SnTicket"/>）用于生成设备码，<b>5 分钟内有效</b>（官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaSnTicketResponse : WxaResponse
{
    /// <summary>设备票据（<c>sn_ticket</c>）。</summary>
    [JsonPropertyName("sn_ticket")]
    public string? SnTicket { get; set; }

    /// <summary>有效期（<c>expires_in</c>，单位秒）。</summary>
    [JsonPropertyName("expires_in")]
    public long? ExpiresIn { get; set; }
}
