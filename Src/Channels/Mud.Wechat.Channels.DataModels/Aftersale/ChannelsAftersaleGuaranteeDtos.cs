// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「保障单」DTO（aftersale/getguaranteeorder、aftersale/searchguaranteeorder、
// aftersale/merchantacceptguarantee、aftersale/merchantmodifyguarantee、aftersale/merchantproofguarantee、
// aftersale/merchantrefuseguarantee）+ 保障单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>保障单当前状态（官方 <c>guarantee_order.status</c> 枚举）。</summary>
public static class ChannelsGuaranteeStatuses
{
    /// <summary>等待商家处理。</summary>
    public const string WaitMerchantHandle = "STATUS_WAIT_MERCHANT_HANDLE";

    /// <summary>等待平台处理。</summary>
    public const string WaitPlatformHandle = "STATUS_WAIT_PLATFORM_HANDLE";

    /// <summary>等待用户确认。</summary>
    public const string WaitUserConfirm = "STATUS_WAIT_USER_CONFIRM";

    /// <summary>等待商家举证。</summary>
    public const string WaitMerchantProof = "STATUS_WAIT_MERCHANT_PROOF";

    /// <summary>等待用户举证。</summary>
    public const string WaitUserProof = "STATUS_WAIT_USER_PROOF";

    /// <summary>等待双方举证。</summary>
    public const string WaitBothProof = "STATUS_WAIT_BOTH_PROOF";

    /// <summary>等待 OP 确认。</summary>
    public const string WaitOpConfirm = "STATUS_WAIT_OP_COMFIRM";

    /// <summary>等待支付分付款。</summary>
    public const string WaitPayscoreDone = "STATUS_WAIT_PAYSCORE_DONE";

    /// <summary>无需赔付。</summary>
    public const string NoNeedPay = "STATUS_NO_NEED_PAY";

    /// <summary>赔付中。</summary>
    public const string Paying = "STATUS_PAYING";

    /// <summary>假一赔三金额异常，待确认。</summary>
    public const string PayBlock = "STATUS_PAY_BLOCK";

    /// <summary>赔付成功。</summary>
    public const string PaySucc = "STATUS_PAY_SUCC";

    /// <summary>赔付失败。</summary>
    public const string PayFail = "STATUS_PAY_FAIL";

    /// <summary>用户取消申请。</summary>
    public const string UserCancel = "STATUS_USER_CANCEL";
}

/// <summary>保障单类型（官方 <c>guarantee_order.type</c> 枚举）。</summary>
public static class ChannelsGuaranteeTypes
{
    /// <summary>假一赔三。</summary>
    public const int FakeOnePayFour = 1;

    /// <summary>坏损包退。</summary>
    public const int BadPay = 2;
}

/// <summary>获取保障单详情（<c>aftersale/getguaranteeorder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetGuaranteeOrderRequest
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id</c>，必填）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public long GuaranteeOrderId { get; set; }
}

/// <summary>获取保障单详情（<c>aftersale/getguaranteeorder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetGuaranteeOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置保障单详情（官方 <c>guarantee_order</c>）。</summary>
    [JsonPropertyName("guarantee_order")]
    public ChannelsGuaranteeOrderInfo? GuaranteeOrder { get; set; }
}

