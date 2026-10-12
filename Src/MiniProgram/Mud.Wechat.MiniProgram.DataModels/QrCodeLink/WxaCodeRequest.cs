// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>获取<b>限量</b>小程序码请求体（<c>POST /wxa/getwxacode</c>）。</summary>
/// <remarks>
/// <para><b>官方文档</b>：<c>qrcode-link/qr-code/api_getqrcode.html</c>。</para>
/// <para>
/// <b>总量限制（官方原文）</b>：本类接口生成的码<b>总数上限 10 万个</b>（与不限量码的本质差异）；
/// <c>path</c> 须为已发布页面、<b>不得带参数</b>（带参数请改用不限量码的 <c>scene</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaCodeRequest
{
    /// <summary>扫码进入的页面路径（<c>path</c>，必填；<b>必须</b>为已发布页面、不得带 query、不带前导 <c>/</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>二维码边长（<c>width</c>，选填，单位 px；官方要求 280~1280）。</summary>
    [JsonPropertyName("width")]
    public long? Width { get; set; }

    /// <summary>是否自动配置线条颜色（<c>auto_color</c>，选填）。</summary>
    [JsonPropertyName("auto_color")]
    public bool? AutoColor { get; set; }

    /// <summary>线条颜色（<c>line_color</c>，选填），见 <see cref="WxaRgbColor"/>。</summary>
    [JsonPropertyName("line_color")]
    public WxaRgbColor? LineColor { get; set; }

    /// <summary>是否需要透明底色（<c>is_hyaline</c>，选填）。</summary>
    [JsonPropertyName("is_hyaline")]
    public bool? IsHyaline { get; set; }
}
