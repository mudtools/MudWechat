// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商分期支付「曝光咨询」请求体（微信支付 APIv3，<b>POST /v3/pay/instal/partner/exposure</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016196820"/>（2026-10-14 逐字段核验）。
/// 用途：向微信支付咨询某用户是否推荐曝光「分期支付」入口，以及各分期档期是否有息费减免券。</para>
/// <para><b>⚠️ 与本域下单族的节点名重合陷阱</b>：本请求的 <c>user_id</c> 是<b>对象</b>
/// （<see cref="PartnerInstalUserId"/>），不是下单族 <c>payer</c> 里的 <c>sp_openid</c>/<c>sub_openid</c> 平铺形态；
/// 且 <c>user_id_type</c> 必填、四值枚举见 <see cref="PartnerUserIdTypes"/>。</para>
/// <para><b>频率上限</b>：官方 200 次/秒（服务商商户号维度），超限须降频。</para>
/// <para><b>注意路由前缀</b>：本端点在 <c>/v3/pay/instal/</c> 下，<b>不在</b> <c>/v3/pay/partner/transactions/</c>
/// 前缀内 —— 守卫 PT-B2 按「8 条 transactions 前缀 + 1 条 instal 前缀」钉住该形态。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerInstalExposureRequest
{
    /// <summary>服务商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>服务商 APPID（<c>sp_appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>用户身份（<c>user_id</c>，必填对象），见 <see cref="PartnerInstalUserId"/>。</summary>
    [JsonPropertyName("user_id")]
    public PartnerInstalUserId? UserId { get; set; }
}

/// <summary>分期曝光咨询的用户身份（<c>user_id</c>，必填对象）。</summary>
/// <remarks>
/// <para><c>user_id_type</c> 决定应上送哪个标识：<c>SP_OPENID</c> ⇒ <c>sp_openid</c>、
/// <c>SUB_OPENID</c> ⇒ <c>sub_openid</c>；<c>OPENID</c> 为不区分归属的通用形态，
/// <c>UNKNOWN_USER_ID_TYPE</c> 表示未知（官方枚举原文四值，守卫 PT-B5 锁定）。类型层面无法表达
/// 「按 type 二选一」⇒ 由调用方保证，两标识在 DTO 内皆为选填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerInstalUserId
{
    /// <summary>用户标识类型（<c>user_id_type</c>，必填 string(32)）：取值见 <see cref="PartnerUserIdTypes"/>。</summary>
    [JsonPropertyName("user_id_type")]
    public string? UserIdType { get; set; }

    /// <summary>用户在服务商 APPID 下的标识（<c>sp_openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenId { get; set; }

    /// <summary>用户在子商户 APPID 下的标识（<c>sub_openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }
}

/// <summary>服务商分期支付「曝光咨询」应答。</summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016196820"/>（2026-10-14 核验）。</para>
/// <para><b>字段语义</b>：<c>recommend_exposure</c> 为是否推荐曝光；<c>repayment_info</c> 逐档期给出
/// 是否存在息费减免券。应答<b>不含</b>任何金额或费率 ⇒ 展示逻辑须由调用方另行取得。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerInstalExposureResponse : WechatPayResponse
{
    /// <summary>是否推荐曝光分期支付入口（<c>recommend_exposure</c>，必填 boolean）。</summary>
    [JsonPropertyName("recommend_exposure")]
    public bool? RecommendExposure { get; set; }

    /// <summary>各分期档期信息（<c>repayment_info</c>，数组），见 <see cref="PartnerRepaymentInfo"/>。</summary>
    [JsonPropertyName("repayment_info")]
    public List<PartnerRepaymentInfo>? RepaymentInfo { get; set; }
}

/// <summary>分期档期信息（<c>repayment_info[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerRepaymentInfo
{
    /// <summary>分期档期（<c>repayment_type</c>，string(32)）：取值见 <see cref="PartnerRepaymentTypes"/>。</summary>
    [JsonPropertyName("repayment_type")]
    public string? RepaymentType { get; set; }

    /// <summary>该档期是否存在息费减免券（<c>exist_cash_off_coupon</c>，必填 boolean）。</summary>
    [JsonPropertyName("exist_cash_off_coupon")]
    public bool? ExistCashOffCoupon { get; set; }
}