/// <summary>保障单结构（官方 <c>guarantee_order</c> 对象，<c>getguaranteeorder</c> 响应主载荷）。</summary>
/// <remarks>
/// 保障单类型见 <see cref="ChannelsGuaranteeTypes"/>、状态枚举见 <see cref="ChannelsGuaranteeStatuses"/>。
/// 嵌套对象内部字段以官方保障单文档为准（<c>fake_one_pay_four_info</c> 假一赔三详情 / <c>bad_pay_info</c> 坏损包退详情）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeOrderInfo
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id</c>）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public long? GuaranteeOrderId { get; set; }

    /// <summary>获取或设置保障单类型（官方 <c>type</c>，见 <see cref="ChannelsGuaranteeTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置保障单创建时间戳（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置保障单更新时间戳（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置申请原因（官方 <c>apply_reason</c>）。</summary>
    [JsonPropertyName("apply_reason")]
    public string? ApplyReason { get; set; }

    /// <summary>获取或设置商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public List<ChannelsGuaranteeProductInfo>? ProductInfo { get; set; }

    /// <summary>获取或设置保障单过期时间（官方 <c>expire_time</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>获取或设置买家身份标识（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置买家在开放平台的唯一标识符（官方 <c>unionid</c>）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>pay_amount</c>，单位分）。</summary>
    [JsonPropertyName("pay_amount")]
    public long? PayAmount { get; set; }

    /// <summary>获取或设置商家拒绝原因（官方 <c>merchant_refuse_reason</c>）。</summary>
    [JsonPropertyName("merchant_refuse_reason")]
    public string? MerchantRefuseReason { get; set; }

    /// <summary>获取或设置订单支付信息（官方 <c>order_pay_info</c>）。</summary>
    [JsonPropertyName("order_pay_info")]
    public ChannelsGuaranteeOrderPayInfo? OrderPayInfo { get; set; }

    /// <summary>获取或设置保障单完成时间（官方 <c>complete_time</c>）。</summary>
    [JsonPropertyName("complete_time")]
    public long? CompleteTime { get; set; }

    /// <summary>获取或设置申请原因类型（官方 <c>apply_reason_type</c>）。</summary>
    [JsonPropertyName("apply_reason_type")]
    public int? ApplyReasonType { get; set; }

    /// <summary>获取或设置送礼信息（官方 <c>order_present_info</c>）。</summary>
    [JsonPropertyName("order_present_info")]
    public ChannelsGuaranteeOrderPresentInfo? OrderPresentInfo { get; set; }

    /// <summary>获取或设置发起保障的订单类型（官方 <c>order_type</c>）：0 普通订单；10 礼物订单。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置假一赔三保障单详情（官方 <c>fake_one_pay_four_info</c>）。</summary>
    [JsonPropertyName("fake_one_pay_four_info")]
    public ChannelsGuaranteeFakeOnePayFourInfo? FakeOnePayFourInfo { get; set; }

    /// <summary>获取或设置坏损包退保障单详情（官方 <c>bad_pay_info</c>）。</summary>
    [JsonPropertyName("bad_pay_info")]
    public ChannelsGuaranteeBadPayInfo? BadPayInfo { get; set; }

    /// <summary>获取或设置小程序会员已经优惠金额（官方 <c>wxa_vip_discounted_price</c>，单位分）。</summary>
    [JsonPropertyName("wxa_vip_discounted_price")]
    public long? WxaVipDiscountedPrice { get; set; }

    /// <summary>获取或设置协商历史（官方 <c>history_list</c>）。</summary>
    [JsonPropertyName("history_list")]
    public List<ChannelsGuaranteeHistory>? HistoryList { get; set; }

    /// <summary>获取或设置保障单当前状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsGuaranteeStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

/// <summary>保障单商品信息（官方 <c>guarantee_order.product_info</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeProductInfo
{
    /// <summary>获取或设置商品小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品价格（官方 <c>real_price</c>）。</summary>
    [JsonPropertyName("real_price")]
    public long? RealPrice { get; set; }

    /// <summary>获取或设置商品件数（官方 <c>product_cnt</c>）。</summary>
    [JsonPropertyName("product_cnt")]
    public long? ProductCnt { get; set; }

    /// <summary>获取或设置商品详情（官方 <c>detail</c>）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>获取或设置商品 spu id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }
}

