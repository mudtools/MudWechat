// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Sns;

/// <summary>刷新用户授权凭证（<c>sns/oauth2/refresh_token</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：响应含 scope 字段（用户授权的作用域，逗号分隔）——
/// 与 sns/oauth2/access_token 的响应表不同（那边无 scope），照各自页面原文建模。
/// refresh_token 轮换回写归宿主（I5 裁决）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Sns")]
public class MpSnsRefreshTokenResponse : MpResponse
{
    /// <summary>获取或设置网页授权接口调用凭证（官方 <c>access_token</c>；用户级）。</summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>获取或设置凭证有效期（官方 <c>expires_in</c>，单位秒）。</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>获取或设置用户刷新 access_token 的凭证（官方 <c>refresh_token</c>；刷新后轮换，归宿主回写）。</summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>获取或设置用户唯一标识（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户授权的作用域（官方 <c>scope</c>，使用逗号分隔）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}
