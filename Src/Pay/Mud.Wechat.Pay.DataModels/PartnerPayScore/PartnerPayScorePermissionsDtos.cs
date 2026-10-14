// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerPayScore;

/// <summary>
/// 商户预授权请求体（<b>POST /v3/payscore/partner/permissions</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012382030"/>，2026-10-14 逐字段核验。
/// <para><b>服务商面 AppID 命名与本线 P0-3 相反</b>：本域用 <c>appid</c>（服务商公众账号）+
/// <c>sub_appid</c>，服务商商户号在<b>应答</b>里叫 <c>mchid</c> —— <b>不是</b>
/// <c>sp_appid</c>/<c>sp_mchid</c>（守卫 PS-B3 锁定），跨域抄字段名即 <c>PARAM_ERROR</c>。</para>
/// <para><c>authorization_code</c> 是<b>商户侧生成的授权协议号</b>，同一服务商商户号下唯一，
/// 多个用户授权<b>不得复用</b>同一协议号。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScorePermissionsRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：支付分产品权限审核通过后由微信支付运营下发。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商公众账号 APPID（<c>appid</c>，必填 string(32)）：须与服务商商户号建立支付绑定关系。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）：须与 <c>sub_mchid</c> 绑定。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，必填 string(32)）：商户侧生成，同一服务商商户号下唯一。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，选填 string(255)）：接收「开启/解除授权服务」回调通知的地址。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// 商户预授权应答：<b>只有</b> <c>apply_permissions_token</c>。
/// </summary>
/// <remarks>
/// <b>时效 1 小时</b>，且是一次性凭据：用于拉起支付分小程序授权页，<b>不得</b>落库长期复用、
/// 也不得下发到端上后再由端上反复调起。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScorePermissionsResponse : WechatPayResponse
{
    /// <summary>预授权 token（<c>apply_permissions_token</c>，必填 string(300)）：时效 <b>1 小时</b>。</summary>
    [JsonPropertyName("apply_permissions_token")]
    public string? ApplyPermissionsToken { get; set; }
}

/// <summary>
/// 场景化预授权请求体（<b>POST /v3/payscore/partner/permissions/apply-for-scene</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012778802"/>，2026-10-14 逐字段核验。
/// <para><b>与 <see cref="PartnerPayScorePermissionsRequest"/> 的差异</b>：本端点多了<b>必填</b>的
/// <c>scene</c>（官方当前只开放 <see cref="PartnerPayScoreScenes.Transaction"/> 一值）与选填的
/// <c>transaction_scene_info</c>，官方注明「<c>scene</c> 为 <c>TRANSACTION</c> 时填写该节」
/// ⇒ 场景为交易时 <c>transaction_scene_info</c> 事实上必填。</para>
/// <para>应答回来的<b>不是</b> <c>apply_permissions_token</c> 而是 <c>transaction_permissions_token</c>
/// （用于预签约支付下单）⇒ 两个 token 字段<b>不共用</b>类型。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreScenePermissionsRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：该服务 ID 须有本接口对应产品的权限。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商公众账号 APPID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，必填 string(32)）：同一商户号下唯一。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>通知地址（<c>notify_url</c>，选填 string(255)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>授权场景（<c>scene</c>，<b>必填</b> string）：取值见 <see cref="PartnerPayScoreScenes"/>，当前官方仅 <c>TRANSACTION</c>。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>交易场景参数（<c>transaction_scene_info</c>，选填 object）：官方原文「<c>scene</c> 为 <c>TRANSACTION</c> 时填写」，见 <see cref="PartnerPayScoreTransactionSceneInfo"/>。</summary>
    [JsonPropertyName("transaction_scene_info")]
    public PartnerPayScoreTransactionSceneInfo? TransactionSceneInfo { get; set; }
}