/// <summary>保障单订单支付信息（官方 <c>guarantee_order.order_pay_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeOrderPayInfo
{
    /// <summary>获取或设置交易单号（官方 <c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}

/// <summary>保障单送礼信息（官方 <c>guarantee_order.order_present_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeOrderPresentInfo
{
    /// <summary>获取或设置礼物单号（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public long? PresentOrderId { get; set; }

    /// <summary>获取或设置收礼时间（官方 <c>accept_present_time</c>）。</summary>
    [JsonPropertyName("accept_present_time")]
    public long? AcceptPresentTime { get; set; }

    /// <summary>获取或设置送礼者微信昵称（官方 <c>giver_nickname</c>）。</summary>
    [JsonPropertyName("giver_nickname")]
    public string? GiverNickname { get; set; }
}

/// <summary>假一赔三保障单详情（官方 <c>guarantee_order.fake_one_pay_four_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeFakeOnePayFourInfo
{
    /// <summary>获取或设置鉴定费用（官方 <c>identify_fee</c>）。</summary>
    [JsonPropertyName("identify_fee")]
    public long? IdentifyFee { get; set; }

    /// <summary>获取或设置商品费用（官方 <c>product_fee</c>）。</summary>
    [JsonPropertyName("product_fee")]
    public long? ProductFee { get; set; }

    /// <summary>获取或设置预计赔付（官方 <c>total_pay_fee</c>）。</summary>
    [JsonPropertyName("total_pay_fee")]
    public long? TotalPayFee { get; set; }

    /// <summary>获取或设置鉴定凭证 media_id 列表（官方 <c>identify_proof_pic_list</c>）。</summary>
    [JsonPropertyName("identify_proof_pic_list")]
    public List<string>? IdentifyProofPicList { get; set; }

    /// <summary>获取或设置费用凭证 media_id 列表（官方 <c>fee_proof_pic_list</c>）。</summary>
    [JsonPropertyName("fee_proof_pic_list")]
    public List<string>? FeeProofPicList { get; set; }

    /// <summary>获取或设置申请原因（官方 <c>apply_reason</c>）：1 假一赔三假冒注册商标；2 假一赔三假冒材质成分。</summary>
    [JsonPropertyName("apply_reason")]
    public int? ApplyReason { get; set; }

    /// <summary>获取或设置最终赔付金额（官方 <c>acctual_pay</c>）。</summary>
    [JsonPropertyName("acctual_pay")]
    public long? AcctualPay { get; set; }
}

/// <summary>坏损包退保障单详情（官方 <c>guarantee_order.bad_pay_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeBadPayInfo
{
    /// <summary>获取或设置损坏程度（官方 <c>bad_level</c>）。</summary>
    [JsonPropertyName("bad_level")]
    public int? BadLevel { get; set; }

    /// <summary>获取或设置损坏凭证文字（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置损坏凭证图片 media_id 列表（官方 <c>pic_list</c>）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string>? PicList { get; set; }

    /// <summary>获取或设置赔付金额（官方 <c>pay_fee</c>，含运费）。</summary>
    [JsonPropertyName("pay_fee")]
    public long? PayFee { get; set; }

    /// <summary>获取或设置商家协商损坏程度（官方 <c>merchant_modify_level</c>）。</summary>
    [JsonPropertyName("merchant_modify_level")]
    public int? MerchantModifyLevel { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_remark</c>）。</summary>
    [JsonPropertyName("merchant_remark")]
    public string? MerchantRemark { get; set; }

    /// <summary>获取或设置平台调整损坏程度（官方 <c>platform_modify_level</c>）。</summary>
    [JsonPropertyName("platform_modify_level")]
    public int? PlatformModifyLevel { get; set; }

    /// <summary>获取或设置退款类型（官方 <c>refund_type</c>）：1 仅退款；2 退货退款；3 换货。</summary>
    [JsonPropertyName("refund_type")]
    public int? RefundType { get; set; }

    /// <summary>获取或设置是否收到货（官方 <c>receive_product</c>）：1 未收到货；2 收到货。</summary>
    [JsonPropertyName("receive_product")]
    public int? ReceiveProduct { get; set; }

    /// <summary>获取或设置退款原因（官方 <c>refund_reason</c>）。</summary>
    [JsonPropertyName("refund_reason")]
    public int? RefundReason { get; set; }

    /// <summary>获取或设置退款原因文案（官方 <c>refund_reason_text</c>）。</summary>
    [JsonPropertyName("refund_reason_text")]
    public string? RefundReasonText { get; set; }

    /// <summary>获取或设置凭证图片列表（官方 <c>title_pic_list</c>，按图片类型分组）。</summary>
    [JsonPropertyName("title_pic_list")]
    public List<ChannelsGuaranteeTitlePic>? TitlePicList { get; set; }
}

/// <summary>凭证图片分组（官方 <c>bad_pay_info.title_pic_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeTitlePic
{
    /// <summary>获取或设置凭证图片类型（官方 <c>title</c>）：1 坏损商品图；2 全部商品图；3 包裹图。</summary>
    [JsonPropertyName("title")]
    public int? Title { get; set; }

    /// <summary>获取或设置该类型下的凭证图片 URL 列表（官方 <c>pic_list</c>，旧素材兼容字段，输出图片临时访问 URL）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string>? PicList { get; set; }

    /// <summary>获取或设置新素材列表（官方 <c>new_media_list</c>，支持图片和视频）。</summary>
    [JsonPropertyName("new_media_list")]
    public List<ChannelsGuaranteeMedia>? NewMediaList { get; set; }
}

/// <summary>保障单新素材（官方 <c>title_pic_list[].new_media_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeMedia
{
    /// <summary>获取或设置媒体类型（官方 <c>type</c>）：1 图片；2 视频。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置图片信息（官方 <c>picture</c>，type 为 1 时有值）。</summary>
    [JsonPropertyName("picture")]
    public ChannelsMediaPicture? Picture { get; set; }

    /// <summary>获取或设置视频信息（官方 <c>video</c>，type 为 2 时有值）。</summary>
    [JsonPropertyName("video")]
    public ChannelsMediaVideo? Video { get; set; }
}

/// <summary>保障单协商历史（官方 <c>guarantee_order.history_list</c> 数组元素，字段集以官方文档为准）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeHistory
{
    /// <summary>获取或设置历史操作类型（官方 <c>item_type</c>）。</summary>
    [JsonPropertyName("item_type")]
    public int? ItemType { get; set; }

    /// <summary>获取或设置操作时间（官方 <c>time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>获取或设置相关文本内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置相关图片 media_id 列表（官方 <c>media_id_list</c>）。</summary>
    [JsonPropertyName("media_id_list")]
    public List<string>? MediaIdList { get; set; }

    /// <summary>获取或设置多媒体信息列表（官方 <c>media_infos</c>）。</summary>
    [JsonPropertyName("media_infos")]
    public List<ChannelsMediaInfo>? MediaInfos { get; set; }
}

/// <summary>获取保障单列表（<c>aftersale/searchguaranteeorder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsSearchGuaranteeOrderRequest
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id_list</c>）。</summary>
    [JsonPropertyName("guarantee_order_id_list")]
    public List<long>? GuaranteeOrderIdList { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id_list</c>）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<long>? OrderIdList { get; set; }

    /// <summary>获取或设置想要获取的保障单类型（官方 <c>type</c>）：0 全部；1 假一赔三；2 坏损包退。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置保障单申请的开始时间戳（官方 <c>begin_time</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置保障单申请的结束时间戳（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置保障单当前状态筛选（官方 <c>status_list</c>，枚举值见 <see cref="ChannelsGuaranteeStatuses"/>）。</summary>
    [JsonPropertyName("status_list")]
    public List<string>? StatusList { get; set; }

    /// <summary>获取或设置列表起始下标（官方 <c>offset</c>，默认从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置需要的保障单列表条数（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>获取保障单列表（<c>aftersale/searchguaranteeorder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsSearchGuaranteeOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置保障单详情列表（官方 <c>guarantee_order_list</c>）。</summary>
    [JsonPropertyName("guarantee_order_list")]
    public List<ChannelsGuaranteeOrderInfo>? GuaranteeOrderList { get; set; }

    /// <summary>获取或设置保障单列表总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}

/// <summary>保障单同意（<c>aftersale/merchantacceptguarantee</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMerchantAcceptGuaranteeRequest
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id</c>，必填）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public long GuaranteeOrderId { get; set; }
}

/// <summary>保障单协商（<c>aftersale/merchantmodifyguarantee</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMerchantModifyGuaranteeRequest
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id</c>，必填）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public long GuaranteeOrderId { get; set; }

    /// <summary>获取或设置商家修改坏损比例（官方 <c>bad_level</c>，必填，可填 10/30/50/80/100）。</summary>
    [JsonPropertyName("bad_level")]
    public int BadLevel { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_remark</c>）。</summary>
    [JsonPropertyName("merchant_remark")]
    public string? MerchantRemark { get; set; }
}

/// <summary>保障单举证（<c>aftersale/merchantproofguarantee</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMerchantProofGuaranteeRequest
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id</c>，必填）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public long GuaranteeOrderId { get; set; }

    /// <summary>获取或设置商家举证文字内容（官方 <c>content</c>，必填）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>获取或设置举证 id 列表（官方 <c>pic_list</c>，必填，经 图片上传接口 获取 media_id）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string> PicList { get; set; } = new();
}

/// <summary>保障单拒绝（<c>aftersale/merchantrefuseguarantee</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMerchantRefuseGuaranteeRequest
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id</c>，必填）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public long GuaranteeOrderId { get; set; }

    /// <summary>获取或设置商家拒绝原因（官方 <c>reason</c>，必填）。</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>获取或设置拒绝凭证 id 列表（官方 <c>pic_list</c>，必填，经 图片上传接口 获取 media_id）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string> PicList { get; set; } = new();
}
