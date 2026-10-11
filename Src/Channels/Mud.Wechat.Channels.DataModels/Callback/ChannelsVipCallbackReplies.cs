// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 「获取小程序会员信息」事件（<c>channels_ec_vip_user_info_get</c>）的结构化应答。
/// </summary>
/// <remarks>
/// <para>
/// 官方（notify/wxa_callback/channels_ec_vip_user_info_get）：用户尝试关联会员时推送，
/// 商家<b>同步应答</b>是否为会员——<see cref="Info.IsVip"/> 为 true 时本次关联<b>静默完成</b>
/// （无需用户跳转小程序确认），并回传商家自定义会员身份与会员 id。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsVipUserInfoGetReply
{
    /// <summary>获取或设置错误码（0 成功）。</summary>
    [JsonPropertyName("errcode")]
    public long ErrCode { get; set; }

    /// <summary>获取或设置错误信息。</summary>
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; } = string.Empty;

    /// <summary>获取或设置会员信息（官方 info 结构）。</summary>
    [JsonPropertyName("info")]
    public ChannelsVipUserInfoGetReplyInfo? Info { get; set; }
}

/// <summary>「获取小程序会员信息」应答的 info 结构（官方字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsVipUserInfoGetReplyInfo
{
    /// <summary>获取或设置是否为会员（true = 是；官方：返回 true 时本次关联走静默关联，无需用户跳转小程序确认）。</summary>
    [JsonPropertyName("is_vip")]
    public bool IsVip { get; set; }

    /// <summary>获取或设置小程序定义的用户会员身份（is_vip 为 true 时需填写）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>获取或设置小程序定义的用户会员 id（is_vip 为 true 时可填写）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }
}

/// <summary>
/// 「下单页获取初始会员权益」事件（<c>channels_ec_gen_order_info_init</c>）的结构化应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsGenOrderInfoInitReply
{
    /// <summary>获取或设置错误码（0 成功）。</summary>
    [JsonPropertyName("errcode")]
    public long ErrCode { get; set; }

    /// <summary>获取或设置错误信息。</summary>
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; } = string.Empty;

    /// <summary>获取或设置会员权益信息（官方 info 结构）。</summary>
    [JsonPropertyName("info")]
    public ChannelsGenOrderInfoInitReplyInfo? Info { get; set; }
}

/// <summary>「下单页获取初始会员权益」应答的 info 结构（官方字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsGenOrderInfoInitReplyInfo
{
    /// <summary>获取或设置会员优惠总金额（分）。</summary>
    [JsonPropertyName("vip_discounted_price")]
    public long VipDiscountedPrice { get; set; }

    /// <summary>获取或设置商品列表（逐商品拆解会员优惠金额，官方必填）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsGenOrderInfoInitReplyProduct>? ProductInfos { get; set; }

    /// <summary>获取或设置是否叠加使用商家优惠（0 叠加，1 不叠加、仅用小程序优惠；默认 0）。</summary>
    [JsonPropertyName("unuse_shop_discount")]
    public long? UnuseShopDiscount { get; set; }
}

/// <summary>「下单页获取初始会员权益」应答的商品条目（官方字段，均必填）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsGenOrderInfoInitReplyProduct
{
    /// <summary>获取或设置商品 spu id。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 sku id。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置会员优惠金额（分）。</summary>
    [JsonPropertyName("vip_discounted_price")]
    public long VipDiscountedPrice { get; set; }
}

/// <summary>
/// 「下单使用会员权益通知」事件（<c>channels_ec_order_use_vip_discount</c>）的结构化应答。
/// </summary>
/// <remarks>
/// 官方（notify/wxa_callback/channels_ec_order_use_vip_discount）：用户提交订单（未支付）使用会员权益时
/// 实时回调，商家校验后经 <see cref="Result"/> 确认——确认通过才能以该优惠金额下单。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsOrderUseVipDiscountReply
{
    /// <summary>获取或设置错误码（0 成功）。</summary>
    [JsonPropertyName("errcode")]
    public long ErrCode { get; set; }

    /// <summary>获取或设置错误信息。</summary>
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; } = string.Empty;

    /// <summary>获取或设置下单确认结果（1 允许下单，2 不允许下单；为空或其他值时会拦截下单）。</summary>
    [JsonPropertyName("result")]
    public long Result { get; set; }
}

/// <summary>
/// 结构化回包加密信封（官方 dev_before/message_push「其他回包内容需加密」的回包格式）。
/// </summary>
/// <remarks>由 <c>WechatChannelsCallbackReplyWriter</c> 组装；字段名照官方原文（PascalCase）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsCallbackReplyEnvelope
{
    /// <summary>获取或设置 AES-256-CBC 加密的应答密文（Base64）。</summary>
    [JsonPropertyName("Encrypt")]
    public string Encrypt { get; set; } = string.Empty;

    /// <summary>获取或设置回包签名（SHA1(token, TimeStamp, Nonce, Encrypt) 四参排序）。</summary>
    [JsonPropertyName("MsgSignature")]
    public string MsgSignature { get; set; } = string.Empty;

    /// <summary>获取或设置回包时间戳（秒）。</summary>
    [JsonPropertyName("TimeStamp")]
    public long TimeStamp { get; set; }

    /// <summary>获取或设置随机数（官方要求回填回调 URL 参数中的 nonce）。</summary>
    [JsonPropertyName("Nonce")]
    public string Nonce { get; set; } = string.Empty;
}

/// <summary>下单确认结果常量（官方 <c>result</c> 取值）。</summary>
public static class ChannelsOrderUseVipDiscountResults
{
    /// <summary>允许下单。</summary>
    public const long Allow = 1;

    /// <summary>不允许下单（拦截）。</summary>
    public const long Deny = 2;
}