/// <summary>交易场景预授权附带参数（<c>transaction_scene_info</c>）。</summary>
/// <remarks>两个子字段官方<b>均标必填</b>；本节的 <c>sub_mchid</c> 与外层 <c>sub_mchid</c> 是<b>两处独立字段</b>，
/// 官方要求填「支付下单的子商户号」⇒ 与外层不一致时按外层为授权主体、本节为下单主体理解，不得只填一处。</remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreTransactionSceneInfo
{
    /// <summary>支付下单的商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>支付下单的子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}

/// <summary>
/// 场景化预授权应答：<c>scene</c> 回显 + <c>transaction_permissions_token</c>。
/// </summary>
/// <remarks>官方注明「当请求 <c>scene</c> 为 <c>TRANSACTION</c> 时返回该字段，用于<b>预签约支付下单</b>」
/// ⇒ 与 <see cref="PartnerPayScorePermissionsResponse.ApplyPermissionsToken"/> 是<b>两个不同用途</b>的 token。</remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreScenePermissionsResponse : WechatPayResponse
{
    /// <summary>授权场景（<c>scene</c>，必填 string）：与入参一致，取值见 <see cref="PartnerPayScoreScenes"/>。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>交易中授权 token（<c>transaction_permissions_token</c>，选填 string）：<c>scene=TRANSACTION</c> 时返回。</summary>
    [JsonPropertyName("transaction_permissions_token")]
    public string? TransactionPermissionsToken { get; set; }
}

/// <summary>
/// 服务商评估用户分层请求体（<b>POST /v3/payscore/partner/permissions/assess-user-risk-level</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012783585"/>，2026-10-14 逐字段核验。
/// <para><b>⚠️ 本端点的 <c>sub_mchid</c> 是 query 参数，不在请求体里</b> —— 官方 curl 原文
/// <c>…/assess-user-risk-level?sub_mchid=1230000109</c>。这是「<c>sub_mchid</c> 位置按端点三分」
/// 在本域最容易被抄错的一处（守卫 PS-B1 以参数位置断言钉住）。</para>
/// <para><b>用户标识三选一</b>：<c>authorization_code</c> / <c>openid</c> / <c>sub_openid</c>
/// 官方原文「三选一」，三者皆选填 ⇒ 校验只能由官方在 <c>PARAM_ERROR</c> 上做，SDK 侧不重复实现。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreRiskAssessRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）：该服务 ID 须有本接口对应产品的权限。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商公众账号 APPID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）：上送 <c>sub_openid</c> 时必填。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，选填 string(32)）：与 <c>openid</c>/<c>sub_openid</c> <b>三选一</b>。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>服务商 AppID 下的用户标识（<c>openid</c>，选填 string(128)）：三选一。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>子商户 AppID 下的用户标识（<c>sub_openid</c>，选填 string(128)）：三选一。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>先用后付商品信息（<c>shopping_info</c>，选填），见 <see cref="PartnerPayScoreShoppingInfo"/>。</summary>
    [JsonPropertyName("shopping_info")]
    public PartnerPayScoreShoppingInfo? ShoppingInfo { get; set; }
}

/// <summary>先用后付商品信息（<c>shopping_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreShoppingInfo
{
    /// <summary>商品列表（<c>goods</c>，<b>必填</b> array）。</summary>
    [JsonPropertyName("goods")]
    public List<PartnerPayScoreShoppingGoods>? Goods { get; set; }
}

