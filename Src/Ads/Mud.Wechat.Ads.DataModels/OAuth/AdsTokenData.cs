// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.OAuth;

/// <summary>
/// 授权方信息（官方 <c>data.authorizer_info</c>，struct）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://developers.e.qq.com/v3.0/docs/api/oauth/token"/>（应答字段表，2026-10-10 逐页核验）。</para>
/// <para>
/// <b>只在 <c>grant_type = authorization_code</c> 时返回</b>（官方原文「当 grant_type=refresh_token 时不返回」）
/// ⇒ 全部字段可空；刷新链路不会刷新本信息，<c>AccountId</c> 一旦落库就必须随令牌一起持久化
/// （见 <c>Mud.Wechat.Ads.Abstractions.Auth.AdsAuthorizationState</c>）。
/// </para>
/// <para>
/// <b>官方自相矛盾（照录、不替官方修正）</b>：<c>account_id</c> 在应答字段表标为 <b>integer</b>，
/// 而同一页的应答示例写作 <c>"account_id": "&lt;ACCOUNT_ID&gt;"</c>（占位字符串）。
/// 建模取<b>字段表</b>（integer ⇒ <see cref="long"/>），示例是占位符而非真实值；
/// 若真实应答为字符串，属官方文档缺陷，应由本类改为字符串形态而非放宽判错。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OAuth")]
public class AdsAuthorizerInfo
{
    /// <summary>授权推广帐号对应的 QQ 号（<c>account_uin</c>，integer）。</summary>
    [JsonPropertyName("account_uin")]
    public long? AccountUin { get; set; }

    /// <summary>授权的推广帐号 id，即有操作权限的帐号 id（<c>account_id</c>，integer）。</summary>
    /// <remarks>业务接口的归属主键一律是 <c>account_id</c>（v3.0 <b>不存在</b> <c>advertiser_id</c>）。</remarks>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>权限列表（<c>scope_list</c>，string[]）；<b>为空表示拥有该应用的全部权限</b>（官方原文）。</summary>
    [JsonPropertyName("scope_list")]
    public List<string>? ScopeList { get; set; }

    /// <summary>授权推广帐号对应的微信帐号 id（<c>wechat_account_id</c>，string）。</summary>
    [JsonPropertyName("wechat_account_id")]
    public string? WechatAccountId { get; set; }

    /// <summary>授权账号身份类型（<c>account_role_type</c>，enum；官方应答示例值 <c>ACCOUNT_ROLE_TYPE_AGENCY</c>）。</summary>
    [JsonPropertyName("account_role_type")]
    public string? AccountRoleType { get; set; }

    /// <summary>账号类型（<c>account_type</c>，enum；官方列有「枚举详情」子页，本线不复制枚举值集）。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>角色（<c>role_type</c>，enum；同上）。</summary>
    [JsonPropertyName("role_type")]
    public string? RoleType { get; set; }

    /// <summary>
    /// 掩码描述：<b>只输出身份与权限，不输出任何令牌</b>（本类型本就不含令牌字段，覆写用于统一日志形态）。
    /// </summary>
    public override string ToString()
        => $"AdsAuthorizerInfo(AccountId={AccountId}, AccountUin={AccountUin}, RoleType={AccountRoleType}, Scopes={(ScopeList is null ? 0 : ScopeList.Count)})";
}

/// <summary>
/// 令牌载荷（<c>oauth/token</c> 与 <c>oauth/refresh_token</c> 共同的 <c>data</c> 形态）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://developers.e.qq.com/v3.0/docs/api/oauth/token"/>、
/// <see href="https://developers.e.qq.com/v3.0/docs/api/oauth/refresh_token"/>（2026-10-10 逐页核验）。</para>
/// <para>
/// <b>两支端点的 <c>data</c> 字段集不同</b>（故可空性必须逐字段表达，不能靠一个 DTO 兜底）：
/// </para>
/// <list type="bullet">
/// <item><description><c>oauth/token</c>：<c>authorizer_info</c> + <c>access_token</c> + <c>refresh_token</c> +
/// 两支 <c>*_expires_in</c>；其中 <c>authorizer_info</c> 与 <c>refresh_token</c> 在
/// <c>grant_type = refresh_token</c> 时<b>不返回</b>。</description></item>
/// <item><description><c>oauth/refresh_token</c>：仅 <c>access_token</c> + <c>refresh_token</c> +
/// 两支 <c>*_expires_in</c>（<b>无</b> <c>authorizer_info</c>）。</description></item>
/// </list>
/// <para>
/// <b><c>*_expires_in</c> 是「时长（秒）」而非绝对时刻</b>（官方原文「access_token 过期时间，单位（秒）」，
/// 示例 86400 / 2592000）。SDK 侧必须换算为本地绝对过期时刻再落库
/// （换算点唯一：<c>AdsAuthorizationService</c>），不得把时长当时刻用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OAuth")]
public class AdsTokenData
{
    /// <summary>授权方信息（<c>authorizer_info</c>）；<c>grant_type = refresh_token</c> 及刷新端点均不返回。</summary>
    [JsonPropertyName("authorizer_info")]
    public AdsAuthorizerInfo? AuthorizerInfo { get; set; }

    /// <summary>应用 access token（<c>access_token</c>，string）。<b>不得</b>写入日志 / 遥测 / 异常消息。</summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>应用 refresh token（<c>refresh_token</c>，string）。<b>不得</b>写入日志 / 遥测 / 异常消息。</summary>
    /// <remarks>
    /// <b>一次性凭据</b>：<c>oauth/refresh_token</c> 成功后原 refresh_token 立即失效（官方原文见
    /// <c>IAdsAuthorizationService</c> remarks）⇒ 本字段在 <c>oauth/token(grant_type=refresh_token)</c> 形态下
    /// <b>不返回</b>，此时不得覆盖已持久化的旧值。
    /// </remarks>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>access_token 的<b>有效时长</b>（<c>access_token_expires_in</c>，integer，单位秒；官方示例 86400）。</summary>
    [JsonPropertyName("access_token_expires_in")]
    public long? AccessTokenExpiresIn { get; set; }

    /// <summary>refresh_token 的<b>有效时长</b>（<c>refresh_token_expires_in</c>，integer，单位秒；官方示例 2592000）。</summary>
    [JsonPropertyName("refresh_token_expires_in")]
    public long? RefreshTokenExpiresIn { get; set; }

    /// <summary>掩码描述：只输出时长与是否存在令牌，<b>绝不输出令牌值</b>。</summary>
    public override string ToString()
        => $"AdsTokenData(AccessToken={(string.IsNullOrEmpty(AccessToken) ? "absent" : "present")}, " +
           $"RefreshToken={(string.IsNullOrEmpty(RefreshToken) ? "absent" : "present")}, " +
           $"AccessExpiresIn={AccessTokenExpiresIn}, RefreshExpiresIn={RefreshTokenExpiresIn}, Authorizer={AuthorizerInfo})";
}

/// <summary>
/// <c>oauth/token</c> 与 <c>oauth/refresh_token</c> 的应答（<c>{code, message, message_cn, data}</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OAuth")]
public sealed class AdsTokenResponse : AdsResponse<AdsTokenData>
{
}
