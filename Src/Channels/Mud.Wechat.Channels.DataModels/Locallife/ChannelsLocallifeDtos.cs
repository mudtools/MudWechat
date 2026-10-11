// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>上传团购优惠 code（<c>uploadvouchercode</c>）请求体。</summary>
/// <remarks>官方契约：仅「商家预存 code 模式」（<c>code_source_type=2</c>）需要；
/// 一次最多 200 个 code，单个 code 不超过 48 位，仅支持数字和字母。</remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherCodesUploadRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填；通过本地生活 api 或页面创建的商品 id）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>，必填；创建商品时返回的 sku_id）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置商户上传的券 code 列表（官方 <c>codes</c>，必填；一次最多 200 个）。</summary>
    [JsonPropertyName("codes")]
    public List<string> Codes { get; set; } = new();
}

/// <summary>上传团购优惠 code（<c>uploadvouchercode</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherCodesUploadResponse : ChannelsResponse
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置本次上传去重后的 code 总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>获取或设置本次上传操作上传成功个数（官方 <c>success_count</c>）。</summary>
    [JsonPropertyName("success_count")]
    public int? SuccessCount { get; set; }

    /// <summary>获取或设置上传成功的 code 列表（官方 <c>success_codes</c>）。</summary>
    [JsonPropertyName("success_codes")]
    public List<string>? SuccessCodes { get; set; }

    /// <summary>获取或设置本次上传操作上传失败的 code 数（官方 <c>fail_count</c>）。</summary>
    [JsonPropertyName("fail_count")]
    public int? FailCount { get; set; }

    /// <summary>获取或设置导入失败的 code 信息（官方 <c>fail_codes</c>，修改后重试）。</summary>
    [JsonPropertyName("fail_codes")]
    public List<ChannelsVoucherCodeFailItem>? FailCodes { get; set; }
}

