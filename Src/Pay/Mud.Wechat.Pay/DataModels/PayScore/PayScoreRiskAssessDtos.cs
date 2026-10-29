// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PayScore;

/// <summary>
/// 评估用户风险等级请求体（<b>POST /v3/payscore/permissions/assess-user-risk-level</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012856611"/>，2026-10-18 逐字段核验；
/// 支持商户：<b>普通商户</b>。</para>
/// <para><b><c>authorization_code</c> 与 <c>openid</c> 至少传一个</b>：官方注明两者均选填，
/// 但须能定位到用户；只传 <c>service_id</c> + <c>appid</c> 无法评估。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreRiskAssessRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：该服务 ID 须有本接口对应产品的权限。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，选填 string(32)）：与 <c>openid</c> 至少传一个。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）：用户在商户 <c>appid</c> 下的唯一标识，与 <c>authorization_code</c> 至少传一个。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>先用后付商品信息（<c>shopping_info</c>，选填），见 <see cref="PayScoreShoppingInfo"/>。</summary>
    [JsonPropertyName("shopping_info")]
    public PayScoreShoppingInfo? ShoppingInfo { get; set; }
}

/// <summary>先用后付商品信息（<c>shopping_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreShoppingInfo
{
    /// <summary>商品列表（<c>goods</c>，<b>必填</b> array），见 <see cref="PayScoreShoppingGoods"/>。</summary>
    [JsonPropertyName("goods")]
    public List<PayScoreShoppingGoods>? Goods { get; set; }
}

/// <summary>
/// 先用后付商品项（<c>shopping_info.goods[]</c>）。
/// </summary>
/// <remarks>官方对 <c>amount</c> 有跨字段校验：<b>所有商品「单价 × 数量」之和不得超过服务风险金</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreShoppingGoods
{
    /// <summary>商品名称（<c>name</c>，必填 string(100)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>商品图片链接（<c>picture</c>，必填 string(255)）。</summary>
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }

    /// <summary>商品单价（<c>amount</c>，必填整型，单位分，<b>≥0</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>商品数量（<c>count</c>，必填整型）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>商品品类 ID（<c>category_id</c>，选填 array&lt;string(16)&gt;）：一个商品最多 3 个品类，当前仅支持整数型品类 ID。</summary>
    [JsonPropertyName("category_id")]
    public List<string>? CategoryId { get; set; }
}

/// <summary>
/// 评估用户风险等级应答。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012856611"/>，2026-10-18 逐字段核验。</para>
/// <para><b><c>user_label</c>/<c>channel_id</c>/<c>user_risk_level</c>/<c>risk_level_version</c>
/// 是风控分层结果</b>：须与「服务额度查询」同一 <c>risk_level_version</c> 口径配合使用，
/// 不得跨版本比较风险分层。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreRiskAssessResponse : WechatPayResponse
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，选填 string(32)）。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>授权状态（<c>authorization_state</c>，必填 string(16)）：取值见 <see cref="PayScoreAuthorizationStates"/>。</summary>
    [JsonPropertyName("authorization_state")]
    public string? AuthorizationState { get; set; }

    /// <summary>授权通知地址（<c>notify_url</c>，选填 string(255)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>最近一次解除授权时间（<c>cancel_authorization_time</c>，选填 string(64)，rfc3339）。</summary>
    [JsonPropertyName("cancel_authorization_time")]
    public string? CancelAuthorizationTime { get; set; }

    /// <summary>最近一次授权成功时间（<c>authorization_success_time</c>，选填 string(64)，rfc3339）。</summary>
    [JsonPropertyName("authorization_success_time")]
    public string? AuthorizationSuccessTime { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string(32)）：仅服务商/平台类接入出现。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户公众账号 ID（<c>sub_appid</c>，选填 string(32)）：仅服务商/平台类接入出现。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户侧用户标识（<c>sub_openid</c>，选填 string(128)）：仅服务商/平台类接入出现。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>用户分层标签（<c>user_label</c>，选填 string(32)）。</summary>
    [JsonPropertyName("user_label")]
    public string? UserLabel { get; set; }

    /// <summary>渠道 ID（<c>channel_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>用户风险分层（<c>user_risk_level</c>，选填整型）：与本服务 <c>risk_level_version</c> 版本下的额度配合使用。</summary>
    [JsonPropertyName("user_risk_level")]
    public int? UserRiskLevel { get; set; }

    /// <summary>风险分层版本号（<c>risk_level_version</c>，选填整型）：与「服务额度查询」入参同源。</summary>
    [JsonPropertyName("risk_level_version")]
    public int? RiskLevelVersion { get; set; }
}

/// <summary>
/// 解除用户授权关系·按 OpenID 请求体
/// （<b>POST /v3/payscore/permissions/openid/{openid}/terminate</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012647413"/>，2026-10-18 逐字段核验；
/// 支持商户：<b>普通商户</b>。</para>
/// <para><b>定位键走 path</b>（<c>openid</c>），请求体只剩 <c>service_id</c> / <c>appid</c> / <c>reason</c>；
/// 官方<b>无应答包体</b>（204）⇒ 接口方法返回 <c>Task</c>。</para>
/// <para><b>与「按授权协议号解除」的请求体不同</b>：后者无 <c>appid</c>（<see cref="PayScoreTerminateAuthorizationRequest"/>），
/// 本支必填 <c>appid</c> —— 两类型不共用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreTerminateByOpenIdRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>撤销原因（<c>reason</c>，必填 string(50)）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
