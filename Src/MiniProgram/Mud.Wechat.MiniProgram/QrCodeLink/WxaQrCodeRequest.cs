// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>创建<b>二维码 C</b>请求体（<c>POST /cgi-bin/wxaapp/createwxaqrcode</c>）。</summary>
/// <remarks>
/// <para><b>官方文档</b>：<c>qrcode-link/qr-code/api_createqrcode.html</c>。</para>
/// <para>
/// <b>与另两种码的差异（官方原文）</b>：本端点生成的是<b>二维码（QR Code）</b>而非小程序码，
/// 样式不可定制（无 <c>auto_color</c> / <c>line_color</c> / <c>is_hyaline</c>），
/// 且同样受<b>总量 10 万个</b>限制。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQrCodeRequest
{
    /// <summary>扫码进入的页面路径（<c>path</c>，必填；已发布页面、不得带 query）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>二维码边长（<c>width</c>，选填，单位 px；官方要求 280~1280）。</summary>
    [JsonPropertyName("width")]
    public long? Width { get; set; }
}