/// <summary>上传失败的 code 信息（官方 <c>fail_codes</c> 数组元素，uploadvouchercode 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherCodeFailItem
{
    /// <summary>获取或设置商家自定义券 code（官方 <c>code</c>，代表唯一一张券）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置对应券 code 上传失败的错误码（官方 <c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public int? ErrCode { get; set; }

    /// <summary>获取或设置上传失败的错误信息描述（官方 <c>errmsg</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}

/// <summary>查询用户的团购优惠详情（<c>getvoucher</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<c>code</c> 与 <c>encrypted_code</c> 二者选其一；使用 <c>encrypted_code</c> 查询时不用传 <c>sku_id</c>。
/// 官方说明原文将加密 code 写作 <c>encrypted_data</c>，参数表字段名为 <c>encrypted_code</c>（矛盾照录，按参数表建模）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherRequest
{
    /// <summary>获取或设置用户订单详情页展示的二维码（官方 <c>encrypted_code</c>，选填；需进行 URL 解码，与 <c>code</c> 二选一）。</summary>
    [JsonPropertyName("encrypted_code")]
    public string? EncryptedCode { get; set; }

    /// <summary>获取或设置用户的券 code（官方 <c>code</c>，选填；查询用户的券接口返回的 code，与 <c>encrypted_code</c> 二选一）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>，选填；使用 <c>encrypted_code</c> 查询时不传）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }
}

/// <summary>查询用户的团购优惠详情（<c>getvoucher</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherResponse : ChannelsResponse
{
    /// <summary>获取或设置用户券详情（官方 <c>voucher</c>）。</summary>
    [JsonPropertyName("voucher")]
    public ChannelsVoucher? Voucher { get; set; }
}

/// <summary>
/// 用户团购券详情（官方 <c>voucher</c> 对象；getvoucher / findvoucher(get_list) / consumevoucher /
/// withdrawconsume(revoke) 四端点响应的券对象官方示例字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾照录（示例超集）</b>：getvoucher 参数表仅登记 <c>code</c>/<c>status</c>/<c>send_time</c>/
/// <c>update_time</c>/<c>start_time</c>/<c>end_time</c> 6 字段，但官方示例另含
/// <c>consume_time</c>/<c>refund_time</c>/<c>product_id</c>/<c>consume_store_name</c>/<c>voucher_type</c>/
/// <c>voucher_buy_amount</c>/<c>voucher_actual_amount</c>/<c>telphone_no</c>/<c>openid</c>/<c>code_type</c>/
/// <c>order_id</c>/<c>sku_id</c>（findvoucher 与核销/撤销响应示例同）——按参数表类型口径建模 6 字段，
/// 示例扩展字段照录进 DTO（反序列化容错，缺失不抛错）。
/// </para>
/// <para>金额字段单位为<b>分</b>；时间字段为秒级时间戳。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucher
{
    /// <summary>获取或设置券 code（官方 <c>code</c>；平台生成的 code 是数字加字母组合、最长 32 位，商家自定义 code 不超过 48 位）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置券状态（官方 <c>status</c>，见 <see cref="ChannelsVoucherStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置发放时间（官方 <c>send_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("send_time")]
    public long? SendTime { get; set; }

    /// <summary>获取或设置最近更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置核销生效时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置核销结束时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置核销时间（官方 <c>consume_time</c>，秒级时间戳；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("consume_time")]
    public long? ConsumeTime { get; set; }

    /// <summary>获取或设置退款时间（官方 <c>refund_time</c>，秒级时间戳；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("refund_time")]
    public long? RefundTime { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置核销门店名称（官方 <c>consume_store_name</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("consume_store_name")]
    public string? ConsumeStoreName { get; set; }

    /// <summary>获取或设置券类型（官方 <c>voucher_type</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("voucher_type")]
    public int? VoucherType { get; set; }

    /// <summary>获取或设置券售卖金额（官方 <c>voucher_buy_amount</c>，单位：分；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("voucher_buy_amount")]
    public long? VoucherBuyAmount { get; set; }

    /// <summary>获取或设置券实付金额（官方 <c>voucher_actual_amount</c>，单位：分；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("voucher_actual_amount")]
    public long? VoucherActualAmount { get; set; }

    /// <summary>获取或设置用户手机号（官方 <c>telphone_no</c>——官方历史拼写照抄；官方示例字段，参数表未登记）。</summary>
    [JsonPropertyName("telphone_no")]
    public string? TelphoneNo { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置 code 类型（官方 <c>code_type</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("code_type")]
    public int? CodeType { get; set; }

    /// <summary>获取或设置购买券的订单 ID（官方 <c>order_id</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置用户 unionid（官方 <c>union_id</c>；仅 getvoucher 官方示例出现——照录）。</summary>
    [JsonPropertyName("union_id")]
    public string? UnionId { get; set; }
}

/// <summary>查询用户的团购优惠列表（<c>findvoucher</c>）请求体。</summary>
/// <remarks>官方契约：页大小 <c>1 ≤ page_size ≤ 50</c>；<c>page_ctx</c> 第一次请求填空、后续请求填上次返回值。</remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherListRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置券状态（官方 <c>status</c>，选填，不填代表查所有券，见 <see cref="ChannelsVoucherStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置页大小（官方 <c>page_size</c>，必填，1 ≤ value ≤ 50）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>，必填；第一次请求填空，后续请求值为上次请求的返回）。</summary>
    [JsonPropertyName("page_ctx")]
    public string PageCtx { get; set; } = string.Empty;
}

/// <summary>查询用户的团购优惠列表（<c>findvoucher</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherListResponse : ChannelsResponse
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户券列表（官方 <c>voucher_list</c>；官方未给出元素字段表，官方示例与 getvoucher 券对象同构 ⇒ 共用 <see cref="ChannelsVoucher"/>）。</summary>
    [JsonPropertyName("voucher_list")]
    public List<ChannelsVoucher>? VoucherList { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>）。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}

