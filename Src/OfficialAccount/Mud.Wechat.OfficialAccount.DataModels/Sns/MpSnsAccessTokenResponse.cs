// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Sns;

/// <summary>
/// 换取用户授权凭证（<c>sns/oauth2/access_token</c>）响应（<b>用户级</b>授权 token）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：成功响应含 access_token / expires_in / refresh_token / openid /
/// unionid（仅 snsapi_userinfo 作用域返回）/ is_snapshotuser（仅快照页模式虚拟账号返回，值为 1）；
/// <b>响应表无 scope 字段</b>（与 refresh_token 接口的响应表不同——逐页核验确认，勿互相「补全」）。
/// </para>
/// <para>
/// 官方原文：「此 access_token 与基础支持的 access_token 不同」——网页授权凭证为<b>用户级</b>
/// （每 openid 一份），SDK <b>不建模缓存管理器</b>（I5 裁决：per-(app,openid) 键空间 +
/// refresh_token 轮换回写语义过重；refresh_token 有效期 <b>30 天</b>，生命周期归宿主）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Sns")]
public class MpSnsAccessTokenResponse : MpResponse
{
    /// <summary>获取或设置网页授权接口调用凭证（官方 <c>access_token</c>；与基础支持的 access_token 不同，用户级）。</summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>获取或设置凭证有效期（官方 <c>expires_in</c>，单位秒；官方示例 7200）。</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>获取或设置用户刷新 access_token 的凭证（官方 <c>refresh_token</c>；有效期 30 天）。</summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>获取或设置用户唯一标识（官方 <c>openid</c>；未关注用户访问网页也会产生）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户统一标识（官方 <c>unionid</c>；只有 scope 为 snsapi_userinfo 时返回）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置是否为快照页模式虚拟账号（官方 <c>is_snapshotuser</c>；仅快照页虚拟账号返回，值为 1）。</summary>
    [JsonPropertyName("is_snapshotuser")]
    public int? IsSnapshotUser { get; set; }
}
