// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Security;

/// <summary>获取用户安全等级请求体（<c>POST /wxa/getuserriskrank</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>sec-center/safety-control-capability/api_getuserriskrank.html</c>。</para>
/// <para>
/// <b>能力口径（官方原文）</b>：根据提交的用户信息获取安全等级，<b>无需用户授权</b>；
/// <see cref="MobileNo"/> 如需加密处理可传加密后的密文；<see cref="ExtendedInfo"/> 须为标准 JSON
/// 序列化字符串（如设备型号、客户端版本等，官方上限 ≤2K）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaGetUserRiskRankRequest
{
    /// <summary>用户小程序 AppID（<c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>用户唯一标识（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>场景值（<c>scene</c>，必填）；<c>2</c> 为营销活动，其余场景值需联系微信团队开通。</summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }

    /// <summary>用户手机号（<c>mobile_no</c>，选填；如需加密处理请传加密后的密文）。</summary>
    [JsonPropertyName("mobile_no")]
    public string? MobileNo { get; set; }

    /// <summary>用户访问源 IP（<c>client_ip</c>，选填）。</summary>
    [JsonPropertyName("client_ip")]
    public string? ClientIp { get; set; }

    /// <summary>用户邮箱（<c>email_address</c>，选填）。</summary>
    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    /// <summary>额外补充信息（<c>extended_info</c>，选填；标准 JSON 序列化字符串 ≤2K）。</summary>
    [JsonPropertyName("extended_info")]
    public string? ExtendedInfo { get; set; }
}
