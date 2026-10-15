// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandCardMember;

/// <summary>积分兑券结果同步请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015897268"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberPointsCouponConfirmRequest
{
    /// <summary>【积分兑券ID】 用户积分兑券的记录ID，可从 用户积分兑券事件通知 中获取（官方字段路径 <c>record_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("record_id")]
    public string? RecordId { get; set; }

    /// <summary>【积分兑券模板ID】 积分兑券模板ID，由微信支付生成（灰度期间联系微信支付运营获取）（官方字段路径 <c>exchange_coupon_template_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("exchange_coupon_template_id")]
    public string? ExchangeCouponTemplateId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【积分兑券结果】 积分兑券结果。可选取值：`POINT_EXCHANGE_COUPON_ALLOW` 允许积分兑券 / `POINT_EXCHANGE_COUPON_REJECT` 拒绝积分兑券（官方字段路径 <c>result</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>【拒绝兑券原因】 拒绝兑券原因，当积分兑券结果为拒绝时必填（官方字段路径 <c>reject_reason</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

}

/// <summary>积分兑券结果同步响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015897268"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberPointsCouponConfirmResponse : WechatPayResponse
{
    /// <summary>【积分兑券ID】 用户积分兑券的记录ID，可从 用户积分兑券事件通知 中获取（官方字段路径 <c>record_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("record_id")]
    public string? RecordId { get; set; }

    /// <summary>【积分兑券模板ID】 积分兑券模板ID，由微信支付生成（灰度期间联系微信支付运营获取）（官方字段路径 <c>exchange_coupon_template_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("exchange_coupon_template_id")]
    public string? ExchangeCouponTemplateId { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【积分兑券状态】 积分兑券状态。可选取值：`POINT_EXCHANGE_COUPON_SUCCESS` 积分兑券成功 / `POINT_EXCHANGE_COUPON_FAIL` 积分兑券失败（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【商品券ID】 积分兑券模板配置的商品券ID（官方字段路径 <c>product_coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_id")]
    public string? ProductCouponId { get; set; }

    /// <summary>【商品券批次类型】 用于区分商品券的批次类型。可选取值：`PRODUCT_COUPON_STOCK_TYPE_BUSI_FAVOR` 基于商家券创建的商品券 / `PRODUCT_COUPON_STOCK_TYPE_CASH_FAVOR` 基于代金券创建的商品券 / `PRODUCT_COUPON_STOCK_TYPE_BRAND_FAVOR` 商品券2.0（官方字段路径 <c>product_coupon_stock_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_coupon_stock_type")]
    public string? ProductCouponStockType { get; set; }

    /// <summary>【批次ID】 积分兑券模板配置的商品券批次ID（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【兑券成功的券code】 商品券的唯一标识，当积分兑券状态为成功时必填（官方字段路径 <c>coupon_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    /// <summary>【拒绝兑券原因】 拒绝兑券原因，当积分兑券结果为拒绝时必填（官方字段路径 <c>reject_reason</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

}

/// <summary>积分余额同步请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015897280"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberPointsSyncRequest
{
    /// <summary>【商家请求单号】 商家同步积分余额凭据号。商家自定义，注意保持唯一性，仅供参考的格式：品牌ID+时间戳+流水号。字符仅允许包含英文半角的数字、字母、连接线-和下划线_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【积分余额】 积分余额（官方字段路径 <c>point_balance</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("point_balance")]
    public long? PointBalance { get; set; }

}

/// <summary>积分余额同步响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015897280"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberPointsSyncResponse : WechatPayResponse
{
    /// <summary>【商家请求单号】 商家同步积分余额凭据号。商家自定义，注意保持唯一性，仅供参考的格式：品牌ID+时间戳+流水号。字符仅允许包含英文半角的数字、字母、连接线-和下划线_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【积分余额】 积分余额（官方字段路径 <c>point_balance</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("point_balance")]
    public long? PointBalance { get; set; }

}
