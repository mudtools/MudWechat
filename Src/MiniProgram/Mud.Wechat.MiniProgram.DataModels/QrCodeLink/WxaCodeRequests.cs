// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>
/// 获取<b>不限量</b>小程序码请求体（<c>POST /wxa/getwxacodeunlimit</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<c>qrcode-link/qr-code/api_getunlimitedqrcode.html</c>（2026-10-09 逐字段核验）。</para>
/// <para>
/// <b>与「限量小程序码」的本质差异</b>：本端点数<b>不受 10 万个限制</b>（`scene` 携带业务参数，
/// 但<b>总长度上限 32 可见字符</b>），适合「一物一码 / 一人一码」场景；
/// 限量码（<see cref="WxaCodeRequest"/>）以 <c>path</c> 直接表达、总量受限。
/// </para>
/// <para><b>应答是图片二进制流</b>（Content-Type 为 <c>image/*</c>）；<b>出错时</b>才是 JSON ——
/// 故本端点由 <c>IWxaCodeService</c> 承载（Content-Type 分支判错），不进 JSON 反序列化管线。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaCodeUnlimitRequest
{
    /// <summary>场景值（<c>scene</c>，必填；<b>最大 32 个可见字符</b>，支持数字与 <c>!#$&amp;'()*+,/:;=?@-._~</c> 等字符）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>扫码进入的页面路径（<c>page</c>，选填；须为已发布页面，<b>不得带前导 <c>/</c></b>、不带 <c>.html</c>）。</summary>
    [JsonPropertyName("page")]
    public string? Page { get; set; }

    /// <summary>
    /// 是否校验 <c>page</c> 是否存在（<c>check_path</c>，选填；默认 <c>true</c>）。
    /// </summary>
    /// <remarks>开发期可置 <c>false</c> 放行未发布页面；<b>正式环境应保持默认</b>。</remarks>
    [JsonPropertyName("check_path")]
    public bool? CheckPath { get; set; }

    /// <summary>小程序版本（<c>env_version</c>，选填）：<c>release</c> / <c>trial</c> / <c>develop</c>。</summary>
    [JsonPropertyName("env_version")]
    public string? EnvVersion { get; set; }

    /// <summary>二维码边长（<c>width</c>，选填，单位 px；官方要求 280~1280）。</summary>
    [JsonPropertyName("width")]
    public long? Width { get; set; }

    /// <summary>是否自动配置线条颜色（<c>auto_color</c>，选填；<c>false</c> 时用 <c>line_color</c>）。</summary>
    [JsonPropertyName("auto_color")]
    public bool? AutoColor { get; set; }

    /// <summary>线条颜色（<c>line_color</c>，选填；<c>{"r":…,"g":…,"b":…}），见 <see cref="WxaRgbColor"/>。</summary>
    [JsonPropertyName("line_color")]
    public WxaRgbColor? LineColor { get; set; }

    /// <summary>是否需要透明底色（<c>is_hyaline</c>，选填；<c>true</c> 时输出透明 PNG）。</summary>
    [JsonPropertyName("is_hyaline")]
    public bool? IsHyaline { get; set; }
}

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

/// <summary>线条颜色（<c>line_color</c>，RGB 三元组）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaRgbColor
{
    /// <summary>红（<c>r</c>，0-255）。</summary>
    [JsonPropertyName("r")]
    public long? R { get; set; }

    /// <summary>绿（<c>g</c>，0-255）。</summary>
    [JsonPropertyName("g")]
    public long? G { get; set; }

    /// <summary>蓝（<c>b</c>，0-255）。</summary>
    [JsonPropertyName("b")]
    public long? B { get; set; }
}