/// <summary>
/// 先用后付商品项（<c>shopping_info.goods[]</c>）。
/// </summary>
/// <remarks>官方对 <c>amount</c> 有跨字段校验：<b>所有商品「单价 × 数量」之和不得超过服务风险金</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreShoppingGoods
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
/// 用户授权记录应答（<b>两支查询端点共用</b>：按授权协议号查询与按 OpenID 查询，官方两张应答表逐项一致）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692687"/>（授权协议号）、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692798"/>（OpenID），2026-10-14 逐页核验。
/// <para><b><c>authorization_state</c> 的值集在两页不同</b>：按协议号页列 3 值
/// （含 <see cref="PartnerPayScoreAuthorizationStates.UnbindUser"/>），按 OpenID 页只列 2 值
/// ⇒ 本类型按<b>超集</b>承载（守卫 PS-B4 断言两页差异、不得「统一」成一套），
/// 宿主按 OpenID 查询时不要把「未出现 <c>UNBINDUSER</c>」读成「不会出现」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreAuthorizationRecordResponse : WechatPayResponse
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商公众账号 APPID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务商商户号（<c>mchid</c>，必填 string(32)）：官方在本域应答里就叫 <c>mchid</c>，<b>不带</b> <c>sp_</c> 前缀。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>用户在服务商 <c>appid</c> 下的标识（<c>openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，选填 string(32)）。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>授权状态（<c>authorization_state</c>，必填 string(16)）：取值见 <see cref="PartnerPayScoreAuthorizationStates"/>。</summary>
    [JsonPropertyName("authorization_state")]
    public string? AuthorizationState { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(255)）：预授权时传入的回显。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>最近一次解除授权时间（<c>cancel_authorization_time</c>，选填 string(64)，rfc3339）。</summary>
    [JsonPropertyName("cancel_authorization_time")]
    public string? CancelAuthorizationTime { get; set; }

    /// <summary>最近一次授权成功时间（<c>authorization_success_time</c>，选填 string(64)，rfc3339）。</summary>
    [JsonPropertyName("authorization_success_time")]
    public string? AuthorizationSuccessTime { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>用户在子商户 <c>sub_appid</c> 下的标识（<c>sub_openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }
}

/// <summary>
/// 服务商评估用户分层应答：在授权记录字段之上<b>另加四字段</b>（官方两张表不同 ⇒ 分建类型）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012783585"/>，2026-10-14 逐字段核验。
/// <b>与 <see cref="PartnerPayScoreAuthorizationRecordResponse"/> 的差异</b>：多出
/// <c>user_label</c>、<c>channel_id</c>、<c>user_risk_level</c>、<c>risk_level_version</c> 四字段，
/// 且本层风险分要与「服务额度查询」返回的 <c>risk_level</c> 配合使用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreRiskAssessResponse : WechatPayResponse
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商公众账号 APPID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务商商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）：AppID 下的用户标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，选填 string(32)）。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    /// <summary>授权状态（<c>authorization_state</c>，必填 string(16)）：本页官方列 <c>UNAVAILABLE</c>/<c>AVAILABLE</c>/<c>UNBINDUSER</c>。</summary>
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

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户用户标识（<c>sub_openid</c>，选填 string(128)）。</summary>
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
/// 解除用户授权关系·按授权协议号请求体（<b>POST /v3/payscore/partner/permissions/authorization-code/{authorization_code}/terminate</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012382049"/>，2026-10-14 逐字段核验。
/// <b>定位键走 path</b>（<c>authorization_code</c>），请求体只剩这三字段；官方<b>无应答包体</b>（204）。</remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreTerminateByCodeRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>撤销原因（<c>reason</c>，必填 string(50)）：商户侧解除用户支付分授权的原因。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 解除用户授权关系·按 OpenID 请求体（<b>POST /v3/payscore/partner/permissions/terminate</b>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461154"/>，2026-10-14 逐字段核验。
/// <para><b>与按协议号解除是两处独立形态</b>：本端点<b>无 path 参数</b>，用户标识走请求体
/// <c>openid</c>/<c>sub_openid</c> <b>二选一</b>（上送 <c>sub_openid</c> 则 <c>sub_appid</c> 必填），
/// 且 <c>appid</c> 在本端点为必填。</para>
/// <para>官方<b>无应答包体</b>（204）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreTerminateByOpenIdRequest
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>服务商公众账号 APPID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户公众账号 APPID（<c>sub_appid</c>，选填 string(32)）：上送 <c>sub_openid</c> 时<b>必填</b>。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>服务商侧用户标识（<c>openid</c>，选填 string(128)）：与 <c>sub_openid</c> <b>二选一</b>。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>撤销原因（<c>reason</c>，必填 string(50)）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>子商户侧用户标识（<c>sub_openid</c>，选填 string(128)）：与 <c>openid</c> 二选一，传本字段则 <c>sub_appid</c> 必填。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }
}
