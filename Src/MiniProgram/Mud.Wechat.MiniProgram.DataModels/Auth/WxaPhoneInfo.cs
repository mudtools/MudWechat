// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>手机号信息（<c>phone_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaPhoneInfo
{
    /// <summary>手机号（<c>phoneNumber</c>，<b>带区号</b>，如 <c>+86 13800000000</c>）。</summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    /// <summary>不含区号的手机号（<c>purePhoneNumber</c>）。</summary>
    [JsonPropertyName("purePhoneNumber")]
    public string? PurePhoneNumber { get; set; }

    /// <summary>区号（<c>countryCode</c>）。</summary>
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    /// <summary>数据水印（<c>watermark</c>，官方要求校验），见 <see cref="WxaWatermark"/>。</summary>
    [JsonPropertyName("watermark")]
    public WxaWatermark? Watermark { get; set; }
}