/// <summary>核销用户的团购优惠（<c>consumevoucher</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约：<c>consume_request_no</c> 为核销操作幂等串（商户自定义、小店下唯一、最长 128 字节）；
/// <c>encrypted_codes</c>（从订单详情页跳转 path 获取）与 <c>codes + sku_id</c> 二选一；
/// 次卡传多次相同值代表核销多次；多张券整体同成功或同失败；数组大小最大为 10。
/// </para>
/// <para>
/// <b>官方文档矛盾照录</b>：官方参数表字段名为 <c>consume_store_name</c>，请求示例写作 <c>store_name</c>
/// ——按参数表建模。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsConsumeVoucherRequest
{
    /// <summary>获取或设置核销操作幂等串（官方 <c>consume_request_no</c>，必填；商户自定义，小店下唯一，最长 128 字节）。</summary>
    [JsonPropertyName("consume_request_no")]
    public string ConsumeRequestNo { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code 列表（官方 <c>encrypted_codes</c>，选填；与 <c>codes + sku_id</c> 二选一，最大 10 个）。</summary>
    [JsonPropertyName("encrypted_codes")]
    public List<string>? EncryptedCodes { get; set; }

    /// <summary>获取或设置用户券 code 列表（官方 <c>codes</c>，选填；与 <c>encrypted_codes</c> 二选一，最大 10 个）。</summary>
    [JsonPropertyName("codes")]
    public List<string>? Codes { get; set; }

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>，选填；非加密 code 核销时必传）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置 C 端展示的核销门店名称（官方 <c>consume_store_name</c>，选填；仅展示，最长 128 字节；官方示例写作 store_name——矛盾照录）。</summary>
    [JsonPropertyName("consume_store_name")]
    public string? ConsumeStoreName { get; set; }

    /// <summary>获取或设置核销渠道（官方 <c>consume_channel</c>，必填，见 <see cref="ChannelsVoucherConsumeChannels"/>）。</summary>
    [JsonPropertyName("consume_channel")]
    public int ConsumeChannel { get; set; }

    /// <summary>获取或设置商家自定义门店 id（官方 <c>out_store_id</c>，选填；仅作标记）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>获取或设置预约单号（官方 <c>reserve_no</c>，选填；在预约白名单内且商品开启预约时，需先预约才能核销）。</summary>
    [JsonPropertyName("reserve_no")]
    public string? ReserveNo { get; set; }
}

/// <summary>核销用户的团购优惠（<c>consumevoucher</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsConsumeVoucherResponse : ChannelsResponse
{
    /// <summary>获取或设置用户券详情列表（官方 <c>voucher_list</c>，与详情/撤销响应券对象同构 ⇒ 共用 <see cref="ChannelsVoucher"/>）。</summary>
    [JsonPropertyName("voucher_list")]
    public List<ChannelsVoucher>? VoucherList { get; set; }

    /// <summary>获取或设置无法操作的券及原因（官方 <c>fail_voucher_list</c>；当返回 10027061 时返回）。</summary>
    [JsonPropertyName("fail_voucher_list")]
    public List<ChannelsVoucherFailItem>? FailVoucherList { get; set; }
}

/// <summary>撤销用户已核销团购优惠（<c>withdrawconsume</c>）请求体。</summary>
/// <remarks>
/// 官方契约：待撤回核销的券最多 10 张且需属于同一订单；次卡传多个相同项代表撤销多次，整体同成功或同失败；
/// 撤销核销次卡时必须传 <c>consume_request_no</c>（说明撤销的核销单号）。
/// <b>官方参数表字段名 <c>reovke_vouchers</c> 为官方历史拼写，照抄原文（勿改写为 revoke）。</b>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsRevokeVoucherRequest
{
    /// <summary>获取或设置撤销操作幂等串（官方 <c>revoke_request_no</c>，必填；商户自定义，请保证在小店下唯一）。</summary>
    [JsonPropertyName("revoke_request_no")]
    public string RevokeRequestNo { get; set; } = string.Empty;

    /// <summary>获取或设置待撤回核销的券列表（官方 <c>reovke_vouchers</c>——官方历史拼写照抄，必填，最多 10 张）。</summary>
    [JsonPropertyName("reovke_vouchers")]
    public List<ChannelsRevokeVoucherItem> ReovkeVouchers { get; set; } = new();

    /// <summary>获取或设置原核销单号（官方 <c>consume_request_no</c>，选填；撤销核销次卡时必传）。</summary>
    [JsonPropertyName("consume_request_no")]
    public string? ConsumeRequestNo { get; set; }
}

/// <summary>待撤回核销的券条目（官方 <c>reovke_vouchers</c> 数组元素，withdrawconsume 请求）。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsRevokeVoucherItem
{
    /// <summary>获取或设置券加密 code（官方 <c>encrypted_code</c>，选填；和券 code 二选一）。</summary>
    [JsonPropertyName("encrypted_code")]
    public string? EncryptedCode { get; set; }

    /// <summary>获取或设置券 code（官方 <c>code</c>，选填；和券加密 code 二选一）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置商品下的 sku id（官方 <c>sku_id</c>，选填；填写 code 时必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置撤销张数（官方 <c>cancel_count</c>，选填；官方已废弃——照录，勿在新代码中使用）。</summary>
    [JsonPropertyName("cancel_count")]
    public int? CancelCount { get; set; }
}

/// <summary>撤销用户已核销团购优惠（<c>withdrawconsume</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsRevokeVoucherResponse : ChannelsResponse
{
    /// <summary>获取或设置用户券详情列表（官方 <c>voucher_list</c>，与详情/核销响应券对象同构 ⇒ 共用 <see cref="ChannelsVoucher"/>）。</summary>
    [JsonPropertyName("voucher_list")]
    public List<ChannelsVoucher>? VoucherList { get; set; }

    /// <summary>获取或设置无法操作的券（官方 <c>fail_voucher_list</c>；明确有券无法核销时返回，含无法核销原因）。</summary>
    [JsonPropertyName("fail_voucher_list")]
    public List<ChannelsVoucherFailItem>? FailVoucherList { get; set; }
}

/// <summary>无法操作的券明细（官方 <c>fail_voucher_list</c> 数组元素；consumevoucher 与 withdrawconsume 响应字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherFailItem
{
    /// <summary>获取或设置操作的券 code（官方 <c>code</c>；核销/撤销时传 code 或传了有效加密 code 时返回）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置操作的券 sku_id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置核销/撤销时传入的加密 code（官方 <c>encrypted_code</c>；加密 code 非法时返回）。</summary>
    [JsonPropertyName("encrypted_code")]
    public string? EncryptedCode { get; set; }

    /// <summary>获取或设置操作结果（官方 <c>ret_code</c>，官方类型为 string）。</summary>
    [JsonPropertyName("ret_code")]
    public string? RetCode { get; set; }

    /// <summary>获取或设置操作结果说明（官方 <c>ret_msg</c>）。</summary>
    [JsonPropertyName("ret_msg")]
    public string? RetMsg { get; set; }
}

/// <summary>查询券账单（<c>getvoucherbilllist</c>）请求体。</summary>
/// <remarks>官方契约：页大小 <c>1 ≤ page_size ≤ 100</c>；<c>page_ctx</c> 第一次请求填空、后续请求填上次返回值。</remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherBillListRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填；通过本地生活 api 创建的商品 id）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置账单日期（官方 <c>bill_date</c>，必填；券发起核销时间，官方示例形态 <c>2023-12-03</c>）。</summary>
    [JsonPropertyName("bill_date")]
    public string BillDate { get; set; } = string.Empty;

    /// <summary>获取或设置页大小（官方 <c>page_size</c>，必填，1 ≤ value ≤ 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>，必填；第一次请求填空，后续请求值为上次请求的返回）。</summary>
    [JsonPropertyName("page_ctx")]
    public string PageCtx { get; set; } = string.Empty;
}

