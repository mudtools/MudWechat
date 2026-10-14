// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Repayment;

/// <summary>
/// 「还款下单」请求（<b>POST /v3/repayment/combine-transactions/partner/prepay</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015124463"/>，2026-10-14 逐字段核验。</para>
/// <para><b>合单形态的商户标识体系</b>（第三套，与服务商转账面、银行面互不相通）：
/// <c>combine_out_trade_no</c> + <c>combine_mchid</c>（还款发起方）+ <c>sub_orders[]</c>
/// 内 <c>mchid</c>（官方注明此处<b>填写服务商自身商户号</b>，非直连语义）+ <c>sub_mchid</c> + <c>out_trade_no</c>。</para>
/// <para><b>四个场景对象至少传一个</b>：<c>h5_info</c> / <c>app_info</c> / <c>miniprogram_info</c> /
/// <c>wechat_web_info</c> 官方均标<b>选填</b>，但正文写明「至少需传入一个，支付过程中将检查场景信息是否正确」
/// ⇒ 单个 DTO 属性无法表达该约束，缺场景时由官方以 <c>PARAM_ERROR</c> 拒绝（守卫 RP-B4 锁定四支同名不同形）。</para>
/// <para><b>场景对象各带自己的 <c>combine_appid</c></b>（必填）：与合单支付把 AppID 放顶层的形态不同，
/// 本域 AppID <b>嵌在场景对象内</b> ⇒ 照抄 <c>CombineTransactions</c> 族字段位置会静默丢字段。</para>
/// <para><b>敏感字段加密</b>：<c>combine_payer_info.identity</c> 的 <c>number</c> / <c>name</c> 须加密
/// （<c>Wechatpay-Serial</c> 请求头必填）。</para>
/// <para><b><c>time_expire</c> 仅首次下单可设且不可改</b>（官方原文）；超期须换新单号重新下单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentPrepayRequest
{
    /// <summary>商户还款单号（<c>combine_out_trade_no</c>，必填 string(32)）：同一商户号下唯一。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>还款发起方商户号（<c>combine_mchid</c>，必填 string(32)）：微信支付分配给服务商的唯一标识。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>还款明细单列表（<c>sub_orders</c>，必填 array，<b>最多 10 条</b>）：见 <see cref="RepaymentSubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<RepaymentSubOrder>? SubOrders { get; set; }

    /// <summary>支付结束时间（<c>time_expire</c>，选填 string）：rfc3339；<b>仅首次下单可设，后续不可修改</b>。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>回调通知地址（<c>notify_url</c>，选填 string(255)）：接收还款支付成功通知。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>支付者信息（<c>combine_payer_info</c>，选填 object）：可指定用户实名信息，见 <see cref="RepaymentPayerInfo"/>。</summary>
    [JsonPropertyName("combine_payer_info")]
    public RepaymentPayerInfo? CombinePayerInfo { get; set; }

    /// <summary>H5 场景信息（<c>h5_info</c>，选填 object）：四场景之一，见 <see cref="RepaymentH5Info"/>。</summary>
    [JsonPropertyName("h5_info")]
    public RepaymentH5Info? H5Info { get; set; }

    /// <summary>APP 场景信息（<c>app_info</c>，选填 object）：四场景之一，见 <see cref="RepaymentAppInfo"/>。</summary>
    [JsonPropertyName("app_info")]
    public RepaymentAppInfo? AppInfo { get; set; }

    /// <summary>小程序场景信息（<c>miniprogram_info</c>，选填 object）：四场景之一，见 <see cref="RepaymentMiniprogramInfo"/>。</summary>
    [JsonPropertyName("miniprogram_info")]
    public RepaymentMiniprogramInfo? MiniprogramInfo { get; set; }

    /// <summary>微信内网页场景信息（<c>wechat_web_info</c>，选填 object）：四场景之一，见 <see cref="RepaymentWechatWebInfo"/>。</summary>
    [JsonPropertyName("wechat_web_info")]
    public RepaymentWechatWebInfo? WechatWebInfo { get; set; }
}

/// <summary>
/// 还款明细单（<c>sub_orders[]</c>，下单面）。
/// </summary>
/// <remarks>
/// 官方对 <c>mchid</c> 的原文是「此处请填写服务商（还款发起方）的商户号 <c>combine_mchid</c>」
/// ⇒ 与合单支付里 <c>mchid</c> 指子商户直连号的习惯读法相反，两者须一致。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentSubOrder
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）：官方要求填服务商（还款发起方）商户号，与 <c>combine_mchid</c> 一致。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，<b>下单面必填</b> string(32)）：服务商为子商户进件后获取。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户明细单号（<c>out_trade_no</c>，必填 string(32)）：还款明细单在商户系统内唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填 object）：见 <see cref="RepaymentSubOrderAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public RepaymentSubOrderAmount? Amount { get; set; }

    /// <summary>商户数据包（<c>attach</c>，<b>下单面必填</b> string(128)）：用户不可见，查询与回调原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)）：展示在支付密码确认页、支付凭证与用户账单。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填 object）：见 <see cref="RepaymentSubOrderSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public RepaymentSubOrderSettleInfo? SettleInfo { get; set; }
}

