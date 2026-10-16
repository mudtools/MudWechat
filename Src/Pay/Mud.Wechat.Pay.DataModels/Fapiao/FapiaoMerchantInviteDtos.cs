// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Fapiao;

/// <summary>获取服务商电子发票能力邀请链接响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941495"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoSpInviteUrlResponse : WechatPayResponse
{
    /// <summary>【邀请开通二维码链接】 用于生成邀请二维码，商户使用微信扫描二维码进行授权开通电子发票能力（官方字段路径 <c>invite_url</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("invite_url")]
    public string? InviteUrl { get; set; }

    /// <summary>【开通小程序AppID】 商户授权的小程序 AppID。与miniprogram_path 共同作为小程序跳转插件的参数，用于商户点击跳转授权小程序，开通服务商电子发票发票能力。参考： wx.navigateToMiniProgram（官方字段路径 <c>miniprogram_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("miniprogram_appid")]
    public string? MiniprogramAppid { get; set; }

    /// <summary>【开通小程序跳转路径】 商户开通服务商电子发票小程序的页面路径，与 miniprogram_appid 共同作为小程序跳转插件的参数，用于商户点击跳转授权小程序，开通服务商电子发票发票能力。参考： wx.navigateToMiniProgram（官方字段路径 <c>miniprogram_path</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("miniprogram_path")]
    public string? MiniprogramPath { get; set; }

}

/// <summary>获取邀请开通的商户信息响应·【相关链接】 相关链接。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941524"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoSpInviteMerchantListResponseLinks
{
    /// <summary>【下一页链接】 使用同样的limit进行下一页查询时的请求链接。当前为最后一页时，返回为空（官方字段路径 <c>links.next</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }

    /// <summary>【上一页链接】 使用同样的limit进行上一页查询时的请求链接。如果为第一页时，返回为空（官方字段路径 <c>links.prev</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    /// <summary>【当前链接】 当前的请求链接（官方字段路径 <c>links.self</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }

}

/// <summary>获取邀请开通的商户信息响应·【邀请的商户结果】 服务商邀请的商户结果列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941524"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoSpInviteMerchantListResponseMchInviteResultListItem
{
    /// <summary>【邀请的商户商户号】 服务商邀请的商户号，当邀请状态为MCH_INVITE_SUCC返回（官方字段路径 <c>mch_invite_result_list.sub_mchid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【邀请商户的状态】 邀请商户的具体状态。可选取值：`MCH_INVITE_SUCC` 商户已接受邀请，通过 [【检查子商户开票功能状态】](https://pay.weixin.qq.com/doc/v3/partner/4015792561.md) 接口查询具体状态 / `MCH_INVITE_FAILED` 商户邀请开通失败，具体失败原因查看invite_failed_reason字段（官方字段路径 <c>mch_invite_result_list.mch_invite_status</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mch_invite_status")]
    public string? MchInviteStatus { get; set; }

    /// <summary>【商户名称】 商户名称（官方字段路径 <c>mch_invite_result_list.ep_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("ep_name")]
    public string? EpName { get; set; }

    /// <summary>【商户税号】 商户税号（官方字段路径 <c>mch_invite_result_list.tax_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("tax_id")]
    public string? TaxId { get; set; }

    /// <summary>【服务商邀请商户code】 邀请链接唯一标识（官方字段路径 <c>mch_invite_result_list.invite_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("invite_code")]
    public string? InviteCode { get; set; }

    /// <summary>【商户操作时间】 如果邀请状态为MCH_INVITE_SUCC，则是商户确认授权的时间；如果邀请状态为MCH_INVITE_FAILED，则是商户邀请失败的时间。遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>mch_invite_result_list.operate_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("operate_time")]
    public string? OperateTime { get; set; }

    /// <summary>【商户邀请失败错误码】 邀请失败时返回。可选取值：`UNKNOWN_TAX_INFO` 未查询到单位税务登记信息，请联系税局确认税务登记信息 / `INVALID_COMPANY_INFO` 单位信息有误，请确认微信支付平台和税务局登记主体信息一致，微信支付更新主体信息参考： [商家助手修改商户法人、经营者](https://kf.qq.com/faq/230818qqIJnE230818i2mqi6.html) / `NOT_MEET_ACCESS_CONDITION` 不符合乐企接入条件 / `NOT_IN_PILOT_SCOPE` 未纳入数电票试点范围，请参考 [如何进行发票票种核定](https://mp.weixin.qq.com/s/xjT7b0J4yAKSLPs9pYVlYA) 开通数电发票 / `MISSING_TAX_AUTHORITY_CODE` 无法获取省市级主管税务机关代码 / `MERCHANT_INFO_NOT_SET` 商户主体信息未设置，请参考 [商家助手修改商户执照、登记证书](https://kf.qq.com/faq/2308182qaMza230818mi6ZNB.html) 修改商户主体信息 / `UNKNOWN_ERROR` 未知异常（官方字段路径 <c>mch_invite_result_list.invite_failed_code</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("invite_failed_code")]
    public string? InviteFailedCode { get; set; }

    /// <summary>【商户邀请失败的原因】 邀请失败时返回，描述商户邀请失败的具体原因（官方字段路径 <c>mch_invite_result_list.invite_failed_reason</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("invite_failed_reason")]
    public string? InviteFailedReason { get; set; }

}

/// <summary>获取邀请开通的商户信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941524"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoSpInviteMerchantListResponse : WechatPayResponse
{
    /// <summary>【总数】 满足查询条件的所有邀请商户总数（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【本次查询的起始位置】 本次查询的起始位置。从0开始计数，如offset=10，表示从第11条记录开始返回（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【本次查询的最大数量】 每页所查询的商户信息数量（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【相关链接】 相关链接（官方字段路径 <c>links</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("links")]
    public FapiaoSpInviteMerchantListResponseLinks? Links { get; set; }

    /// <summary>【邀请的商户结果】 服务商邀请的商户结果列表（官方字段路径 <c>mch_invite_result_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("mch_invite_result_list")]
    public List<FapiaoSpInviteMerchantListResponseMchInviteResultListItem>? MchInviteResultList { get; set; }

}
