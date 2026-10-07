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

/// <summary>检验用户授权凭证（<c>sns/auth</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：有效凭证 <c>{"errcode":0,"errmsg":"ok"}</c>；
/// 无效凭证 <c>{"errcode":40003,"errmsg":"invalid openid"}</c> ⇒ 调用方判 <see cref="MpResponse.IsSuccess"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Sns")]
public class MpSnsAuthResponse : MpResponse
{
}

/// <summary>
/// 获取授权用户信息（<c>sns/userinfo</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>需 snsapi_userinfo 作用域</b>且用户手动同意（否则 48001）；
/// 无须关注公众号即可获取基本信息。
/// </para>
/// <para>
/// <b>与 /cgi-bin/user/info 的字段集分别核验（勿照抄）</b>：本接口<b>仍提供</b>
/// nickname / sex / province / city / country / headimgurl 等完整资料字段
/// （前提：用户经 snsapi_userinfo 授权同意）；而基础信息接口自 2021-12-27 起
/// 已停供头像昵称（见 <see cref="User.MpUserInfo"/> remarks）。两页字段集<b>不同且各自权威</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Sns")]
public class MpSnsUserInfoResponse : MpResponse
{
    /// <summary>获取或设置用户的唯一标识（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户昵称（官方 <c>nickname</c>；本接口在有效授权下仍返回——与基础信息接口不同）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置性别（官方 <c>sex</c>：1 男 / 2 女 / 0 未知）。</summary>
    [JsonPropertyName("sex")]
    public int? Sex { get; set; }

    /// <summary>获取或设置用户个人资料填写的省份（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置普通用户个人资料填写的城市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置国家（官方 <c>country</c>，如中国为 CN）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>获取或设置用户头像（官方 <c>headimgurl</c>；末尾数值代表正方形大小 0/46/64/96/132；「若用户更换头像，原有头像 URL 将失效」）。</summary>
    [JsonPropertyName("headimgurl")]
    public string? HeadImgUrl { get; set; }

    /// <summary>获取或设置用户特权信息（官方 <c>privilege</c>，json 数组，如微信沃卡用户为 chinaunicom）。</summary>
    [JsonPropertyName("privilege")]
    public List<string>? Privilege { get; set; }

    /// <summary>获取或设置用户统一标识（官方 <c>unionid</c>；只有在公众号绑定到微信开放平台账号后才会出现）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
}