/// <summary>
/// 还款明细单金额（下单面 <c>sub_orders[].amount</c>，官方 2 字段）。
/// </summary>
/// <remarks><b>与查询面的同名字段不是同一张表</b>（查询面 4 字段，多出 <c>payer_amount</c> / <c>payer_currency</c>）⇒ 分建两型。</remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentSubOrderAmount
{
    /// <summary>商品单金额（<c>total_amount</c>，必填 integer）：单位为<b>分</b>。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>货币（<c>currency</c>，必填 string(8)）：ISO 4217 三位字母代码，人民币为 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// 还款明细单结算信息（<c>sub_orders[].settle_info</c>）。
/// </summary>
/// <remarks>官方口径：<c>profit_sharing = true</c> 时资金冻结入子商户不可用余额，须走分账接口或 30 天后自动解冻。</remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentSubOrderSettleInfo
{
    /// <summary>分账标识（<c>profit_sharing</c>，选填 boolean，默认 <c>false</c>）。</summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }

    /// <summary>补差金额（<c>subsidy_amount</c>，选填 integer）：仅 <c>profit_sharing = true</c> 生效，单笔上限 5000 元。</summary>
    [JsonPropertyName("subsidy_amount")]
    public int? SubsidyAmount { get; set; }
}

/// <summary>
/// 支付者信息（<c>combine_payer_info</c>，下单面）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentPayerInfo
{
    /// <summary>用户实名信息（<c>identity</c>，选填 object）：见 <see cref="RepaymentPayerIdentity"/>。</summary>
    [JsonPropertyName("identity")]
    public RepaymentPayerIdentity? Identity { get; set; }
}

/// <summary>
/// 用户实名信息（<c>combine_payer_info.identity</c>）。
/// </summary>
/// <remarks>
/// <b><c>number</c> 与 <c>name</c> 均为密文</b>（string(1024)，官方注明须按敏感信息加密说明处理）
/// ⇒ 与进件域同一套加密面；本 SDK 只承载密文，不做加解密。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentPayerIdentity
{
    /// <summary>证件类型（<c>type</c>，必填 string(32)，7 值）：见 <see cref="RepaymentIdentityTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>证件号（<c>number</c>，必填 string(1024)）：<b>密文</b>。</summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>姓名（<c>name</c>，必填 string(1024)）：<b>密文</b>。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// H5 还款场景信息（<c>h5_info</c>）：四场景对象中字段最多的一支。
/// </summary>
/// <remarks>
/// <b><c>type</c> 本域是「场景类型」而非证件类型</b>（<c>Wap</c>/<c>iOS</c>/<c>Android</c>/<c>Harmony</c>，
/// 见 <see cref="RepaymentH5SceneTypes"/>）⇒ 与 <see cref="RepaymentPayerIdentity.Type"/> 同名不同义，勿共用常量类。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentH5Info
{
    /// <summary>服务商 AppID（<c>combine_appid</c>，必填 string(32)）：须与 <c>combine_mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>用户端实际 IP（<c>payer_client_ip</c>，必填 string(45)）：支持 IPv4 / IPv6。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>场景类型（<c>type</c>，必填 string(32)，4 值）：见 <see cref="RepaymentH5SceneTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>应用名称（<c>app_name</c>，选填 string(64)）。</summary>
    [JsonPropertyName("app_name")]
    public string? AppName { get; set; }

    /// <summary>网站 URL（<c>app_url</c>，必填 string(128)）。</summary>
    [JsonPropertyName("app_url")]
    public string? AppUrl { get; set; }

    /// <summary>iOS 平台 BundleID（<c>bundle_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bundle_id")]
    public string? BundleId { get; set; }

    /// <summary>Android 平台 PackageName（<c>package_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }
}

/// <summary>
/// APP 还款场景信息（<c>app_info</c>）：官方字段表<b>只有 AppID 一节</b>。
/// </summary>
/// <remarks>调起支付所需的端上参数由「调起支付」四支端上形态给出，不属本 HTTP 面（守卫 RP-B2 记录裁决）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentAppInfo
{
    /// <summary>服务商 AppID（<c>combine_appid</c>，必填 string(32)）：须为移动应用类型 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }
}

/// <summary>
/// 小程序还款场景信息（<c>miniprogram_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentMiniprogramInfo
{
    /// <summary>服务商 AppID（<c>combine_appid</c>，必填 string(32)）：须为小程序类型 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>用户 OpenID（<c>openid</c>，必填 string(128)）：在该 <c>combine_appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 微信内网页还款场景信息（<c>wechat_web_info</c>）。
/// </summary>
/// <remarks><c>webview_url</c> 须为发起微信内网页支付的域名，且已在「网络经营场景」中绑定（官方硬约束）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentWechatWebInfo
{
    /// <summary>服务商 AppID（<c>combine_appid</c>，必填 string(32)）：官方此处文案仍写「小程序类型的 APPID」。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>用户 OpenID（<c>openid</c>，必填 string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>网页经营场景域名（<c>webview_url</c>，必填 string(128)）：须在网络经营场景中绑定。</summary>
    [JsonPropertyName("webview_url")]
    public string? WebviewUrl { get; set; }
}

/// <summary>
/// 「还款下单」应答（200 报文，两字段<b>均选填</b>）。
/// </summary>
/// <remarks>
/// <para><b>两字段按场景互斥出现</b>：<c>prepay_id</c> 供 APP / 小程序 / 微信内网页端上调起，
/// <c>h5_url</c> 供 H5 直接跳转 ⇒ 两者皆选填，读到一个即够，不得断言恒有 <c>prepay_id</c>。</para>
/// <para><b>字段名是 <c>h5_url</c></b>（含数字）⇒ 本域是支付线里少数名字带数字的字段，
/// C# 属性名写作 <c>H5Url</c>、序列化名单引号钉住（守卫 RP-B3 断言照抄原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentPrepayResponse : WechatPayResponse
{
    /// <summary>预支付交易会话标识（<c>prepay_id</c>，选填 string(64)）：端上调起支付用。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>H5 支付跳转链接（<c>h5_url</c>，选填 string(512)）：仅 H5 场景返回。</summary>
    [JsonPropertyName("h5_url")]
    public string? H5Url { get; set; }
}
