// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Sns;

/// <summary>
/// 「第三方平台代公众号网页授权」用户级令牌响应（官方 <c>sns/oauth2/component/access_token</c> 与
/// <c>sns/oauth2/component/refresh_token</c> 共用形态；GET、无请求体）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与公众号线 <c>/sns/oauth2/access_token</c> 的关系</b>：同前缀不同段——本端点是
/// <b>第三方平台</b>代网页授权（令牌参数为 <c>component_access_token</c>，且带 <c>component_appid</c>）；
/// 公众号线为普通网页授权（令牌参数为 <c>secret</c>）。守卫锁定两线路由不交叠。
/// </para>
/// <para>
/// <b>本响应的 access_token 是用户级令牌</b>（不是应用级 / 授权方 / 平台令牌），由<b>调用方</b>持久化与轮换
/// （refresh_token 30 天有效，失效后需用户重新授权）——SDK 不代管。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Sns")]
public class OpenPlatformSnsComponentTokenResponse : OpenPlatformResponse
{
    /// <summary>获取或设置网页授权的用户级接口调用凭证（官方 <c>access_token</c>；<b>敏感字段勿落日志</b>）。</summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>获取或设置凭证有效期（官方 <c>expires_in</c>，秒）。</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>获取或设置刷新凭证（官方 <c>refresh_token</c>；30 天有效，<b>敏感字段勿落日志</b>）。</summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>获取或设置授权用户唯一标识（官方 <c>openid</c>；本 <c>appid</c> 下唯一）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户授权的作用域（官方 <c>scope</c>；逗号分隔，如 <c>snsapi_userinfo</c>）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>获取或设置用户在开放平台的唯一标识（官方 <c>unionid</c>；仅用户绑定到微信开放平台账号后返回，可选）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
}
