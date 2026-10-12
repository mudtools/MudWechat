// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 创建卡券 / 礼品码二维码（<c>POST /card/qrcode/create</c>）应答。
/// </summary>
/// <remarks>
/// <para>
/// <b>四字段全建模</b>：<c>ticket</c> / <c>url</c> / <c>show_qrcode_url</c> / <c>expire_seconds</c>。
/// 其中 <c>show_qrcode_url</c> 为「可直接展示的二维码图片地址」——取回该图需自行下载，
/// SDK <b>不做图片通道</b>（对齐小程序码手工通道的既存口径）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeCreateResponse : MpResponse
{
    /// <summary>获取或设置二维码有效秒数（官方 <c>expire_seconds</c>）。</summary>
    [JsonPropertyName("expire_seconds")]
    public long? ExpireSeconds { get; set; }

    /// <summary>获取或设置二维码票据（官方 <c>ticket</c>）。</summary>
    [JsonPropertyName("ticket")]
    public string? Ticket { get; set; }

    /// <summary>获取或设置二维码跳转链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置可直接展示的二维码图片地址（官方 <c>show_qrcode_url</c>）。</summary>
    [JsonPropertyName("show_qrcode_url")]
    public string? ShowQrCodeUrl { get; set; }
}
