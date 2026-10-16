// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingBusifavor;

/// <summary>向用户发放商品券请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015717371"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorProductCouponSendRequest
{
    /// <summary>【用户OpenID】 OpenID信息，用户在AppID下的唯一标识。校验规则：传入的 OpenID 需是调用方商户号（即请求头里面的商户号）有绑定关系的 AppID 获取的 OpenID。 获取 OpenID 文档（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【公众账号ID】 支持传入与当前调用接口商户号有绑定关系的 AppID。支持小程序 AppID 与公众号 AppID。如何获取AppID可参考 开发必要参数说明。校验规则：传入的 AppID 需与调用方商户号（即请求头里面的商户号）有绑定关系的 AppID 或传入的 AppID 需是归属商户号有绑定关系的 AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【批次号】 微信为每个商家券批次分配的唯一 ID（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【发券凭证】 发券凭证，可包含英文字母，数字，&amp;#124;，_，\*，-等内容，不允许出现其他不合法符号，商户侧需保证在单个批次单个用户下的唯一性（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【券code】 创建商家券时，设定 coupon_code_mode 为 MERCHANT_API，则该字段必填（官方字段路径 <c>coupon_code</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【券类型】 枚举值：NORMAL-普通券；MEMBER-会员券; MEMBER_ACTIVITY-会员活动（官方字段路径 <c>coupon_type</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("coupon_type")]
    public string? CouponType { get; set; }

    /// <summary>【会员卡ID】 当 coupon_type 为 MEMBER 时必填，通过《 创建会员卡模板》接口创建成功时，获得的会员卡模板 card_id（官方字段路径 <c>member_card_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("member_card_id")]
    public string? MemberCardId { get; set; }

}

/// <summary>向用户发放商品券响应·【发券结果】 仅在发券成功时返回，发券失败无此字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015717371"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorProductCouponSendResponseSendCouponResult
{
    /// <summary>【批次号】 微信为每个商家券批次分配的唯一 ID（官方字段路径 <c>send_coupon_result.stock_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【发券凭证】 发券凭证，可包含英文字母，数字，&amp;#124;，_，\*，-等内容，不允许出现其他不合法符号，商户侧需保证在单个批次单个用户下的唯一性（官方字段路径 <c>send_coupon_result.out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【券code】 券的唯一标识（官方字段路径 <c>send_coupon_result.coupon_code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

}

/// <summary>向用户发放商品券响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015717371"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorProductCouponSendResponse : WechatPayResponse
{
    /// <summary>【发券结果】 仅在发券成功时返回，发券失败无此字段（官方字段路径 <c>send_coupon_result</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("send_coupon_result")]
    public BusifavorProductCouponSendResponseSendCouponResult? SendCouponResult { get; set; }

}
