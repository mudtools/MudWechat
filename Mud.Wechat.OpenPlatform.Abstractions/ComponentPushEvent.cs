// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>
/// 「授权事件接收 URL」推送的 <c>InfoType</c> 取值（官方原文）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源（2026-10-09 逐字核验）</b>：第三方平台的<b>同一个</b>「授权事件接收 URL」
/// 会收到两类推送 ——
/// ① 票据：<see cref="ComponentVerifyTicket"/>（创建审核通过后每 10 分钟一次）；
/// ② 授权变更：<see cref="Authorized"/> / <see cref="UpdateAuthorized"/> / <see cref="Unauthorized"/>
/// （用户授权、取消授权、更新授权后触发）。
/// 见《验证票据》与《授权变更通知推送》两页。
/// </para>
/// <para>
/// <b>⚠️ 取值全小写</b>：与本仓支付线的支付分/发票 <c>event_type</c>（<b>全大写</b>）风格相反 ——
/// 不同产品线的取值风格确实不统一，<b>不得</b>按「微信都用大写」类推。
/// </para>
/// </remarks>
public static class ComponentPushInfoTypes
{
    /// <summary>验证票据（官方 <c>component_verify_ticket</c>）。</summary>
    public const string ComponentVerifyTicket = "component_verify_ticket";

    /// <summary>授权成功（官方 <c>authorized</c>）。</summary>
    public const string Authorized = "authorized";

    /// <summary>更新授权（官方 <c>updateauthorized</c>）。</summary>
    /// <remarks>官方补充：授权更新时<b>若权限集没有变化，将不会触发</b>本通知。</remarks>
    public const string UpdateAuthorized = "updateauthorized";

    /// <summary>取消授权（官方 <c>unauthorized</c>）。</summary>
    public const string Unauthorized = "unauthorized";
}

/// <summary>推送 XML 的元素名（官方原文，<b>含大小写</b>）。</summary>
/// <remarks>
/// <b>照录、勿「规范」为驼峰/snake</b>：开放平台推送用的是 <c>AuthorizerAppid</c>（小写 d）、
/// <c>InfoType</c>（驼峰）这类混合写法，与 JSON 接口的 <c>authorizer_appid</c> <b>不是同一批拼写</b>。
/// </remarks>
public static class ComponentPushXmlElements
{
    /// <summary>密文元素（<c>Encrypt</c>，官方常以 CDATA 包裹）。</summary>
    public const string Encrypt = "Encrypt";

    /// <summary>第三方平台 appid（<c>AppId</c>）。</summary>
    public const string AppId = "AppId";

    /// <summary>时间戳（<c>CreateTime</c>）。</summary>
    public const string CreateTime = "CreateTime";

    /// <summary>通知类型（<c>InfoType</c>）。</summary>
    public const string InfoType = "InfoType";

    /// <summary>授权方 appid（<c>AuthorizerAppid</c>）。</summary>
    public const string AuthorizerAppId = "AuthorizerAppid";

    /// <summary>授权码（<c>AuthorizationCode</c>，可用于换取授权信息）。</summary>
    public const string AuthorizationCode = "AuthorizationCode";

    /// <summary>授权码过期时间（<c>AuthorizationCodeExpiredTime</c>）。</summary>
    public const string AuthorizationCodeExpiredTime = "AuthorizationCodeExpiredTime";

    /// <summary>预授权码（<c>PreAuthCode</c>）。</summary>
    public const string PreAuthCode = "PreAuthCode";

    /// <summary>票据（<c>ComponentVerifyTicket</c>）。</summary>
    public const string ComponentVerifyTicket = "ComponentVerifyTicket";
}

/// <summary>
/// 授权变更事件（<c>authorized</c> / <c>updateauthorized</c> / <c>unauthorized</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段按事件类型分布（官方原文）</b>：授权成功与更新授权含
/// <c>AuthorizerAppid</c> + <c>AuthorizationCode</c> + <c>AuthorizationCodeExpiredTime</c> + <c>PreAuthCode</c>；
/// <b>取消授权只有</b> <c>AuthorizerAppid</c> ⇒ 本类字段全部可空，消费侧按
/// <see cref="InfoType"/> 判断该读哪些。
/// </para>
/// <para>
/// <b>⚠️ <see cref="AuthorizationCodeExpiredTime"/> 只做原样透传</b>：官方对该字段的说明是
/// 「授权码过期时间，单位秒」，但<b>没有</b>说明它是「剩余秒数」还是「绝对时间戳」⇒
/// 本仓<b>刻意不做</b>时间换算（把绝对时间戳当剩余秒数会产生「刚拿到就以为过期」或反之的错误），
/// 由调用方结合实测语义处理。
/// </para>
/// <para>
/// <b>授权码是有时效的凭据</b>：官方以本事件把 <c>AuthorizationCode</c> 推给平台，
/// 平台应立即调用 <c>api_query_auth</c> 换取 <c>authorizer_access_token</c> 与
/// <c>authorizer_refresh_token</c>（后者须持久化）。<b>本类不持有任何令牌</b>。
/// </para>
/// </remarks>
public sealed class ComponentAuthorizerEvent
{
    /// <summary>创建事件。</summary>
    /// <param name="infoType">事件类型（<c>InfoType</c>）。</param>
    /// <param name="authorizerAppId">授权方 appid。</param>
    /// <param name="authorizationCode">授权码（取消授权时为 <c>null</c>）。</param>
    /// <param name="authorizationCodeExpiredTime">授权码过期时间原文（<b>不做换算</b>）。</param>
    /// <param name="preAuthCode">预授权码。</param>
    /// <param name="createTime">推送时间戳原文。</param>
    public ComponentAuthorizerEvent(
        string? infoType,
        string? authorizerAppId,
        string? authorizationCode,
        string? authorizationCodeExpiredTime,
        string? preAuthCode,
        string? createTime)
    {
        InfoType = infoType;
        AuthorizerAppId = authorizerAppId;
        AuthorizationCode = authorizationCode;
        AuthorizationCodeExpiredTime = authorizationCodeExpiredTime;
        PreAuthCode = preAuthCode;
        CreateTime = createTime;
    }

    /// <summary>事件类型（官方 <c>InfoType</c>）：见 <see cref="ComponentPushInfoTypes"/>。</summary>
    public string? InfoType { get; }

    /// <summary>授权方 appid（官方 <c>AuthorizerAppid</c>）。</summary>
    public string? AuthorizerAppId { get; }

    /// <summary>授权码（官方 <c>AuthorizationCode</c>）：可立即用于换取授权信息。</summary>
    public string? AuthorizationCode { get; }

    /// <summary>授权码过期时间（官方 <c>AuthorizationCodeExpiredTime</c>，原样透传，见类注释）。</summary>
    public string? AuthorizationCodeExpiredTime { get; }

    /// <summary>预授权码（官方 <c>PreAuthCode</c>）。</summary>
    public string? PreAuthCode { get; }

    /// <summary>推送时间戳（官方 <c>CreateTime</c>）。</summary>
    public string? CreateTime { get; }

    /// <summary>是否为取消授权事件（此时<b>不应</b>再尝试换取令牌）。</summary>
    public bool IsUnauthorized
        => string.Equals(InfoType, ComponentPushInfoTypes.Unauthorized, StringComparison.Ordinal);
}