/// <summary>查询券账单（<c>getvoucherbilllist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherBillListResponse : ChannelsResponse
{
    /// <summary>获取或设置用户券账单列表（官方 <c>bill_list</c>）。</summary>
    [JsonPropertyName("bill_list")]
    public List<ChannelsVoucherBill>? BillList { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>；如果为空，说明没有下一页）。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}

/// <summary>券账单条目（官方 <c>bill_list</c> 数组元素，getvoucherbilllist 响应）。</summary>
/// <remarks>金额字段单位均为<b>分</b>；次卡相关金额为单次金额；时间字段为秒级时间戳。</remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherBill
{
    /// <summary>获取或设置商品名称（官方 <c>voucher_name</c>）。</summary>
    [JsonPropertyName("voucher_name")]
    public string? VoucherName { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置券 code（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置购买券的订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置用户在商家品牌 appid 下的 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置购买时间（官方 <c>voucher_buy_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("voucher_buy_time")]
    public long? VoucherBuyTime { get; set; }

    /// <summary>获取或设置券售卖金额（官方 <c>voucher_buy_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("voucher_buy_amount")]
    public long? VoucherBuyAmount { get; set; }

    /// <summary>获取或设置用户支付金额（官方 <c>voucher_pay_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("voucher_pay_amount")]
    public long? VoucherPayAmount { get; set; }

    /// <summary>获取或设置视频号小店商家优惠减价金额（官方 <c>mch_favor_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("mch_favor_amount")]
    public long? MchFavorAmount { get; set; }

    /// <summary>获取或设置视频号小店平台优惠减价金额（官方 <c>platform_favor_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("platform_favor_amount")]
    public long? PlatformFavorAmount { get; set; }

    /// <summary>获取或设置核销时间（官方 <c>consume_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("consume_time")]
    public long? ConsumeTime { get; set; }

    /// <summary>获取或设置核销的门店名称（官方 <c>consume_store_name</c>；商户在核销券接口传入后才会返回）。</summary>
    [JsonPropertyName("consume_store_name")]
    public string? ConsumeStoreName { get; set; }

    /// <summary>获取或设置给商家的已结算金额（官方 <c>mch_settle_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("mch_settle_amount")]
    public long? MchSettleAmount { get; set; }

    /// <summary>获取或设置给达人的结算金额（官方 <c>finder_settle_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("finder_settle_amount")]
    public long? FinderSettleAmount { get; set; }

    /// <summary>获取或设置核销接口传入的商家自定义门店 id（官方 <c>out_store_id</c>）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>获取或设置次序号标识（官方 <c>time_index</c>；2024 年 2 月底次卡上线后返回）。</summary>
    [JsonPropertyName("time_index")]
    public long? TimeIndex { get; set; }

    /// <summary>获取或设置退款时间（官方 <c>refund_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("refund_time")]
    public long? RefundTime { get; set; }

    /// <summary>获取或设置商家结算时间（官方 <c>mch_settle_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("mch_settle_time")]
    public long? MchSettleTime { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }

    /// <summary>获取或设置退款账户（官方 <c>refund_account</c>，见 <see cref="ChannelsVoucherRefundAccounts"/>）。</summary>
    [JsonPropertyName("refund_account")]
    public int? RefundAccount { get; set; }

    /// <summary>获取或设置退款关联的售后单 id（官方 <c>after_sale_id</c>）。</summary>
    [JsonPropertyName("after_sale_id")]
    public string? AfterSaleId { get; set; }

    /// <summary>获取或设置平台技术服务费结算时间（官方 <c>commission_fee_settle_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("commission_fee_settle_time")]
    public long? CommissionFeeSettleTime { get; set; }

    /// <summary>获取或设置平台技术服务费（官方 <c>commission_fee</c>，单位：分）。</summary>
    [JsonPropertyName("commission_fee")]
    public long? CommissionFee { get; set; }
}

/// <summary>团购券状态常量（官方 <c>voucher.status</c> 枚举值；findvoucher 侧官方枚举名 VOUCHER_STATUS_*）。</summary>
public static class ChannelsVoucherStatuses
{
    /// <summary>待使用（VOUCHER_STATUS_VALID）。</summary>
    public const int Valid = 1;

    /// <summary>已使用（VOUCHER_STATUS_USED）。</summary>
    public const int Used = 2;

    /// <summary>已退券（VOUCHER_STATUS_REFUND）。</summary>
    public const int Refund = 3;

    /// <summary>已过期（VOUCHER_STATUS_EXPIRED）。</summary>
    public const int Expired = 4;

    /// <summary>已预约（官方 2026-06-22 新增）。</summary>
    public const int Reserved = 5;
}

/// <summary>团购券核销渠道常量（官方 <c>consume_channel</c> 枚举值，consumevoucher）。</summary>
public static class ChannelsVoucherConsumeChannels
{
    /// <summary>小程序（CONSUME_CHANNEL_MINI_APP）。</summary>
    public const int MiniApp = 1;

    /// <summary>到店（CONSUME_CHANNEL_IN_STORE）。</summary>
    public const int InStore = 2;
}

/// <summary>团购券退款账户常量（官方 <c>refund_account</c> 枚举值，getvoucherbilllist）。</summary>
public static class ChannelsVoucherRefundAccounts
{
    /// <summary>可提现账户。</summary>
    public const int Withdrawable = 2000;

    /// <summary>保证金账户。</summary>
    public const int Deposit = 3000;
}
