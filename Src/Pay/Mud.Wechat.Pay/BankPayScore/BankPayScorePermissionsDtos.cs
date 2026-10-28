// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.BankPayScore;

/// <summary>
/// 从业机构预授权请求体（<b>POST /v3/payscore/acquiringbank/permissions</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691489"/>，2026-10-14 逐字段核验。
/// <para><b>比服务商面（P0-4）多一层 <c>channel_id</c></b>：从业机构 → 渠道商 → 子商户三级，
/// <c>channel_id</c>（渠道商商户号）在本域<b>十一端点处处必填</b>，而服务商支付分域完全没有该字段
/// ⇒ 两域请求类型不可互换（守卫 BS-B3 锁定）。</para>
/// <para><c>authorization_code</c> 是<b>从业机构侧自生成</b>的授权协议号（32 字符内、仅数字大小写字母 <c>_-|*</c>），
/// 同一从业机构商户号下唯一，多个用户授权<b>不得复用</b>。</para>
/// <para>本域<b>没有</b>服务商面的「场景化预授权」「评估用户分层」「按 OpenID 查授权」「按 OpenID 解除授权」四支
/// （守卫 BS-B2 以零声明断言钉住，正对照 = 服务商面四支全在）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreApplyPermissionsRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：支付分权限审核通过后由微信支付运营下发。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）：从业机构下的渠道商，须在服务商平台完成入驻。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>从业机构公众账号 APPID（<c>appid</c>，必填 string(32)）：须与从业机构商户号在服务商平台绑定。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）：由从业机构或渠道商走「开户意愿确认」生成。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，必填 string(32)）：从业机构侧生成，同一商户号下唯一。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，选填 string(255)）：接收「开启/解除授权服务」回调通知的地址。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// 从业机构预授权应答：<b>只有</b> <c>apply_permissions_token</c>。
/// </summary>
/// <remarks>
/// <b>时效 1 小时</b>的一次性凭据，用于拉起支付分小程序授权页；<b>不得</b>长期落库复用，
/// 也不得下发到端上后由端上反复调起。形态与服务商面同名单字段，但两域<b>各持类型</b>（跨域不共用 DTO）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreApplyPermissionsResponse : WechatPayResponse
{
    /// <summary>预授权 token（<c>apply_permissions_token</c>，必填 string(300)）：时效 <b>1 小时</b>。</summary>
    [JsonPropertyName("apply_permissions_token")]
    public string? ApplyPermissionsToken { get; set; }
}

/// <summary>
/// 解除用户授权关系请求体（<b>POST /v3/payscore/acquiringbank/permissions/authorization-code/{authorization_code}/terminate</b>，成功 <b>204 无包体</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461012"/>，2026-10-14 逐字段核验。
/// <c>reason</c> 必填且 ≤50 字符；<c>sub_mchid</c> 在本端点位于 <b>body</b>（同域两支 GET 才走 query）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreTerminatePermissionsRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>撤销原因（<c>reason</c>，必填 string(50)）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 查询用户授权记录应答（<b>GET …/permissions/authorization-code/{authorization_code}</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461004"/>，2026-10-14 逐字段核验。
/// <para><b><c>channel_id</c> 在本应答降为选填</b>（请求 query 为必填）：本域必填性随页面/位置而变
/// （守卫 BS-B1 逐点锁定），勿按「请求必填则应答也必填」推断。</para>
/// <para>两个时间为 rfc3339 字符串（官方原文把它写作「Rfc399」，是其自身拼写笔误）⇒ 保持 <c>string</c>。</para>
/// <para><c>user_label</c> 仅授权成功时返回且<b>不保证一定返回</b>，官方值集为 <c>A</c>/<c>B</c>/<c>C</c> 三值
/// —— 官方原文的比较说明「（其中A&gt;B&gt;」在页面里<b>即已截断</b>，故本仓只录值集、不转录排序语义。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreAuthorizationRecordResponse : WechatPayResponse
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>从业机构公众账号 APPID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>从业机构商户号（<c>mchid</c>，必填 string(32)）：与 <c>appid</c> 有绑定关系。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）：从业机构 <c>appid</c> 维度。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，选填 string(32)）。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>授权状态（<c>authorization_state</c>，必填 string(16)）：取值见 <see cref="BankPayScoreAuthorizationStates"/>。</summary>
    [JsonPropertyName("authorization_state")]
    public string? AuthorizationState { get; set; }

    /// <summary>授权通知地址（<c>notify_url</c>，选填 string(255)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>最近一次解除授权时间（<c>cancel_authorization_time</c>，选填 string(64)）：rfc3339。</summary>
    [JsonPropertyName("cancel_authorization_time")]
    public string? CancelAuthorizationTime { get; set; }

    /// <summary>最近一次授权成功时间（<c>authorization_success_time</c>，选填 string(64)）：rfc3339。</summary>
    [JsonPropertyName("authorization_success_time")]
    public string? AuthorizationSuccessTime { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>用户标识（<c>sub_openid</c>，选填 string(128)）：子商户 <c>sub_appid</c> 维度。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>用户标签（<c>user_label</c>，选填 string(32)）：仅授权成功时可能返回，取值见 <see cref="BankPayScoreUserLabels"/>。</summary>
    [JsonPropertyName("user_label")]
    public string? UserLabel { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，<b>选填</b> string(32)）：请求 query 同名字段为必填，本应答为选填。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }
}
