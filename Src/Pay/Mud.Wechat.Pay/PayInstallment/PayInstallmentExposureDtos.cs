// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PayInstallment;

/// <summary>获取微信分付曝光建议请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4016196762"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayInstallment")]
public class PayInstallmentExposureRequest
{
    /// <summary>【商户号】 是由微信支付系统生成并分配给每个商户的唯一标识符，商户号获取方式请参考 普通商户模式开发必要参数说明（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户AppID】 由微信生成的应用ID，全局唯一。是商户在微信 开放平台（移动应用）或 公众平台（公众号/小程序）上申请的一个唯一标识。需确保该appid与mchid有绑定关系，具体请参考 普通商户模式开发必要参数说明。请求基础下单接口时请注意AppID的应用属性，例如公众号场景下，需使用应用属性为公众号的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户ID的类型】 只能填 `OPENID`。可选取值：`UNKNOWN_USER_ID_TYPE` 未知类型 / `OPENID` 普通商户的OpenID / `SP_OPENID` 服务商的OpenID / `SUB_OPENID` 子商户的OpenID（官方字段路径 <c>user_id_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_id_type")]
    public string? UserIdType { get; set; }

    /// <summary>【用户标识】 用户在商户appid下的唯一标识。详见 OpenID获取。若user_id_type为OPENID，则填此字段（官方字段路径 <c>openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>获取微信分付曝光建议响应·【还款信息】 用户可享受的平台优惠信息（例如：该用户有分付12期的立减优惠，则返。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4016196762"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayInstallment")]
public class PayInstallmentExposureResponseRepaymentInfoItem
{
    /// <summary>【还款方式】 还款方式。可选取值：`REPAYMENT_DAILY` 随时还 / `REPAYMENT_INSTALL_3` 分3期 / `REPAYMENT_INSTALL_6` 分6期 / `REPAYMENT_INSTALL_12` 分12期 / `REPAYMENT_INSTALL_24` 分24期（官方字段路径 <c>repayment_info.repayment_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("repayment_type")]
    public string? RepaymentType { get; set; }

    /// <summary>【是否存在平台优惠】 使用微信分付支付时该商户是否有可用的平台优惠（官方字段路径 <c>repayment_info.exist_cash_off_coupon</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("exist_cash_off_coupon")]
    public bool? ExistCashOffCoupon { get; set; }

}

/// <summary>获取微信分付曝光建议响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4016196762"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayInstallment")]
public class PayInstallmentExposureResponse : WechatPayResponse
{
    /// <summary>【是否建议曝光微信分付】 是否建议曝光微信分付（官方字段路径 <c>recommend_exposure</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("recommend_exposure")]
    public bool? RecommendExposure { get; set; }

    /// <summary>【还款信息】 用户可享受的平台优惠信息（例如：该用户有分付12期的立减优惠，则返回12期平台优惠信息），用户是否有优惠由平台的活动决定。该字段仅当用户有可享受的平台优惠时返回（官方字段路径 <c>repayment_info</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("repayment_info")]
    public List<PayInstallmentExposureResponseRepaymentInfoItem>? RepaymentInfo { get; set; }

}
